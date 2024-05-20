using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class тучеиетершгтп : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject btn;
    TMPro.TextMeshProUGUI textMeshPro;
    void Start()
    {
        textMeshPro = gameObject.GetComponent<TMPro.TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        if (btn.transform.eulerAngles.z == -90f || btn.transform.eulerAngles.z == 270f)
        {
            textMeshPro.text = "Закінчити";
        }
        else
        {
            textMeshPro.text = "Продовжити";
        }
    }
}
