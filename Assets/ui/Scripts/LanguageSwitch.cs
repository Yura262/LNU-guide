using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LanguageSwitch : MonoBehaviour
{

    public TextMeshProUGUI currentLang;
    public TextMeshProUGUI otherLang;




    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SwitchLanguage()
    {
        //sys Events manager.lang changed event()
        string tempText = currentLang.text;
        currentLang.text = otherLang.text;
        otherLang.text = tempText;
    }


}
