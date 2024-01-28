using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BatterySavingSwitch : MonoBehaviour
{
    public Image batteryIcon;
    public TextMeshProUGUI savingText;

    public void SwitchBatteryMode()
    {
        if (batteryIcon.color != Color.white)
        {
            batteryIcon.color = Color.white;
            savingText.color = Color.white;
        }
        else
        {
            batteryIcon.color = Color.green;
            savingText.color = Color.green;

        }
    }

}
