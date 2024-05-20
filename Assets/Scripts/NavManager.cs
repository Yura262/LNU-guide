using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.UIElements;

public class NavManager : MonoBehaviour
{
    public bool Navigating { get; private set; }
    public static NavManager instance { get; private set; }
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        gameObject.AddComponent<AuditorySearch>();
    }
    List<Auditory> auditories;//existing on map
    public NavMeshAgent agent;
    float remainingDistance_;
    float DistanceToNextStop = 0;
    public ToCameraRotator NavPanelUI;
    public LineRenderer NavLineRenderer;
    Vector3 prevposition;
    public Auditory auditoryToGo;
    Vector3? startPosition;
    public PinchDetection pinchDetection;
    public GameObject pointer;
    public GameObject MarkerToShowMovingToAudGameobj;
    public GameObject topPanel;
    public UnityEvent markAAuditories;
    public GameObject nextButton;
    void Start()
    {
        Navigating = false;
        prevposition = transform.position;
        auditories = FindObjectsOfType<Auditory>().ToList();
        foreach (var a in auditories)
            Debug.Log(a);

        AuditorySearch.instance.FilterNotImplementedAuditories(auditories.Select(c => c.navID).ToList());

        foreach (var val in auditories)
            val.auditoryStruct = AuditorySearch.instance.Get(val.navID);
        nextButton.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    void Update()
    {
        if (Navigating)
        {
            topPanel.SetActive(true);
        }
        else
            topPanel.SetActive(false);
        if (agent.hasPath)
        {
            if (agent.path.corners.Length > 2)
            {
                NavLineRenderer.positionCount = agent.path.corners.Length;

                for (var i = 0; i < agent.path.corners.Length; i++)
                {
                    if (i == 0)
                        NavLineRenderer.SetPosition(i, agent.path.corners[i]);
                    else if (i == 1)
                    {
                        Vector3 lookObj = new Vector3(agent.path.corners[i].x, pointer.transform.position.y, agent.path.corners[i].z);
                        pointer.transform.LookAt(lookObj);
                        NavLineRenderer.SetPosition(i, agent.path.corners[i] + Vector3.up * 0.5f);
                    }
                    else
                        NavLineRenderer.SetPosition(i, agent.path.corners[i] + Vector3.up * 0.5f);
                }
            }

            if (RemainingDistance(agent.path.corners) <= remainingDistance_ - DistanceToNextStop)
            {
                agent.isStopped = true;
                remainingDistance_ = RemainingDistance(agent.path.corners);
                if (remainingDistance_ < 0.5f)
                {
                    //stop navigation
                    Stop();
                }
                if (remainingDistance_ - DistanceToNextStop < 0.5f)
                {
                    nextButton.transform.eulerAngles = new Vector3(0, 0, -90);
                    //do red btn
                }

                movePanel(DistanceToNextStop);

            }
            if (remainingDistance_ == 0 && auditoryToGo != null)
            {
                remainingDistance_ = RemainingDistance(agent.path.corners);
                DistanceToNextStop = remainingDistance_ / 5;
                movePanel(DistanceToNextStop);
            }
        }
        else
        {
            NavLineRenderer.positionCount = 0;
        }
        if (transform.position != prevposition)
        {
            prevposition = transform.position;
            if (!agent.CalculatePath(auditoryToGo.Position, agent.path))
                Debug.Log("noPath");
        }


    }
    public void PanelClickNext()
    {
        agent.isStopped = false;
        Debug.Log("click");
        Debug.Log(nextButton.transform.eulerAngles.z);
        if (nextButton.transform.eulerAngles.z == -90f || nextButton.transform.eulerAngles.z == 270f)
        {
            Debug.Log("stop");
            //stop
            Stop();
        }
        //StartCoroutine(moveForSomeDistance(DistanceToNextStop));
        //remainingDistance = DistanceToNextStop + RemainingDistance(agent.path.corners);
    }
    void movePanel(float distanceToNextStop)
    {
        Vector3 previousCorner = agent.path.corners[0];
        float lengthSoFar = 0.0F;
        int i = 0;
        Vector3 NextStopPosition = agent.path.corners[0];
        while (i < agent.path.corners.Length)
        {
            //Debug.Log(i);
            //Debug.Log(agent.path.corners.Length);
            //Debug.Log(agent.path.corners.Count());
            NextStopPosition = agent.path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, NextStopPosition);
            previousCorner = NextStopPosition;
            i++;
            if (lengthSoFar >= distanceToNextStop)
                break;
        }
        remainingDistance_ = RemainingDistance(agent.path.corners);
        NavPanelUI.PointTo(NextStopPosition);

        //Vector3 previousLookAt = NextStopPosition;
        //StartCoroutine(SmoothLookAt(NextStopPosition, 1f));
        StartCoroutine(planCoor(NextStopPosition));
    }
    IEnumerator planCoor(Vector3 pos)
    {
        StartCoroutine(SmoothLookAt(pos, 1f));
        yield return new WaitForSeconds(1);
        StartCoroutine(SmoothLookAt(pos, 1f));
        yield return new WaitForSeconds(1);
        StartCoroutine(SmoothLookAt(pos, 1f));
    }

    IEnumerator SmoothLookAt(Vector3 worldPoint, float duration)
    {
        Quaternion startRot = pinchDetection.FollowObj.transform.rotation;
        Quaternion endRot = Quaternion.LookRotation(worldPoint - pinchDetection.FollowObj.transform.position, pinchDetection.FollowObj.transform.up);
        for (float t = 0f; t < duration; t += Time.deltaTime)
        {
            pinchDetection.FollowObj.transform.rotation = Quaternion.Slerp(startRot, endRot, t / duration);
            yield return null;
        }
        pinchDetection.FollowObj.transform.rotation = endRot;
    }
    public void StartNavigation(int fromNavId, int toNavId)
    {
        Stop();
        agent.Warp(auditories.Find(x => x.navID == fromNavId).Position);
        startPosition = agent.gameObject.transform.position;
        Debug.Log("navid" + toNavId.ToString());
        foreach (var a in auditories)
            Debug.Log(a);
        auditoryToGo = auditories.Find(x => x.navID == toNavId);
        if (auditoryToGo == null)
        {
            Debug.Log("Auditory not implemented");
            return;
        }
        agent.SetDestination(auditoryToGo.Position);
        markAAuditories.Invoke();
        if (!agent.CalculatePath(auditoryToGo.Position, agent.path))
            Debug.Log("noPath");
        agent.isStopped = true;
        Navigating = true;
        nextButton.transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public float RemainingDistance(Vector3[] points)
    {
        if (points.Length < 2) return 0;
        float distance = 0;
        for (int i = 0; i < points.Length - 1; i++)
            distance += Vector3.Distance(points[i], points[i + 1]);
        ////Debug.Log("dist" + distance.ToString());
        return distance;
    }
    public void Stop()
    {
        startPosition = null;
        auditoryToGo = null;
        agent.isStopped = true;
        Navigating = false;
        agent.ResetPath();
        agent.path.ClearCorners();
        UI_Manager.instance.StopNavigation();
        markAAuditories.Invoke();
        nextButton.transform.eulerAngles = new Vector3(0, 0, 0);
        //play ad :)
    }
}
