using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface PlaceLocation
{
    public Vector3 Position { get; set; }
    public string Name { get; set; }
    public int LocationID { get; set; }

}
