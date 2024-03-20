using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToCameraRotator : MonoBehaviour
{
    Camera mainC;
    // Start is called before the first frame update
    void Start()
    {
        mainC = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(mainC.transform.position);
    }

    public void pfakspj()
    {
        Debug.Log("Ahjgkfdlfgdsoagfk");
    }
}
