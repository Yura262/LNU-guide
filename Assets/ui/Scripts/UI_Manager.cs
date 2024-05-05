using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour
{
    public BatterySavingSwitch batterySavingSwitch;
    public FloorNumber floorNumber;
    public LanguageSwitch languageSwitch;
    public pickedBodyController pickedBodyController;

    public static UI_Manager instance { get; private set; }

    [SerializeField]
    GameObject SearchPanelScrollView;
    [SerializeField]
    GameObject SearchEntryElement;
    void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }

    public void SearchFieldChanged(string text)
    {
        for (var i = SearchPanelScrollView.transform.childCount - 1; i >= 0; i--)
            Destroy(SearchPanelScrollView.transform.GetChild(i).gameObject);

        var res = AuditorySearch.instance.Search(text);//new Dictionary<int, string> { { 1, "1" }, { 123, "123" } };
        if (res.Count == 0)
        {
            //show nothing text
            Debug.Log("Nothing found");
        }
        foreach (var entry in res)
        {
            GameObject element = Instantiate(SearchEntryElement, SearchPanelScrollView.transform);
            element.GetComponentInChildren<TextMeshProUGUI>().text = entry.ToString();
            element.GetComponentInChildren<Button>().onClick.AddListener(delegate
            {
                startNav(entry.navID);
                for (var i = SearchPanelScrollView.transform.childCount - 1; i >= 0; i--)
                    Destroy(SearchPanelScrollView.transform.GetChild(i).gameObject);
            }
            );
        }


    }

    public void SearchFieldEntered(string text)
    {
        var res = AuditorySearch.instance.Search(text);
        if (res.Count == 1)
        {
            startNav(res[0].navID);
        }
    }

    void startNav(int id)
    {
        if (NavManager.instance.Navigating)
        {
            NavManager.instance.StartNavigation(id);
            //fold some ui panels etc
        }
        else
        {
            Debug.Log("Ми вже навігуємся, скасувати?");
        }
    }
    public void StopNavigation()
    {
        //fold some panels 
    }
    void Update()
    {

    }
}
