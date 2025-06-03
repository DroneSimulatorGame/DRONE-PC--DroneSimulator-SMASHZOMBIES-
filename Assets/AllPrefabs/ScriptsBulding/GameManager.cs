using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;



public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int waveLevel = 1;
    public int gold = 0;
    public int steel = 0;
    public TMP_Text NotEnoughtText;
    public TMP_Text NotEnoughtText2;

    public Building headquarters1;
    public Building headquarters2;
    public Building headquarters3;
    public Building architecture;
    public Building storage;
    public Building walls;
    public Building tower;
    public Building workshop;

    public Building headquarters_des_1;
    public Building headquarters_des_2;
    public Building headquarters_des_3;

    public Building architecture_des_1;
    public Building architecture_des_2;
    public Building architecture_des_3;

    public Building storage_des_1;
    public Building storage_des_2;
    public Building storage_des_3;

    public Building walls_des_1;
    public Building walls_des_2;
    public Building walls_des_3;
    public Building tower_des_1;
    public Building tower_des_2;
    public Building tower_des_3;

    public Building workshop_des;

    public bool yangila = false;
    public bool ismakeBomb = false;
    public int availableStorages = 0;
    public int availableTowers = 0;
    public int availableLamp = 0;
    public GameObject MapUI;
    public GameObject TopUI;


    public GameObject notEnoughUI;

    private Dictionary<string, bool> loggedBuildings = new Dictionary<string, bool>();




    //public float saveInterval = 2f;

    //private float saveTimer = 0f;
    private float timer = 0f;
    private float interval = 5f;


    void Awake()
    {

        Instance = this;
    }

    void Start()
    {
        FindBuilding();
        loggedBuildings["Headquarters3"] = false;
        loggedBuildings["Architecture"] = false;
        loggedBuildings["Storage"] = false;
        loggedBuildings["Walls"] = false;
        loggedBuildings["Tower"] = false;
        loggedBuildings["Workshop"] = false;
        //Debug.Log("Game started. Upgrade your buildings!");
    }
    public void FindBuilding()
    {
        headquarters1 = FindObjectOfType<Headquarters1>();
        headquarters2 = FindObjectOfType<Headquarters2>();
        headquarters3 = FindObjectOfType<Headquarters3>();
        architecture = FindObjectOfType<Architecture>();
        storage = FindObjectOfType<Storage>();
        walls = FindObjectOfType<Walls>();
        tower = FindObjectOfType<Tower>();
        workshop = FindObjectOfType<Workshop>();

        headquarters_des_1 = FindObjectOfType<DestroyHeadquarter_1>();
        headquarters_des_2 = FindObjectOfType<DestroyHeadquarter_2>();
        headquarters_des_3 = FindObjectOfType<DestroyHeadquarter_3>();

        architecture_des_1 = FindObjectOfType<DestroyArchitecture_1>();
        architecture_des_2 = FindObjectOfType<DestroyArchitecture_2>();
        architecture_des_3 = FindObjectOfType<DestroyArchitecture_3>();

        storage_des_1 = FindObjectOfType<DestroySklad_1>();
        storage_des_2 = FindObjectOfType<DestroySklad_2>();
        storage_des_3 = FindObjectOfType<DestroySklad_3>();

        walls_des_1 = FindObjectOfType<DestroyWall_1>();
        walls_des_2 = FindObjectOfType<DestroyWall_2>();
        walls_des_3 = FindObjectOfType<DestroyWall_3>();

        tower_des_1 = FindObjectOfType<DestroyTower_1>();
        tower_des_2 = FindObjectOfType<DestroyTower_2>();
        tower_des_3 = FindObjectOfType<DestroyTower_3>();

        workshop_des = FindObjectOfType<DestroyWorkshop>();
        CheckAllBuildings();
    }
    public void CheckMissingBuildings()
    {
        CheckBuilding(headquarters1, "Headquarters1");
        CheckBuilding(headquarters2, "Headquarters2");
        CheckBuilding(headquarters3, "Headquarters3");
        CheckBuilding(architecture, "Architecture");
        CheckBuilding(storage, "Storage");
        CheckBuilding(walls, "Walls");
        CheckBuilding(tower, "Tower");
        CheckBuilding(workshop, "Workshop");

        CheckBuilding(headquarters_des_1, "DestroyHeadquarter_1");
        CheckBuilding(headquarters_des_2, "DestroyHeadquarter_2");
        CheckBuilding(headquarters_des_3, "DestroyHeadquarter_3");

        CheckBuilding(architecture_des_1, "DestroyArchitecture_1");
        CheckBuilding(architecture_des_2, "DestroyArchitecture_2");
        CheckBuilding(architecture_des_3, "DestroyArchitecture_3");

        CheckBuilding(storage_des_1, "DestroySklad_1");
        CheckBuilding(storage_des_2, "DestroySklad_2");
        CheckBuilding(storage_des_3, "DestroySklad_3");

        CheckBuilding(walls_des_1, "DestroyWall_1");
        CheckBuilding(walls_des_2, "DestroyWall_2");
        CheckBuilding(walls_des_3, "DestroyWall_3");

        CheckBuilding(tower_des_1, "DestroyTower_1");
        CheckBuilding(tower_des_2, "DestroyTower_2");
        CheckBuilding(tower_des_3, "DestroyTower_3");

        CheckBuilding(workshop_des, "DestroyWorkshop");
    }

    void CheckBuilding(Building building, string buildingName)
    {
        if ((building == null || building.gameObject == null))// || !building.gameObject.activeInHierarchy) && yangila)
        {

            FindBuilding();
            yangila = false;

        }
        else
        {
            yangila = true;
        }
    }
    public void UpgradeBuilding(Building building)
    {
        if (building == null)
        {
            //Debug.Log("Building object is null.");
            return;
        }

        if (UpgradeManager.Instance.IsBuildingUpgrading(building.buildingName, building.transform.position, building.transform.rotation.eulerAngles))
        {
            //Debug.Log($"Attempting to upgrade: {building.buildingName}");
            return;
        }





        if (gold >= building.goldCost && steel >= building.steelCost)
        {
            bool canUpgrade = false;


            switch (building.buildingName)
            {
                case "Headquarters1":
                    if (building.level == 1 || building.level == 2)
                    {
                        canUpgrade = true;
                        if (building.level == 2)
                        {
                            //Debug.Log("Storage is unlocked ");
                        }
                    }
                    break;
                case "Headquarters2":
                    if (building.level == 1 || building.level == 2)
                    {
                        canUpgrade = true;
                        if (building.level == 2)
                        {
                            //Debug.Log("Storage is unlocked ");
                        }
                    }
                    break;
                case "Headquarters3":
                    if (building.level == 1 || building.level == 2)
                    {
                        canUpgrade = true;
                        if (building.level == 2)
                        {
                            //Debug.Log("Storage is unlocked ");
                        }
                    }
                    break;

                case "Architecture":

                    if (headquarters2 != null && headquarters2.level == 2 && building.level == 1)
                    {
                        canUpgrade = true;
                    }
                    else if (headquarters3 != null && headquarters3.level == 3 && building.level == 2)
                    {
                        canUpgrade = true;
                    }
                    else if (headquarters3 != null && headquarters3.level == 3 && building.level == 1)
                    {
                        canUpgrade = true;
                    }
                    else
                    {
                        canUpgrade = false;
                    }
                    break;

                case "Storage":

                    if (building.level == 1 && architecture.level > 1)
                    {
                        canUpgrade = true;
                    }
                    else if (building.level == 2 && architecture.level > 2)
                    {
                        canUpgrade = true;
                    }
                    else
                    {
                        canUpgrade = false;
                    }

                    break;


                case "Tower":

                    if (building.level == 1 && architecture.level > 1)
                    {
                        canUpgrade = true;
                    }
                    else if (building.level == 2 && architecture.level > 2)
                    {
                        canUpgrade = true;
                    }
                    else
                    {
                        canUpgrade = false;
                    }
                    break;


                case "Walls":

                    if (building.level == 1 && architecture.level > 1)
                    {
                        canUpgrade = true;
                    }
                    else if (building.level == 2 && architecture.level > 2)
                    {
                        canUpgrade = true;
                    }
                    else
                    {
                        canUpgrade = false;
                    }

                    break;

            }

            if (canUpgrade)
            {
                gold -= building.goldCost;
                steel -= building.steelCost;
                //building.Upgrade();
                if (building == null)
                {
                    //Debug.Log("Building null.");
                    return;
                }

                if (building.buildingName == "Headquarters1")
                {
                    headquarters1.level = 1;
                    //UpdateBuildingCounts();
                }
                if (building.buildingName == "Headquarters2")
                {
                    headquarters2.level = 2;
                    //UpdateBuildingCounts();
                }
                if (building.buildingName == "Headquarters3")
                {
                    headquarters3.level = 3;
                    //UpdateBuildingCounts();
                }
                if (building.buildingName == "Architecture")
                {
                    architecture.level = building.level;
                }
                if (building.buildingName == "Walls")
                {
                    walls.level = building.level;
                }
                if (building.buildingName == "Tower")
                {
                    tower.level = building.level;
                }

                UpgradeManager.Instance.StartUpgrade(building);
            }
            else
            {
                MapUI.SetActive(true);
                TopUI.SetActive(false);
                //Debug.Log($"Error: {building.buildingName} cannot be upgraded.  " + building.level);
            }
        }
        else
        {
            if (building == null)
            {
                return;
            }
            else
            {
                if (building.steelCost == 0) { NotEnoughtText.text = $"Not enough resources to upgrade. {Mathf.Abs(steel - building.steelCost)}"; }
                NotEnoughtText.text = $"{building.steelCost - steel}";
                notEnoughUI.SetActive(true);
                TopUI.SetActive(false);
            }
        }
    }
    //private void UpdateBuildingCounts()
    //{
    //    if (headquarters2.level == 2)
    //    {
    //        Debug.Log($"Level 2 Headquarters: +3 Storage, +2 Architecture");
    //    }

    //}
    public Building GetBuildingByName(string name)
    {
        var buildings = FindObjectsOfType<Building>();
        foreach (var building in buildings)
        {
            if (building.buildingName.Equals(name))
            {
                return building;
            }
        }
        return null;
    }

    public float GetUpgradeTimeForLevel(Building building)
    {
        switch (building.buildingName)
        {
            case "Headquarters1":
                switch (building.level)
                {
                    case 1:
                        return 60;
                    case 2:
                        return 120;
                    case 3:
                        return 180;
                    default:
                        return 0;
                }
            case "Headquarters2":
                switch (building.level)
                {
                    case 1:
                        return 60;
                    case 2:
                        return 120;
                    case 3:
                        return 180;
                    default:
                        return 0;
                }
            case "Headquarters3":
                switch (building.level)
                {
                    case 1:
                        return 60;
                    case 2:
                        return 120;
                    case 3:
                        return 180;
                    default:
                        return 0;
                }
            case "Storage":
                switch (building.level)
                {
                    case 0:
                        return 30;
                    case 1:
                        return 60;
                    case 2:
                        ismakeBomb = true;
                        return 120;
                    case 3:
                        return 180;
                    default:
                        return 0;
                }
            case "Architecture":
                switch (building.level)
                {
                    case 1:
                        return 60;
                    case 2:
                        return 120;
                    case 3:
                        return 180;
                    default:
                        return 0;
                }
            case "Workshop":
                switch (building.level)
                {
                    case 0:
                        return 30;
                    case 1:
                        return 60;
                    case 2:
                        return 120;
                    case 3:
                        return 180;
                    default:
                        return 0;
                }
            case "Tower":
                switch (building.level)
                {
                    case 0:
                        return 30;
                    case 1:
                        return 60;
                    case 2:
                        return 120;
                    case 3:
                        return 180;
                    default:
                        return 0;
                }
            case "Walls":
                switch (building.level)
                {
                    case 1:
                        return 1;
                    case 2:
                        return 2;
                    case 3:
                        return 3;
                    default:
                        return 0;
                }


            default:
                return 0;
        }
    }



    public void CheckAllBuildings()
    {
        CheckHeadquarters3Level();
        CheckArchitectureLevel();
        CheckStorageLevel();
        CheckWallsLevel();
        CheckTowerLevel();
    }

    public void Test()
    {
        Debug.Log("birinchi wave");
        SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.FiftyKills);
    }

    public void CheckHeadquarters3Level()
    {
        if (headquarters3 != null && headquarters3.level == 3 && !loggedBuildings["Headquarters3"])
        {
            Debug.Log("Headquarters3 has reached level 3!");
            SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.MAXCommandCenter);
            loggedBuildings["Headquarters3"] = true;
        }
    }

    public void CheckArchitectureLevel()
    {
        if (architecture != null && architecture.level == 3 && !loggedBuildings["Architecture"])
        {
            Debug.Log("Architecture has reached level 3!");
            SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.MAXArchitecture);
            loggedBuildings["Architecture"] = true;
        }
    }

    public void CheckStorageLevel()
    {
        if (storage != null && storage.level == 3 && !loggedBuildings["Storage"])
        {
            Debug.Log("Storage has reached level 3!");
            SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.MAXPayloadStation);
            loggedBuildings["Storage"] = true;
        }
    }

    public void CheckWallsLevel()
    {
        if (walls != null && walls.level == 3 && !loggedBuildings["Walls"])
        {
            Debug.Log("Walls has reached level 3!");
            SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.MAXWalls);
            loggedBuildings["Walls"] = true;
        }
    }

    public void CheckTowerLevel()
    {
        if (tower != null && tower.level == 3 && !loggedBuildings["Tower"])
        {
            Debug.Log("Tower has reached level 3!");
            SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.MAXTowers);
            loggedBuildings["Tower"] = true;
        }
    }

}
