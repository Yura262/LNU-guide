using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
[RequireComponent(typeof(NavMeshModifierVolume))]
public class Auditory : MonoBehaviour//, PlaceLocation
{

    public Vector3 Position;
    //public string Name;
    public int navID;
    private IAuditoryStructRequirements privateStruct;
    public IAuditoryStructRequirements auditoryStruct
    {
        get
        {
            return privateStruct;
        }
        set
        {
            if (value.navID != navID)
            {
                throw new Exception("],fyf [eqyz yt ghfw.'");
            }
            privateStruct = value;
            privateStruct.navID = navID;
        }
    }

    private NavMeshModifierVolume NMVolume;
    void Start()
    {
        NMVolume = GetComponent<NavMeshModifierVolume>();
        Position = NMVolume.transform.position;
    }

    void Update()
    {

    }
    public override string ToString()
    {
        return auditoryStruct.ToString();
    }
}
