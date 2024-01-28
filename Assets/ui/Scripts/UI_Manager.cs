using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    public BatterySavingSwitch batterySavingSwitch;
    public FloorNumber floorNumber;
    public LanguageSwitch languageSwitch;
    public pickedBodyController pickedBodyController;


    public static UI_Manager instance { get; private set; }


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        //DontDestroyOnLoad(gameObject);
    }




    void Start()
    {

    }


    void Update()
    {

    }
}
