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
    ISearchRequirements SearchC;

    [SerializeField]
    GameObject SearchPanelScrollView;
    [SerializeField]
    GameObject SearchEntryElement;
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

    public void SearchFieldChanged(string text)
    {
        Debug.Log(text);

        var res = SearchC.Search(text);
        if (res.Count == 1)
        {
            //show nothing text
        }

        foreach (var entry in res)
        {
            GameObject element = Instantiate(SearchEntryElement, SearchPanelScrollView.transform);
            element.GetComponentInChildren<TextMeshProUGUI>().text = entry.Value;
            element.GetComponentInChildren<Button>().onClick.AddListener(delegate { NavManager.instance.StartNavigation(entry.Key); });
        }


    }

    public void SearchFieldEntered(string text)
    {
        var res = SearchC.Search(text);
        if (res.Count == 1)
        {
            NavManager.instance.StartNavigation(res.Keys.ToList()[0]);
        }
    }
    void Start()
    {
        SearchC = GetComponent<ISearchRequirements>();
    }


    void Update()
    {

    }
}
