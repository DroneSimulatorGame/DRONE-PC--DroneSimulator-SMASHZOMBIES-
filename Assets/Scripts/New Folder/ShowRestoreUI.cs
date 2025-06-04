using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ShowRestoreUI : MonoBehaviour
{
    public static ShowRestoreUI Instance;

    public GameObject restoreUI;
    public GameObject topMenuUI;
    public GameObject NotEnoughResourseUI;

    [DoNotSerialize] public GameObject destroyedObject;


    public TextMeshProUGUI notEnoughText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI costText;
    [DoNotSerialize] public int cost;


    public int restoredWalls = 0;
    public int restoredTowers = 0;
    public int restoredHeadquaters = 0;

    public int maxRestoredWalls = 500;
    public int maxRestoredTowers = 100;
    public int maxRestoredHeadquaters = 10;



    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        LoadRestoredData();
    }

    // Update is called once per frame
    void Update()
    {

    }


    public void RestoreObject()
    {
        if (GameManager.Instance.steel >= cost)
        {
            GameManager.Instance.steel -= cost;
            destroyedObject.GetComponent<Buzilgan>().ReplaceBuilding();
            IncrementRestoredBuilding(destroyedObject.GetComponent<Buzilgan>().objectName);
            topMenuUI.SetActive(true);
        }
        else
        {
            int amountOfNeededMoney = cost - GameManager.Instance.steel;
            notEnoughText.text = $"{amountOfNeededMoney}";
            restoreUI.SetActive(false);
            NotEnoughResourseUI.SetActive(true);
        }
    }


    public void SaveRestoredData()
    {
        PlayerPrefs.SetInt("RestoredWalls", restoredWalls);
        PlayerPrefs.SetInt("RestoredTowers", restoredTowers);
        PlayerPrefs.SetInt("RestoredHeadquarters", restoredHeadquaters);
        PlayerPrefs.Save();
    }
    public void LoadRestoredData()
    {
        restoredWalls = PlayerPrefs.GetInt("RestoredWalls", 0);
        restoredTowers = PlayerPrefs.GetInt("RestoredTowers", 0);
        restoredHeadquaters = PlayerPrefs.GetInt("RestoredHeadquarters", 0);
    }

    public void IncrementRestoredBuilding(string buildingName)
    {
        switch (buildingName)
        {
            case "Wall":
                restoredWalls++;
                PlayerPrefs.SetInt("RestoredWalls", restoredWalls);
                break;
            case "Tower":
                restoredTowers++;
                PlayerPrefs.SetInt("RestoredTowers", restoredTowers);
                break;
            case "Command center":
                restoredHeadquaters++;
                PlayerPrefs.SetInt("RestoredHeadquarters", restoredHeadquaters);
                break;
            default:
                Debug.LogWarning($"Unknown building name: {buildingName}");
                return;
        }

        PlayerPrefs.Save();
        CheckAchivedAmount();
    }

    public void CheckAchivedAmount()
    {
        if (maxRestoredHeadquaters == restoredHeadquaters)
        {
            Debug.Log("Restored Headquaters Achieved");
            SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.RestoreCommanCenter);
        }
        if (maxRestoredTowers == restoredTowers)
        {
            Debug.Log("Restored Towers Achieved");
            SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.RestoreTowers);
        }
        if (maxRestoredWalls == restoredWalls)
        {
            Debug.Log("Restored Walls Achieved");
            SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.RestoreWalls);
        }
    }


}
