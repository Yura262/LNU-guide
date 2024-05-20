using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
[RequireComponent(typeof(NavMeshModifierVolume))]
public class Auditory : MonoBehaviour//, PlaceLocation
{
    public Vector3 Position;
    public int navID;
    public RomanAuiditoryStruct auditoryStruct;
    GameObject marker;

    private NavMeshModifierVolume NMVolume;
    void Start()
    {
        NMVolume = GetComponent<NavMeshModifierVolume>();
        Position = NMVolume.transform.position;
        marker = Instantiate(NavManager.instance.MarkerToShowMovingToAudGameobj, Position + Vector3.up * 8, Quaternion.identity, transform);
        marker.SetActive(false);
        NavManager.instance.markAAuditories.AddListener(Mark);
    }
    void Update()
    {

    }
    public override string ToString()
    {
        return auditoryStruct.ToString();
    }
    public void Mark()
    {
        if (this == NavManager.instance.auditoryToGo)
        {
            marker.SetActive(true);
        }
        else
        {
            marker.SetActive(false);
        }
    }
}
