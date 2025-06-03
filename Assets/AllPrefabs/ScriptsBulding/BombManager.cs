using UnityEngine;
using System.Collections.Generic;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;

public class BombManager : MonoBehaviour
{
    #region Singleton
    public static BombManager Instance;

    private void Awake()
    {
        Instance = this;
    }
    #endregion

    #region Public Variables
    [Header("UI References")]
    public TMP_Text notEnoughResourcesText;

    [Header("Prefab References")]
    public GameObject bombPrefab;
    public GameObject minePrefab;

    [Header("Position References")]
    public GameObject[] bombPositions;
    public GameObject[] minePositions;
    #endregion

    #region Private Variables
    private List<GameObject> activeBombs = new List<GameObject>();
    private List<GameObject> activeMines = new List<GameObject>();
    private List<Vector3> initialBombPositions = new List<Vector3>();
    private List<Vector3> initialMinePositions = new List<Vector3>();

    private const int BOMB_PRICE = 150;
    private const string CURRENT_LEVEL_KEY = "CurrentBombLevel38";

    [SerializeField]
    private int currentLevel = 0;
    #endregion

    #region Unity Lifecycle Methods
    private void Start()
    {
        InitializePositions();
        LoadCurrentLevel();
        InitializeBombsAndMines();
    }
    #endregion

    #region Level Management
    /// <summary>
    /// Sets the game to a specific level and initializes corresponding explosives
    /// </summary>
    /// <param name="level">Target level to set</param>
    public void NextLevel(int level)
    {
        currentLevel = level;
        SaveCurrentLevel();
        InitializeBombsAndMines();
        //Debug.Log($"Level changed to: {currentLevel}");
    }


    private void LoadCurrentLevel()
    {
        currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 0);
    }



    private void SaveCurrentLevel()
    {
        PlayerPrefs.DeleteAll(); // Barcha avvalgi ma'lumotlarni o'chiradi
        PlayerPrefs.SetInt(CURRENT_LEVEL_KEY, currentLevel);
        PlayerPrefs.Save();
    }

    #endregion

    #region Initialization Methods

    private void InitializePositions()
    {
        SetupInitialPositions(bombPositions, initialBombPositions);
        SetupInitialPositions(minePositions, initialMinePositions);
    }


    private void SetupInitialPositions(GameObject[] positions, List<Vector3> initialPositions)
    {
        foreach (var position in positions)
        {
            if (position != null)
            {
                initialPositions.Add(position.transform.position);
            }
        }
    }


    public void InitializeBombsAndMines()
    {
        ClearAllExplosives();

        switch (currentLevel)
        {
            case 0:
                // Level 0 - No explosives
                break;
            case 1:
                // Level 1 - Basic bombs only
                InstantiateExplosives(bombPrefab, initialBombPositions, activeBombs, 5);
                break;
            case 2:
                // Level 2 - Bombs and mines
                InstantiateExplosives(bombPrefab, initialBombPositions, activeBombs, 5);
                InstantiateExplosives(minePrefab, initialMinePositions, activeMines, 5);
                break;
            case 3:
                // Level 3 - Maximum difficulty
                InstantiateExplosives(bombPrefab, initialBombPositions, activeBombs, 10);
                InstantiateExplosives(minePrefab, initialMinePositions, activeMines, 10);
                break;
            default:
                //// Higher levels - Maximum difficulty
                //InstantiateExplosives(bombPrefab, initialBombPositions, activeBombs, 10);
                //InstantiateExplosives(minePrefab, initialMinePositions, activeMines, 10);
                break;
        }
    }
    #endregion

    #region Explosive Management

    private void InstantiateExplosives(GameObject prefab, List<Vector3> positions, List<GameObject> explosives, int count)
    {
        for (int i = 0; i < count && i < positions.Count; i++)
        {
            GameObject explosive = Instantiate(prefab, positions[i], Quaternion.identity);
            explosives.Add(explosive);
        }
    }


    private void ClearAllExplosives()
    {
        ClearExplosivesList(activeBombs);
        ClearExplosivesList(activeMines);
    }

    private void ClearExplosivesList(List<GameObject> explosives)
    {
        foreach (var explosive in explosives)
        {
            if (explosive != null)
            {
                Destroy(explosive);
            }
        }
        explosives.Clear();
    }
    #endregion

    #region Resource Management

    public void RestoreMissingExplosives()
    {
        int missingBombs = CountMissingExplosives(activeBombs);
        int missingMines = CountMissingExplosives(activeMines);
        int totalRestorationCost = (missingBombs + missingMines) * BOMB_PRICE;

        if (GameManager.Instance.steel >= totalRestorationCost)
        {
            GameManager.Instance.steel -= totalRestorationCost;
            RestoreExplosives(bombPrefab, initialBombPositions, activeBombs, missingBombs);
            RestoreExplosives(minePrefab, initialMinePositions, activeMines, missingMines);
            //Debug.Log($"Restored {missingBombs} bombs and {missingMines} mines");
        }
        else
        {
            int missingSteel = totalRestorationCost - GameManager.Instance.steel;
            string message = $"{missingSteel}";
            ForUi.UInstance.NotEnough.SetActive(true);
            ForUi.UInstance.TopMenuePanel.SetActive(false);

            if (notEnoughResourcesText != null)
            {
                notEnoughResourcesText.text = message;
            }
            //Debug.LogWarning(message);
        }
    }

    private int CountMissingExplosives(List<GameObject> explosives)
    {
        return explosives.Count(explosive => explosive == null);
    }


    private void RestoreExplosives(GameObject prefab, List<Vector3> positions, List<GameObject> explosives, int count)
    {
        for (int i = 0; i < explosives.Count && count > 0; i++)
        {
            if (explosives[i] == null)
            {
                explosives[i] = Instantiate(prefab, positions[i], Quaternion.identity);
                count--;
            }
        }
    }
    #endregion

    #region Public Getters
    public int GetCurrentLevel()
    {
        return currentLevel;
    }
    #endregion
}