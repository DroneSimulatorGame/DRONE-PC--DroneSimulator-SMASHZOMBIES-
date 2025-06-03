using UnityEngine;
using System;

public abstract class Building : MonoBehaviour
{
    public static Building Instance;
    private static Building currentlySelectedBuilding;

    // Indicator references
    private GameObject selectedIndicator;
    private GameObject selectedIndicator2;
    private GameObject selectedIndicator3;

    // Building properties
    public string buildingName;
    public int level;
    public int goldCost;
    public int steelCost;
    public int index;
    public string info;
    public string objectName;
    public string buildingId;
    public bool isBuilding;

    public Building(string buildingName, int goldCost, int stoneCost, int index, string info, bool isBuilding)
    {
        this.buildingName = buildingName;
        this.level = 1;
        this.goldCost = goldCost;
        this.steelCost = stoneCost;
        this.index = index;
        this.info = info;
        this.isBuilding = isBuilding;
    }

    void Awake()
    {
        if (string.IsNullOrEmpty(buildingId))
        {
            buildingId = System.Guid.NewGuid().ToString();
        }
    }

    void Start()
    {
        InitializeIndicators();
        DeactivateAllIndicators();
    }

    private void InitializeIndicators()
    {
        // Try to find each indicator, but don't throw error if missing
        Transform selectedTransform = transform.Find("selected");
        Transform circleTransform = transform.Find("circle");
        Transform circle2Transform = transform.Find("circle 2");

        selectedIndicator = selectedTransform != null ? selectedTransform.gameObject : null;
        selectedIndicator2 = circleTransform != null ? circleTransform.gameObject : null;
        selectedIndicator3 = circle2Transform != null ? circle2Transform.gameObject : null;
    }

    private void DeactivateAllIndicators()
    {
        if (selectedIndicator != null) selectedIndicator.SetActive(false);
        if (selectedIndicator2 != null) selectedIndicator2.SetActive(false);
        if (selectedIndicator3 != null) selectedIndicator3.SetActive(false);
    }

    private void ActivateAllIndicators()
    {
        if (selectedIndicator != null) selectedIndicator.SetActive(true);
        if (selectedIndicator2 != null) selectedIndicator2.SetActive(true);
        if (selectedIndicator3 != null) selectedIndicator3.SetActive(true);
    }

    public void Upgrade()
    {
        if (level < 3)
        {
            if (!isBuilding) { level++; }
            UpdateCosts();
            UpgradePrefab();
            UpdatePrefab();
            UpdateHeadquartersLevel();
            Debug.Log($"{buildingName} is upgraded to level {level}");
        }
    }

    public virtual void UpdatePrefab() { }

    public abstract void UpgradePrefab();

    public string ReplacePrefab(GameObject newPrefab)
    {
        objectName = gameObject.name;

        GameObject newBuilding = Instantiate(newPrefab, transform.position, transform.rotation);
        newBuilding.transform.SetParent(transform.parent);

        Building building = newBuilding.GetComponent<Building>();
        if (building != null)
        {
            building.level = this.level;
            building.goldCost = this.goldCost;
            building.steelCost = this.steelCost;
            building.index = this.index;
            building.info = this.info;
        }

        Destroy(gameObject);
        return objectName;
    }

    private void UpdateCosts()
    {
        switch (buildingName)
        {
            case "Headquarters1" :
                UpdateHeadquartersCosts();
                break;
            case "Headquarters2":
                UpdateHeadquartersCosts();
                break;
            case "Headquarters3":
                UpdateHeadquartersCosts();
                break;
            case "Architecture":
                UpdateArchitectureCosts();
                break;
            case "Storage":
                UpdateStorageCosts();
                break;
            case "Tower":
                UpdateTowerCosts();
                break;
            case "Walls":
                UpdateWallsCosts();
                break;
        }
    }

    private void UpdateHeadquartersCosts()
    {
        if (level == 2)
        {
            goldCost = 0;
            steelCost = 10000;
        }
        else if (level == 3)
        {
            goldCost = steelCost = 0;
            info = "Maximum";
        }
    }

    private void UpdateArchitectureCosts()
    {
        if (level == 2)
        {
            goldCost = 0;
            steelCost = 8000;
        }
        else if (level == 3)
        {
            goldCost = steelCost = 0;
            info = "Maximum";
        }
    }

    private void UpdateStorageCosts()
    {
        if (level == 2)
        {
            goldCost = 0;
            steelCost = 5000;
        }
        else if (level == 3)
        {
            goldCost = steelCost = 0;
            info = "Maximum";
        }
    }

    private void UpdateTowerCosts()
    {
        if (level == 2)
        {
            goldCost = 0;
            steelCost = 7000;
        }
        else if (level == 3)
        {
            goldCost = 0;
            steelCost = 0;
            info = "Maximum";
        }
    }

    private void UpdateWallsCosts()
    {
        if (level == 2)
        {
            goldCost = 0;
            steelCost = 1500;
        }
        else if (level == 3)
        {
            goldCost = 0;
            steelCost = 0;
            info = "Maximum";
        }
    }

    private void UpdateHeadquartersLevel()
    {
        switch (buildingName)
        {
            case "Headquarters1":
                GameManager.Instance.headquarters1.level = 1;
                break;
            case "Headquarters2":
                GameManager.Instance.headquarters2.level = 2;
                break;
            case "Headquarters3":
                GameManager.Instance.headquarters3.level = 3;
                break;
            case "Architecture":
                GameManager.Instance.architecture.level = level;
                break;
            case "Storage":
                if (GameManager.Instance.storage != null)
                {
                    GameManager.Instance.storage.level = level;
                }
                else
                {
                    Debug.LogWarning("Storage reference is missing in GameManager");
                }
                break;
            case "Walls":
                GameManager.Instance.walls.level = level;
                break;
            case "Tower":
                GameManager.Instance.tower.level = level;
                break;
        }
    }

    private void SelectBuilding()
    {
        if (currentlySelectedBuilding == null)
        {
            currentlySelectedBuilding = this;
            ActivateAllIndicators();
            Debug.Log($"Selected building: {this.name}");
            Invoke("ClearSelectedBuilding", 4f);
        }
        else
        {
            if (currentlySelectedBuilding == this)
            {
                DeactivateAllIndicators();
                HandleBuildingInteraction();
                currentlySelectedBuilding = null;
            }
            else
            {
                currentlySelectedBuilding.DeactivateAllIndicators();
                currentlySelectedBuilding = this;
                ActivateAllIndicators();
                Debug.Log($"New selected building: {this.name}");
                Invoke("ClearSelectedBuilding", 4f);
            }
        }
    }

    private void ClearSelectedBuilding()
    {
        currentlySelectedBuilding = null;
        DeactivateAllIndicators();
        Debug.Log("Building selection cleared.");
    }


    private void HandleBuildingInteraction()
    {
        ForUi.UInstance.OnBuildingTouched(this);
        ForUi.UInstance.ActivateBuildingUI(this);
        UpgradeManager.Instance.SelectBuilding(this);
        GameManager.Instance.CheckMissingBuildings();
    }

    void OnMouseDown()
    {
        if (!ForUi.UInstance.IsUIActive())
        {
            SelectBuilding();
        }
    }
    public void SetIndex(int buildingIndex)
    {
        index = buildingIndex;
    }
}