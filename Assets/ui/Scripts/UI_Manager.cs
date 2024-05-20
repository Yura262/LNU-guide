using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class UI_Manager : MonoBehaviour
{
    //public BatterySavingSwitch batterySavingSwitch;
    //public FloorNumber floorNumber;
    //public LanguageSwitch languageSwitch;
    //public pickedBodyController pickedBodyController;

    public static UI_Manager instance { get; private set; }

    [SerializeField]
    GameObject SearchPanelScrollView;
    [SerializeField]
    GameObject SearchEntryElement;
    int fromA = -1;
    int toA = -1;
    public TMP_InputField fromInputField;
    public TMP_InputField toInputField;
    public GameObject nothingFoundWindow;
    public GameObject pickedbody;
    public PinchDetection pinchDetection;
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public void FromFieldChanged(string text)
    {
        for (var i = SearchPanelScrollView.transform.childCount - 1; i >= 0; i--)
            Destroy(SearchPanelScrollView.transform.GetChild(i).gameObject);
        if (text == "") return;

        var res = AuditorySearch.instance.Search(text);//new Dictionary<int, string> { { 1, "1" }, { 123, "123" } };
        res.Reverse();
        if (res.Count == 0)
        {
            //show nothing text
            if (text != "" && fromA != -1)
                if (text != AuditorySearch.instance.Get(fromA).ToString())
                    nothingFoundWindow.gameObject.SetActive(true);
            Debug.Log("Nothing found");
        }
        else { nothingFoundWindow.gameObject.SetActive(false); }
        foreach (var entry in res)
        {
            GameObject element = Instantiate(SearchEntryElement, SearchPanelScrollView.transform);
            element.GetComponentInChildren<TextMeshProUGUI>().text = entry.ToString();
            element.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                if (entry.navID == toA && toA != -1)
                    return;
                nothingFoundWindow.gameObject.SetActive(false);
                fromA = entry.navID;
                fromInputField.DeactivateInputField();
                fromInputField.text = entry.ToString();
                for (var i = SearchPanelScrollView.transform.childCount - 1; i >= 0; i--)
                    Destroy(SearchPanelScrollView.transform.GetChild(i).gameObject);
            }
            );
        }


    }
    public void ToFieldChanged(string text)
    {

        for (var i = SearchPanelScrollView.transform.childCount - 1; i >= 0; i--)
            Destroy(SearchPanelScrollView.transform.GetChild(i).gameObject);
        //if (text == "") return;
        var res = AuditorySearch.instance.Search(text);//new Dictionary<int, string> { { 1, "1" }, { 123, "123" } };
        res.Reverse();
        if (res.Count == 0)
        {
            //show nothing text
            if (text != "" && toA != -1)
                if (text != AuditorySearch.instance.Get(toA).ToString())
                    nothingFoundWindow.gameObject.SetActive(true);
            Debug.Log("Nothing found");
        }
        else { nothingFoundWindow.gameObject.SetActive(false); }

        foreach (var entry in res)
        {
            GameObject element = Instantiate(SearchEntryElement, SearchPanelScrollView.transform);
            element.GetComponentInChildren<TextMeshProUGUI>().text = entry.ToString();
            element.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                if (entry.navID == fromA && fromA != -1)
                    return;
                nothingFoundWindow.gameObject.SetActive(false);
                toA = entry.navID;
                toInputField.DeactivateInputField();
                toInputField.text = entry.ToString();
                for (var i = SearchPanelScrollView.transform.childCount - 1; i >= 0; i--)
                    Destroy(SearchPanelScrollView.transform.GetChild(i).gameObject);

            }
            );
        }


    }
    public void StartNaving()
    {
        if (fromA != -1 && toA != -1)
        {
            startNav(fromA, toA);
            HideSelectWindow();
            Recenter();
        }
    }
    public void FromFieldEntered(string text)
    {
        var res = AuditorySearch.instance.Search(text);
        if (res.Count == 1)
        {
            fromA = res[0].navID;
            //startNav(res[0].navID);
        }
    }
    public void FromFieldSelect()
    {
        fromInputField.text = "";
        fromA = -1;
    }
    public void FromFieldDeSelect()
    {
        nothingFoundWindow.gameObject.SetActive(false);
    }

    public void ToFieldEntered(string text)
    {
        var res = AuditorySearch.instance.Search(text);
        if (res.Count == 1)
        {
            toA = res[0].navID;
            //startNav(res[0].navID);
        }
    }
    public void ToFieldSelect()
    {
        toInputField.text = "";
        toA = -1;
    }
    public void ToFieldDeSelect()
    {
        nothingFoundWindow.gameObject.SetActive(false);
    }

    public void swap()
    {
        int tmp = toA;
        toA = fromA;
        fromA = tmp;
        if (fromA != -1)
            fromInputField.text = AuditorySearch.instance.Get(fromA).ToString();
        else
            fromInputField.text = "";
        if (toA != -1)
            toInputField.text = AuditorySearch.instance.Get(toA).ToString();
        else
            toInputField.text = "";
        toInputField.DeactivateInputField();
        fromInputField.DeactivateInputField();
        for (var i = SearchPanelScrollView.transform.childCount - 1; i >= 0; i--)
            Destroy(SearchPanelScrollView.transform.GetChild(i).gameObject);
        nothingFoundWindow.gameObject.SetActive(false);
    }



    void startNav(int fromId, int toId)
    {
        if (!NavManager.instance.Navigating)
        {
            NavManager.instance.StartNavigation(fromId, toId);
            //fold some ui panels etc
        }
        else
        {
            Debug.Log("Ми вже навігуємся, скасувати?");
        }
    }
    public void StopNavigation()
    {
        fromA = -1;
        toA = -1;
        fromInputField.text = "";
        toInputField.text = "";
        toInputField.DeactivateInputField();
        fromInputField.DeactivateInputField();
        //stop foreal
        //fold some panels 
    }
    public void OpenSelectWindow()
    {
        pickedbody.gameObject.SetActive(true);
    }
    public void HideSelectWindow()
    {
        pickedbody.gameObject.SetActive(false);
    }
    public void Recenter()
    {
        pinchDetection.ResetCamera();
    }

    void Update()
    {

    }
}
