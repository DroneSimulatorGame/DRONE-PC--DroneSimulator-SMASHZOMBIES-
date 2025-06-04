using UnityEngine;

public class DroneUpgrade : MonoBehaviour
{
    #region Variables

    [Header("Components")]
    [SerializeField] private NewGun gun;
    [SerializeField] private GameObject LRpushka; // Nade launcher, now active from level 1
    [SerializeField] private GameObject UIButton;

    [Header("Level Settings")]
    [SerializeField] private int maxLevel = 3;
    [SerializeField] private int upgradeCostLevel2 = 1000;
    [SerializeField] private int upgradeCostLevel3 = 2000;

    private const string DRONE_LEVEL_KEY = "DroneLevel5";
    private int currentLevel = 1;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Validate references
        if (gun == null || LRpushka == null || UIButton == null)
        {
            Debug.LogError("Missing references in DroneUpgrade!", this);
            return;
        }

        LoadSavedLevel();
        // Ensure Nade launcher is active from the start
        LRpushka.SetActive(true);
    }

    private void Start()
    {
        ApplyLevelStats(currentLevel);
    }

    #endregion

    #region Level Management

    public void UpgradeLevel()
    {
        int requiredSteel = GetUpgradeCost();

        if (GameManager.Instance.steel >= requiredSteel)
        {
            if (currentLevel < maxLevel)
            {
                GameManager.Instance.steel -= requiredSteel;
                currentLevel++;
                SaveCurrentLevel();
                ApplyLevelStats(currentLevel);
                Debug.Log("DroneUpgraded!!!!!!!!!!");
            }
        }
        else
        {
            Debug.Log("Mablag' yetarli emas!");
        }
    }

    public int GetUpgradeCost()
    {
        return currentLevel switch
        {
            1 => upgradeCostLevel2,
            2 => upgradeCostLevel3,
            _ => 10000
        };
    }

    private void ApplyLevelStats(int level)
    {
        switch (level)
        {
            case 1:
                SetDroneStats(20, 150, 10f, 0.5f, false);
                break;
            case 2:
                SetDroneStats(30, 200, 15f, 0.7f, false);
                break;
            case 3:
                SetDroneStats(40, 250, 20f, 1f, true);
                break;
            default:
                Debug.LogWarning($"Noto'g'ri level: {level}");
                break;
        }
    }

    private void SetDroneStats(int damage, int speed, float holdTime, float cooldown, bool enableUIButton)
    {
        if (gun == null || LRpushka == null || UIButton == null)
        {
            Debug.LogError("Missing references in SetDroneStats!", this);
            return;
        }

        gun.damageBullet = damage;
        gun.bulletSpeed = speed;
        gun.maxHeatTime = holdTime;
        gun.cooldownSpeed = cooldown;
        // LRpushka is always active, no longer tied to enableUIButton
        LRpushka.SetActive(true);
        // UIButton is only active at level 3
        UIButton.SetActive(enableUIButton);
    }

    #endregion

    #region Save/Load System

    private void SaveCurrentLevel()
    {
        PlayerPrefs.SetInt(DRONE_LEVEL_KEY, currentLevel);
        PlayerPrefs.Save();
    }

    private void LoadSavedLevel()
    {
        currentLevel = PlayerPrefs.GetInt(DRONE_LEVEL_KEY, 1);
        currentLevel = Mathf.Clamp(currentLevel, 1, maxLevel);
    }

    #endregion

    #region Public Getters

    public int GetCurrentLevel() => currentLevel;

    public int GetMaxLevel() => maxLevel;

    public int GetNextLevelCost() => GetUpgradeCost();

    #endregion
}