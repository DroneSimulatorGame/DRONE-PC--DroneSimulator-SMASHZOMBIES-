using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ForUi : MonoBehaviour
{
    #region
    public static ForUi UInstance;
    public GameObject TopMenuePanel;

    public GameObject settings;
    //public GameObject Buildingpanel;
    public GameObject PAUSEPANEL;
    public GameObject PLAYPANEL;
    public GameObject STOREPANEL;
    public GameObject INPROGESS;
    public GameObject FINISHALL;
    public GameObject VICTORYPANEL;
    public GameObject DEFEATPANEL;
    public GameObject MSUI; // <-- Added Missile System UI

    public GameObject NotEnough;
    public GameObject Repair;
    public GameObject ReturnCheck;
    public GameObject NoInternet;
    public GameObject InstructionMap;

    public TextMeshProUGUI notEnoughText;
    public GameObject quitgame;



    private int skladCount = 0;
    private int towerCount = 1;

    public GameObject sklad;
    public GameObject ustaxona;
    public GameObject tower3;
    public GameObject tower4;
    public NewGun gun;
    public DroneUpgrade droneUpgrade;

    public int priceUstaxona = 1200;
    public int priceTower = 1400;
    public int priceSklad = 1000;

    private BombManager bombManager;
    public AudioContainer audiomanager;
    public float touchHoldTime = 0f;
    public float touchThreshold = 1f;
    private bool isTouchingBuilding = false;
    private Building touchedBuilding;
    private bool uiActivated = false;
    Button upgradeButton;

    [Header("Tower UI Settings")]
    public GameObject tower;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI fireRateText;
    public TextMeshProUGUI detectRangeText;
    public TextMeshProUGUI towerPriceText;
    [Header("Command Center UI Settings")]
    public GameObject COMAND_CENTER;
    public TextMeshProUGUI CommadPriceText;
    [Header("Architecture UI Settings")]
    public GameObject ARCHITECTURE;
    public TextMeshProUGUI priceUstaxonaText;
    public TextMeshProUGUI priceTowerText;
    public TextMeshProUGUI priceSkladText;
    public TextMeshProUGUI architecturePriceText;
    [Header("Workshop UI Settings")]
    public GameObject WORKSHOP;
    public TextMeshProUGUI droneDamageText;
    public TextMeshProUGUI droneFireRateText;
    public TextMeshProUGUI droneSpeedText;
    public TextMeshProUGUI dronePriceText;
    [Header("Payload Station UI Settings")]
    public GameObject PAYLOADSTATION;
    public TextMeshProUGUI skladPriceText;
    [Header("Wall UI Settings")]
    public GameObject WALL;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI wallPriceText;

    #endregion




    public void Awake()
    {

        // BombManager ni topish
        bombManager = FindObjectOfType<BombManager>();
        UInstance = this;

    }


    public int CountTowers()
    {
        // Barcha Building obyektlarini olish
        Building[] allBuildings = FindObjectsOfType<Building>();

        // Tower nomiga ega bo'lgan obyektlarni sanash
        int towerCount = 0;
        foreach (Building building in allBuildings)
        {
            if (building.buildingName.Contains("Tower"))
            {
                towerCount++;
            }
        }

        // Towerlar sonini qaytarish
        return towerCount;
    }

    public int CountUstaxona()
    {
        // Barcha Building obyektlarini olish
        Building[] allBuildings = FindObjectsOfType<Building>();

        // Tower nomiga ega bo'lgan obyektlarni sanash
        int workshopCount = 0;
        foreach (Building building in allBuildings)
        {
            if (building.buildingName.Contains("Workshop"))
            {
                workshopCount++;
            }
        }

        // Towerlar sonini qaytarish
        return workshopCount;
    }

    public int CountSklad()
    {
        // Barcha Building obyektlarini olish
        Building[] allBuildings = FindObjectsOfType<Building>();

        // Tower nomiga ega bo'lgan obyektlarni sanash
        int skladCount = 0;
        foreach (Building building in allBuildings)
        {
            if (building.buildingName.Contains("Storage"))
            {
                skladCount++;
            }
        }

        // Towerlar sonini qaytarish
        return skladCount;
    }

    public void ActivateSklad()
    {
        if (sklad == null)
        {
            return;
        }
        if (GameManager.Instance.architecture.level > 1 && CountSklad() < 1)
        {
            Building skladBuilding = sklad.GetComponent<Building>();
            if (GameManager.Instance.steel >= priceSklad)
            {
                skladBuilding.isBuilding = true;
                UpgradeManager.Instance.StartUpgrade(skladBuilding);
                skladCount++;
                GameManager.Instance.steel -= priceSklad;
                sklad.SetActive(true);
                BombManager.Instance.NextLevel(1);
                priceSklad = 0;
            }
            else
            {
                notEnoughText.text = $"{priceSklad - GameManager.Instance.steel}";
                NotEnough.SetActive(true);
                TopMenuePanel.SetActive(false);
            }
        }
        else
        {

            //Debug.Log("skladni boshqa qurib bo'lmaydi");
        }
    }

    public void ActivateUstaxona()
    {
        if (ustaxona == null)
        {
            return;
        }
        if (GameManager.Instance.architecture.level > 1 && CountUstaxona() < 1)
        {
            Building ustaxonaBuilding = ustaxona.GetComponent<Building>();
            if (GameManager.Instance.steel >= priceUstaxona)
            {
                ustaxonaBuilding.isBuilding = true;
                UpgradeManager.Instance.StartUpgrade(ustaxonaBuilding);
                GameManager.Instance.steel -= priceUstaxona;
                ustaxona.SetActive(true);
                priceUstaxona = 0;
            }
            else
            {
                notEnoughText.text = $"{priceUstaxona - GameManager.Instance.steel}";
                NotEnough.SetActive(true);
                TopMenuePanel.SetActive(false);
            }
        }
        else
        {
            //Debug.Log("boshqa qurib bo'lmaydi yoki mablag' yetarli emas" + CountUstaxona());
        }
    }
    public void ActivateTower3()
    {
        //if(tower3 == null || tower4 == null)
        //{
        //    return;
        //}
        if (GameManager.Instance.architecture.level > 1 && 5 > CountTowers())
        {

            if (CountTowers() > 1 && CountTowers() < 3)
            {
                Building tower3Building = tower3.GetComponent<Building>();
                if (GameManager.Instance.steel >= priceTower)
                {
                    tower3Building.isBuilding = true;
                    UpgradeManager.Instance.StartUpgrade(tower3Building);
                    GameManager.Instance.steel -= priceTower;
                    tower3.SetActive(true);
                }
                else
                {
                    notEnoughText.text = $"{priceTower - GameManager.Instance.steel}";
                    NotEnough.SetActive(true);
                    TopMenuePanel.SetActive(false);
                }
            }
            else if (CountTowers() > 2 && CountTowers() < 4)// && UpgradeManager.Instance.CanUpgrade())
            {
                Building tower4Building = tower4.GetComponent<Building>();
                if (GameManager.Instance.steel >= priceTower)
                {
                    tower4Building.isBuilding = true;
                    UpgradeManager.Instance.StartUpgrade(tower4Building);
                    GameManager.Instance.steel -= priceTower;
                    tower4.SetActive(true);
                    priceTower = 0;
                }
                else
                {
                    notEnoughText.text = $"{priceTower - GameManager.Instance.steel}";
                    NotEnough.SetActive(true);
                    TopMenuePanel.SetActive(false);
                }
            }
            else
            {
                //Debug.Log("Nooooooooooooo" + CountTowers());
            }
        }
        else
        {
            //Debug.Log("boshqa qurib bo'lmaydi" + CountTowers() + "  " + GameManager.Instance.headquarters1.level + "  " + GameManager.Instance.steel);
        }
    }


    public void OnBuildingTouched(Building building)
    {
        if (IsUIActive()) return;

        touchedBuilding = building;
        isTouchingBuilding = true;
        touchHoldTime = 0f;
        uiActivated = false;
    }

    public void OnBuildingTouchReleased()
    {
        isTouchingBuilding = false;
        touchHoldTime = 0f;
        uiActivated = false;
    }

    public void ActivateBuildingUI(Building building)
    {

        if (IsUIActive()) return; // Agar UI faol bo'lsa, yangi UI ochilmasin

        DeactivateBuildingUI();

        if (UpgradeManager.Instance.IsBuildingUpgrading(building.buildingName, building.transform.position, building.transform.rotation.eulerAngles))
        {
            INPROGESS.SetActive(true);
            return;
        }
        else
        {
            switch (building.buildingName)
            {
                case "Architecture":
                    priceSkladText.text = $"{priceSklad}";
                    priceTowerText.text = $"{priceTower}";
                    priceUstaxonaText.text = $"{priceUstaxona}";
                    architecturePriceText.text = $"{building.gameObject.GetComponent<Architecture>().steelCost}";
                    ARCHITECTURE.SetActive(true);
                    audiomanager.architec_audio.Play();
                    upgradeButton = ARCHITECTURE.transform.Find("stats holder/upgrade")?.GetComponent<Button>();
                    upgradeButton.onClick.RemoveAllListeners();
                    upgradeButton.onClick.AddListener(() => GameManager.Instance.UpgradeBuilding(building));
                    break;
                case "Headquarters1":
                    CommadPriceText.text = $"{building.gameObject.GetComponent<Headquarters1>().steelCost}";
                    COMAND_CENTER.SetActive(true);
                    audiomanager.comman_audio.Play();
                    upgradeButton = COMAND_CENTER.transform.Find("stats holder/UPGRADE")?.GetComponent<Button>();
                    upgradeButton.onClick.RemoveAllListeners();
                    upgradeButton.onClick.AddListener(() => GameManager.Instance.UpgradeBuilding(building));
                    break;
                case "Headquarters2":
                    CommadPriceText.text = $"{building.gameObject.GetComponent<Headquarters2>().steelCost}";
                    COMAND_CENTER.SetActive(true);
                    audiomanager.comman_audio.Play();
                    upgradeButton = COMAND_CENTER.transform.Find("stats holder/UPGRADE")?.GetComponent<Button>();
                    upgradeButton.onClick.RemoveAllListeners();
                    upgradeButton.onClick.AddListener(() => GameManager.Instance.UpgradeBuilding(building));
                    break;
                case "Headquarters3":
                    CommadPriceText.text = $"{building.gameObject.GetComponent<Headquarters3>().steelCost}";
                    COMAND_CENTER.SetActive(true);
                    audiomanager.comman_audio.Play();
                    upgradeButton = COMAND_CENTER.transform.Find("stats holder/UPGRADE")?.GetComponent<Button>();
                    upgradeButton.onClick.RemoveAllListeners();
                    upgradeButton.onClick.AddListener(() => GameManager.Instance.UpgradeBuilding(building));
                    break;

                case "Storage":
                    skladPriceText.text = $"{building.gameObject.GetComponent<Storage>().steelCost}";
                    PAYLOADSTATION.SetActive(true);
                    audiomanager.payload_audio.Play();
                    upgradeButton = PAYLOADSTATION.transform.Find("stats holder/UPGRADE")?.GetComponent<Button>();
                    upgradeButton.onClick.RemoveAllListeners();
                    upgradeButton.onClick.AddListener(() => GameManager.Instance.UpgradeBuilding(building));
                    break;
                case "Workshop":
                    droneDamageText.text = $"{gun.damageBullet}";
                    droneFireRateText.text = $"{gun.bulletSpeed}";
                    droneSpeedText.text = $"{gun.maxHeatTime}";
                    dronePriceText.text = $"{droneUpgrade.GetUpgradeCost()}";
                    WORKSHOP.SetActive(true);
                    audiomanager.workshop_audio.Play();
                    //upgradeButton = WORKSHOP.transform.Find("stats holder/UPGRADE")?.GetComponent<Button>();
                    //upgradeButton.onClick.RemoveAllListeners();
                    //upgradeButton.onClick.AddListener(() => GameManager.Instance.UpgradeBuilding(building));
                    break;
                case "Walls":
                    wallPriceText.text = $"{building.gameObject.GetComponent<Walls>().steelCost}";
                    healthText.text = $"{building.gameObject.GetComponent<EnimyDetect>().devorHP}";
                    WALL.SetActive(true);
                    audiomanager.wall_audio.Play();
                    upgradeButton = WALL.transform.Find("stats holder/UPGRADE")?.GetComponent<Button>();
                    upgradeButton.onClick.RemoveAllListeners();
                    upgradeButton.onClick.AddListener(() => GameManager.Instance.UpgradeBuilding(building));
                    break;
                case "Tower":
                    towerPriceText.text = $"{building.gameObject.GetComponent<Tower>().steelCost}";
                    damageText.text = $"{building.gameObject.GetComponent<DefenseTower>().damage}";
                    fireRateText.text = $"{building.gameObject.GetComponent<DefenseTower>().fireRate}";
                    detectRangeText.text = $"{building.gameObject.GetComponent<DefenseTower>().range}";
                    tower.SetActive(true);
                    audiomanager.tower_audio.Play();
                    upgradeButton = tower.transform.Find("stats holder/UPGRADE")?.GetComponent<Button>();
                    upgradeButton.onClick.RemoveAllListeners();
                    upgradeButton.onClick.AddListener(() => GameManager.Instance.UpgradeBuilding(building));
                    break;
                case "Missile System":
                    MSUI.SetActive(true);
                    audiomanager.missile_audio.Play();
                    break;

                default:
                    //Debug.LogWarning("No UI found for building: " + building.buildingName);
                    break;
            }
        }
    }

    public void DeactivateBuildingUI()
    {
        TopMenuePanel.SetActive(false);
        tower.SetActive(false);
        settings.SetActive(false);
        COMAND_CENTER.SetActive(false);
        ARCHITECTURE.SetActive(false);
        WORKSHOP.SetActive(false);
        PAYLOADSTATION.SetActive(false);
        WALL.SetActive(false);
        //Buildingpanel.SetActive(false);
        PAUSEPANEL.SetActive(false);
        PLAYPANEL.SetActive(false);
        STOREPANEL.SetActive(false);
        INPROGESS.SetActive(false);
        FINISHALL.SetActive(false);
        VICTORYPANEL.SetActive(false);
        DEFEATPANEL.SetActive(false);
        MSUI.SetActive(false);
        NoInternet.SetActive(false);
        NotEnough.SetActive(false);
        Repair.SetActive(false);
        ReturnCheck.SetActive(false);
        InstructionMap.SetActive(false);
        quitgame.SetActive(false);
    }


    // UI faol yoki faol emasligini tekshirish uchun method
    public bool IsUIActive()
    {
        return tower.activeSelf || settings.activeSelf ||
               COMAND_CENTER.activeSelf || ARCHITECTURE.activeSelf || WORKSHOP.activeSelf ||
               PAYLOADSTATION.activeSelf || WALL.activeSelf ||
               PAUSEPANEL.activeSelf || PLAYPANEL.activeSelf || STOREPANEL.activeSelf ||
               INPROGESS.activeSelf || FINISHALL.activeSelf || VICTORYPANEL.activeSelf ||
               DEFEATPANEL.activeSelf || MSUI.activeSelf || NotEnough.activeSelf || 
               NoInternet.activeSelf || ReturnCheck.activeSelf || Repair.activeSelf || InstructionMap.activeSelf || quitgame.activeSelf;
    }

}
