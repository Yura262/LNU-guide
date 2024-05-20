using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToCameraRotator : MonoBehaviour
{
    Camera mainC;
    public GameObject chToActivateOnNavigation;
    void Start()
    {
        mainC = Camera.main;
    }

    void Update()
    {
        if (NavManager.instance.Navigating)
        {
            chToActivateOnNavigation.SetActive(true);
            if (Vector2.Distance((Vector2)transform.position, (Vector2)mainC.transform.position) > 2)
                transform.LookAt(mainC.transform.position);
        }
        else
            chToActivateOnNavigation.SetActive(false);
    }
    public void PointTo(Vector3 point)
    {
        transform.position = point + new Vector3(0, 10, 0);
    }
    public void pfakspj()
    {
        Debug.Log("Ahjgkfdlfgdsoagfk");
    }
}
