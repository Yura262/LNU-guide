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
        marker = Instantiate(NavManager.instance.MarkerToShowMovingToAudGameobj, Position, Quaternion.identity);

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
        marker.SetActive(!marker.activeInHierarchy);
    }
}
