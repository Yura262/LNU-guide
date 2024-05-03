using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using Unity.VisualScripting;
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
    Vector3 prevposition;
    Auditory auditoryToGo;
    void Start()
    {
        prevposition = transform.position;
        auditories = FindObjectsOfType<Auditory>().ToList();
        foreach (var a in auditories)
            Debug.Log(a);
        ISearchRequirements Stwwdas;
        if (TryGetComponent<ISearchRequirements>(out Stwwdas))
        {
            ISearchRequirements SearchC = GetComponent<ISearchRequirements>();
            SearchC.FilterNotImplementedAuditories(auditories.Select(c => c.navID).ToList());

            foreach (var val in auditories)
                val.auditoryStruct = SearchC.Get(val.navID);
        }
    }

    void Update()
    {
        //Debug.Log(agent.path.corners.Count());
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
            //Debug.Log(RemainingDistance(agent.path.corners));
            //Debug.Log(remainingDistance_);
            //Debug.Log(DistanceToNextStop);
            if (RemainingDistance(agent.path.corners) <= remainingDistance_ - DistanceToNextStop)
            {
                agent.isStopped = true;
                remainingDistance_ = RemainingDistance(agent.path.corners);
                //DistanceToNextStop = remainingDistance / 5;/////
                movePanel(DistanceToNextStop);
            }
            if (remainingDistance_ == 0 && auditoryToGo != null)
            {
                //Debug.Log(remainingDistance_);
                //Debug.Log(RemainingDistance(agent.path.corners));
                remainingDistance_ = RemainingDistance(agent.path.corners);
                //Debug.Log(remainingDistance_);
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
        //while (agent.pathPending)
        //{
        //staswaitForPath
        //}
        agent.isStopped = true;
        //totalDistance = RemainingDistance(agent.path.corners);
        //Debug.Log("fsfas");



    }
    //IEnumerator waitForPath()
    //{
    //    yield return new WaitUntil(() => agent.pathPending == false);
    //}
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
public interface IAuditoryStructRequirements
{
    int navID { get; set; }
    string Name { get; set; }//аудиторія імені Банаха
    string Number { get; set; }//123a
    string ToString(); //return "123f, аудиторія імені Банаха"

}
public interface ISearchRequirements
{
    Dictionary<int, IAuditoryStructRequirements> Search(string request);//int - navId; request - user input; returns auditories sorted by probability of suiting a request

    Dictionary<int, IAuditoryStructRequirements> GetList();//returns a list of all auditories
    IAuditoryStructRequirements Get(int navId);//mainly for initializing many auditories on start of execution (maybe pass a sorted list? )
    //string GetName(int navId);//get аудиторія імені Банаха
    //string GetNumber(int navId);//get 123a
    void FilterNotImplementedAuditories(List<int> auditories)//list of navIDs that are on map
    {

        //removes elements that are not in auditories list (not implemented on map)
        //auditoriesInDB = auditoriesInDB.Where(p => auditories.Contains(p.Key));
    }

}