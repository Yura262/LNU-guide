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
    float totalDistance;
    float remainingDistance;
    float DistanceToNextStop;
    public ToCameraRotator NavPanelUI;
    void Start()
    {

        auditories = GetComponents<Auditory>().ToList();
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
        //Debug.Log(agent.remainingDistance);
        if (agent.gameObject.activeInHierarchy)
        {
            //some step detection can go here
            if (remainingDistance <= DistanceToNextStop)
            {
                remainingDistance = agent.remainingDistance;
                DistanceToNextStop = remainingDistance / 5;/////
                movePanel(DistanceToNextStop);
            }
        }


    }
    public void PanelClickNext()
    {
        remainingDistance = DistanceToNextStop;
    }
    void movePanel(float distanceToNextStop)
    {
        Vector3 previousCorner = agent.path.corners[0];
        float lengthSoFar = 0.0F;
        int i = 1;
        Vector3 NextStopPosition = agent.path.corners[0];
        while (i < agent.path.corners.Length)
        {
            Vector3 currentCorner = agent.path.corners[i];
            lengthSoFar += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
            i++;
            NextStopPosition = agent.path.corners[i];
            if (lengthSoFar >= distanceToNextStop)
                break;
        }
        NavPanelUI.PointTo(NextStopPosition);
    }
    public void StartNavigation(int navId)
    {
        Auditory auditoryToGo = auditories.Find(x => x.navID == navId);
        if (auditoryToGo == null)
        {
            Debug.LogError("Auditory not implemented");
            return;
        }
        agent.SetDestination(auditoryToGo.Position);
        totalDistance = agent.remainingDistance;
        remainingDistance = agent.remainingDistance;
        DistanceToNextStop = remainingDistance - DistanceToNextStop;
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