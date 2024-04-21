using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToCameraRotator : MonoBehaviour
{
    Camera mainC;

    void Start()
    {
        mainC = Camera.main;
    }

    void Update()
    {
        if (Vector2.Distance((Vector2)transform.position, (Vector2)mainC.transform.position) > 2)
            transform.LookAt(mainC.transform.position);
    }
    public void PointTo(Vector3 point)
    {
        transform.position = point + new Vector3(0, 5, 0);
    }
    public void pfakspj()
    {
        Debug.Log("Ahjgkfdlfgdsoagfk");
    }
}
