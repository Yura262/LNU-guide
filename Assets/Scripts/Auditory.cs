using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
[RequireComponent(typeof(NavMeshModifierVolume))]
public class Auditory : MonoBehaviour//, PlaceLocation
{

    public Vector3 Position;
    public string Name;
    public int navID;

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
        return navID.ToString() + " " + Name + " " + Position.ToString();
    }
}
