using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


public struct RomanAuiditoryStruct
{
    int navId;
    string name;
    string number;
    public int navID { get { return navId; } set { navId = value; } }
    public string Name { get { return name; } set { name = value; } }
    public string Number { get { return number; } set { number = value; } }

    public RomanAuiditoryStruct(string number, string name, int navId)
    {
        this.name = name;
        this.number = number;
        this.navId = navId;
    }

    public override string ToString()
    {
        if (name == "")
        {
            return $"{number}";
        }
        else if (number == "")
        {
            return $"{name}";
        }
        else
        {
            return $"{number}; {name}";
        }
    }
}

class AuditorySearch : MonoBehaviour
{
    [SerializeField]
    private List<RomanAuiditoryStruct> mainList;
    TextAsset DbFile;
    public static AuditorySearch instance;
    void Awake()
    {
        DbFile = Resources.Load<TextAsset>("Auditories/AuditoriesList");
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        mainList = new List<RomanAuiditoryStruct>();
        foreach (string line in DbFile.text.Split('\n'))
        {
            //Debug.Log(line);
            string[] parts = line.Split(';');
            RomanAuiditoryStruct element = new RomanAuiditoryStruct(parts[0], parts[1], int.Parse(parts[2]));
            mainList.Add(element);
        }
    }
    public List<RomanAuiditoryStruct> Search(string request)
    {
        List<RomanAuiditoryStruct> resultOfSearch = new List<RomanAuiditoryStruct>();
        if (request == "")
        {
            return resultOfSearch;
        }

        if (char.IsDigit(request[0]))
        {
            for (int i = 0; i < mainList.Count; i++)
            {
                if (mainList[i].Number.StartsWith(request))
                {
                    resultOfSearch.Add(mainList[i]);
                }
            }
            return resultOfSearch;
        }

        else if (char.IsLetter(request[0]))
        {
            string[] partsOfAuditoryName;
            string templeForAllSmallCharactersInAuditory;
            string templeForSearchedWordWithAllSmallCharacters;
            if (request.Contains(" "))
            {
                for (int i = 0; i < mainList.Count; i++)
                {
                    templeForAllSmallCharactersInAuditory = mainList[i].Name.ToLower();
                    templeForSearchedWordWithAllSmallCharacters = request.ToLower();

                    if (templeForAllSmallCharactersInAuditory.Contains(templeForSearchedWordWithAllSmallCharacters))
                    {
                        resultOfSearch.Add(mainList[i]);
                    }

                }

            }
            else
            {
                for (int i = 0; i < mainList.Count; i++)
                {
                    templeForAllSmallCharactersInAuditory = mainList[i].Name.ToLower();
                    templeForSearchedWordWithAllSmallCharacters = request.ToLower();
                    partsOfAuditoryName = templeForAllSmallCharactersInAuditory.Split(' ');
                    foreach (var part in partsOfAuditoryName)
                    {
                        if (part.StartsWith(templeForSearchedWordWithAllSmallCharacters))
                        {
                            resultOfSearch.Add(mainList[i]);
                            break;
                        }
                    }
                }
            }
        }

        return resultOfSearch;

    }

    public List<RomanAuiditoryStruct> GetList()
    {
        return mainList;
    }
    public RomanAuiditoryStruct Get(int navId)
    {
        for (int i = 0; i < mainList.Count; i++)
        {
            if (mainList[i].navID == navId)
            {
                return mainList[i];
            }
        }
        return new RomanAuiditoryStruct();
    }
    public void FilterNotImplementedAuditories(List<int> auditoriesNavIDs)
    {
        for (int i = mainList.Count - 1; i >= 0; i--)
        {
            if (!auditoriesNavIDs.Contains(mainList[i].navID))
            {
                this.mainList.RemoveAt(i);
            }
        }
    }

}