using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
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
    Auditory auditoryToGo;
    Vector3? startPosition;
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

    }

    void Update()
    {
        if (agent.hasPath)
        {
            if (agent.path.corners.Length > 2)
            {
                NavLineRenderer.positionCount = agent.path.corners.Length;

                for (var i = 0; i < agent.path.corners.Length; i++)
                {

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
            Debug.Log(i);
            Debug.Log(agent.path.corners.Length);
            Debug.Log(agent.path.corners.Count());
            NextStopPosition = agent.path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, NextStopPosition);
            previousCorner = NextStopPosition;
            i++;
            if (lengthSoFar >= distanceToNextStop)
                break;
        }
        remainingDistance_ = RemainingDistance(agent.path.corners);
        NavPanelUI.PointTo(NextStopPosition);
    }
    public void StartNavigation(int navId)
    {
        startPosition = agent.gameObject.transform.position;
        Debug.Log("navid" + navId.ToString());
        foreach (var a in auditories)
            Debug.Log(a);
        auditoryToGo = auditories.Find(x => x.navID == navId);
        if (auditoryToGo == null)
        {
            Debug.Log("Auditory not implemented");
            return;
        }
        agent.SetDestination(auditoryToGo.Position);
        if (!agent.CalculatePath(auditoryToGo.Position, agent.path))
            Debug.Log("noPath");
        agent.isStopped = true;
        Navigating = true;
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
        agent.ResetPath();
        Navigating = false;
        UI_Manager.instance.StopNavigation();

        //play ad :)
    }
}
