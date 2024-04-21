using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Android;
using UnityEngine.UIElements;

public class NavManager : MonoBehaviour
{
    public static NavManager instance { get; private set; }
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    List<Auditory> auditories;//existing on map
    public NavMeshAgent agent;
    //float totalDistance;
    float remainingDistance_;
    float DistanceToNextStop = 0;
    public ToCameraRotator NavPanelUI;
    public LineRenderer NavLineRenderer;
    void Start()
    {

        auditories = FindObjectsOfType<Auditory>().ToList();
        foreach (var a in auditories)
            Debug.Log(a);
        ISearchRequirements Stwwdas;
        if (TryGetComponent<ISearchRequirements>(out Stwwdas))
        {
            ISearchRequirements SearchC = GetComponent<ISearchRequirements>();
            SearchC.FilterNotImplementedAuditories(auditories);

            foreach (var val in auditories)
                val.Name = SearchC.GetName(val.navID);
        }
    }

    void Update()
    {
        Debug.Log(agent.path.corners.Count());
        if (agent.hasPath)
        {
            if (agent.path.corners.Length > 2)
            {
                NavLineRenderer.positionCount = agent.path.corners.Length;

                for (var i = 0; i < agent.path.corners.Length; i++)
                {

                    NavLineRenderer.SetPosition(i, agent.path.corners[i]);
                }
            }

            //some step detection can go here
            Debug.Log(RemainingDistance(agent.path.corners));
            //Debug.Log(remainingDistance_);
            //Debug.Log(DistanceToNextStop);
            if (RemainingDistance(agent.path.corners) <= remainingDistance_ - DistanceToNextStop)
            {
                agent.isStopped = true;
                remainingDistance_ = RemainingDistance(agent.path.corners);
                //DistanceToNextStop = remainingDistance / 5;/////
                movePanel(DistanceToNextStop);
            }
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
        NavPanelUI.PointTo(NextStopPosition);
    }
    public void StartNavigation(int navId)
    {
        foreach (var a in auditories)
            Debug.Log(a);
        Auditory auditoryToGo = auditories.Find(x => x.navID == navId);
        if (auditoryToGo == null)
        {
            Debug.LogError("Auditory not implemented");
            return;
        }
        agent.SetDestination(auditoryToGo.Position);
        if (!agent.CalculatePath(auditoryToGo.Position, agent.path))
            Debug.LogError("noPath");
        agent.isStopped = true;
        //totalDistance = RemainingDistance(agent.path.corners);
        Debug.Log(remainingDistance_);
        Debug.Log(RemainingDistance(agent.path.corners));
        remainingDistance_ = RemainingDistance(agent.path.corners);
        Debug.Log(remainingDistance_);
        DistanceToNextStop = remainingDistance_ / 5;
        movePanel(DistanceToNextStop);


    }
    IEnumerator moveForSomeDistance(float distance)
    {
        float startRemD = remainingDistance_;
        agent.isStopped = false;
        while (RemainingDistance(agent.path.corners) > startRemD - distance)
            yield return null;
        agent.isStopped = true;
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
}

public interface ISearchRequirements
{
    //Dictionary<int, string> auditoriesInDB 

    Dictionary<int, string> Search(string request);//int - navId, string - Name; request - user input; returns auditories sorted by probability of suiting a request

    Dictionary<int, string> GetList();//returns a list of all auditories

    string GetName(int navId);//self explanatory (див матан)

    void FilterNotImplementedAuditories(List<Auditory> auditories)
    {
        //removes elements that are not in auditories list
        //auditoriesInDB = auditoriesInDB.Where(p => GetComponents<Auditory>().ToList().Select(a => a.navID).Contains(p.Key));
    }

}