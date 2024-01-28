using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorNumber : MonoBehaviour
{
    int _floor;



    [Header("Numbers")]
    public GameObject _0;
    public GameObject _1;
    public GameObject _2;
    public GameObject _3;
    public GameObject _4;
    [Header("UI Elements")]
    public GameObject stairsPanel;




    public int floor
    {
        get { return _floor; }
        set
        {
            _0.gameObject.SetActive(false);
            _1.gameObject.SetActive(false);
            _2.gameObject.SetActive(false);
            _3.gameObject.SetActive(false);
            _4.gameObject.SetActive(false);
            switch (value)
            {
                case 0:
                    _0.gameObject.SetActive(true);
                    break;
                case 1:
                    _1.gameObject.SetActive(true);
                    break;
                case 2:
                    _2.gameObject.SetActive(true);
                    break;
                case 3:
                    _3.gameObject.SetActive(true);
                    break;
                case 4:
                    _4.gameObject.SetActive(true);
                    break;

                default:
                    Debug.LogError("No such floor number");
                    break;
            }
            _floor = value;
        }
    }



    void Start()
    {
        if (stairsPanel.activeInHierarchy) { stairsPanel.SetActive(false); }
    }

    void Update()
    {

    }
    public void BeginFloorAnimation(Single floorToTransferTo)
    {
        floor = Convert.ToInt16(floorToTransferTo);
        StartCoroutine(SimpleAnimation());
    }
    private IEnumerator SimpleAnimation()
    {
        stairsPanel.SetActive(true);
        yield return new WaitForSeconds(1);
        stairsPanel.SetActive(false);
    }
}

