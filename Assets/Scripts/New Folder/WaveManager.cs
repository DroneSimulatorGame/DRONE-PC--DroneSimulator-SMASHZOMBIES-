using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Data;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [SerializeField] private WaveDataScript waveDataScript;
    [SerializeField] private List<Transform> spawnPoints;
    public List<int> drops = new List<int>();

    // Reference to the pooling system
    [SerializeField] private ZombiePool zombiePool;  // Reference to the zombie pool script

    private float timer = 6f;  // Timer for normal zombies
    private float timer1 = 7f;  // Timer for giant zombies
    private float timer2 = 7f;  // Timer for bomb zombies

    public RewardSystem RewardSystem;

    public GameObject winUi;  // Win UI
    public GameObject loseUI;  // Lose UI
    public GameObject NextWave;  // Next wave UI
    public TextMeshProUGUI waveNumber;
    public GameObject PausePanel;
    public TextMeshProUGUI zombieCounter;

    private string WaveName;

    private int normalZombieSoni;
    private int gigantZombieSoni;
    private int bombZombieSoni;
    public int totalZombies;


    public int normalZombieJoni;
    public int gigantZombieJoni;
    public int bombZombieJoni;

    public int zombieDropSoni;

    private int waveId;
    public int currentWaveId = 1;
    //private int restoreCost = 50;

    private bool noEnemyLeft = false;
    private bool noBuildingleft = false;
    private bool spawningStarted = false;


    private Transform currentSpawnPoint;

    private Coroutine waveSpawnerCoroutine;

    public GameObject headquarter;

    //public GoogleAdsManager GoogleAdsManager;

    public GameObject goldDropRewardDisplayObject;
    public GameObject steelDropRewardDisplayObject;
    public TextMeshProUGUI goldAmountText;
    public TextMeshProUGUI steelAmountText;

    public GameObject[] zombieDropPrefabs;
    public float dropYOffset = 1f;

    public int totalKilledZombies = 0;
    public List<int> zombieKillMilestones = new List<int> { 10, 25, 50, 100 };
    public List<int> waveMilestones = new List<int> { 1, 5, 10, 20 };

    private void Awake()
    {
        if (Instance == null) { Instance = this; }
        else { Destroy(gameObject); }

    }

    void Start()
    {

        // Ensure UIs are inactive at the start
        winUi.SetActive(false);
        loseUI.SetActive(false);
        currentWaveId = GameManager.Instance.waveLevel;
        NextWaveText();


    }

    private void Update()
    {


        GetCurrentWaveId();

        if (!NextWave.activeInHierarchy && !loseUI.activeInHierarchy && spawningStarted && headquarter == null)
        {
            RewardSystem.headquarterActive = false;
            //Debug.Log("headquarterActive =  " + RewardSystem.headquarterActive);

            LoseDisplay();
            //PausePanel.SetActive(false);
            NextWaveText();
            noBuildingleft = false; // Reset flag after processing
        }

        IsEnemyLeft();  // Check if enemies are left
        IsBuildingLeft();

        // Handle wave completion
        if (noEnemyLeft && spawningStarted && !NextWave.activeInHierarchy)
        {
            CheckWaveAchievements(currentWaveId);
            spawningStarted = false;  // Reset flag for the next wave
            GameManager.Instance.waveLevel += 1;
            currentWaveId = GameManager.Instance.waveLevel;  // Increment wave ID for the next wave
            WinDisplay();
            NextWaveText();  // Display "Next Wave" message
        }

        //// Handle loss condition
        //if (noBuildingleft && !NextWave.activeInHierarchy && !loseUI.activeInHierarchy && spawningStarted)
        //{
        //    LoseDisplay();
        //    NextWaveText();  // Display "Next Wave" message even after losing
        //    noBuildingleft = false;  // Reset the building flag
        //}
    }

    private float time = 2f;
    public void GetCurrentWaveId()
    {
        if (time <= 0)
        {
            currentWaveId = GameManager.Instance.waveLevel;
            time = 2f;
            NextWaveText();
        }
        else
        {
            time -= Time.deltaTime;
        }
    }





    // Call this method to start spawning the wave
    public void CallWaveSpawner()
    {
        LoadProgress();
        zombiePool.totalReleasedZombies = 0;
        FindHeadquarter();
        if (waveSpawnerCoroutine != null)
        {
            StopCoroutine(waveSpawnerCoroutine);  // Stop any active wave coroutine
        }
        RewardSystem.StartWave();  // Trigger rewards system
        waveSpawnerCoroutine = StartCoroutine(WaveSpawner());  // Start the wave spawning coroutine
    }

    // Coroutine to spawn zombies for the current wave
    private IEnumerator WaveSpawner()
    {
        //Debug.Log($"Current Wave is WAVE {currentWaveId}");
        waveId = waveDataScript.WavesData.FindIndex(data => data.ID == currentWaveId);
        WaveName = waveDataScript.WavesData[waveId].Name;
        normalZombieSoni = waveDataScript.WavesData[waveId].NormalZombieSoni;
        gigantZombieSoni = waveDataScript.WavesData[waveId].GigantZombieSoni;
        bombZombieSoni = waveDataScript.WavesData[waveId].BombZombieSoni;
        totalZombies = normalZombieSoni + gigantZombieSoni + bombZombieSoni;

        normalZombieJoni = waveDataScript.WavesData[waveId].NormalZombieJoni;
        gigantZombieJoni = waveDataScript.WavesData[waveId].GigantZombieJoni;
        bombZombieJoni = waveDataScript.WavesData[waveId].BombZombieJoni;

        zombieDropSoni = Random.Range(1, 5);

        zombiePool.GetZombieDrop();

        drops = GetUniqueRandomIndices(totalZombies, zombieDropSoni);

        ShowTotalActiveZombies();

        // Spawning loop - keeps spawning zombies until counts reach 0
        while (normalZombieSoni > 0 || gigantZombieSoni > 0 || bombZombieSoni > 0)
        {
            currentSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];  // Pick a random spawn point

            timer -= Time.deltaTime;
            timer1 -= Time.deltaTime;
            timer2 -= Time.deltaTime;

            // Spawn normal zombies using the object pool
            if (timer <= 0 && normalZombieSoni > 0)
            {
                //GameObject selectedZombiePrefab = normalZombiePrefabs[Random.Range(0, normalZombiePrefabs.Length)];
                GameObject zombie = zombiePool.GetNormalZombie(currentSpawnPoint.position);  // Retrieve zombie from pool

                if (zombie != null)
                {
                    // Customize zombie if needed (e.g., set type-specific properties)
                    zombie.transform.position = currentSpawnPoint.position;
                    zombie.transform.rotation = currentSpawnPoint.rotation;
                    zombie.SetActive(true);  // Activate zombie
                    zombie.GetComponent<DetectBullet>().health = normalZombieJoni;

                    normalZombieSoni--;
                }
                timer = Random.Range(0.1f, 0.2f);  // Reset normal zombie timer
            }

            // Spawn giant zombies using pooling
            if (timer1 <= 0 && gigantZombieSoni > 0)
            {
                GameObject giantZombie = zombiePool.GetGiantZombie(currentSpawnPoint.position);  // Retrieve from pool

                if (giantZombie != null)
                {
                    giantZombie.transform.position = currentSpawnPoint.position;
                    giantZombie.transform.rotation = currentSpawnPoint.rotation;
                    giantZombie.SetActive(true);  // Activate giant zombie
                    giantZombie.GetComponent<DetectBullet>().health = gigantZombieJoni;

                    gigantZombieSoni--;
                }
                timer1 = Random.Range(10f, 15f);  // Reset giant zombie timer
            }

            // Spawn bomb zombies using pooling
            if (timer2 <= 0 && bombZombieSoni > 0)
            {
                GameObject bombZombie = zombiePool.GetBomberZombie(currentSpawnPoint.position);  // Retrieve from pool

                if (bombZombie != null)
                {
                    bombZombie.transform.position = currentSpawnPoint.position;
                    bombZombie.transform.rotation = currentSpawnPoint.rotation;
                    bombZombie.SetActive(true);  // Activate bomb zombie
                    bombZombie.GetComponent<DetectBullet>().health = bombZombieJoni;

                    bombZombieSoni--;
                }
                timer2 = Random.Range(10f, 15f);  // Reset bomb zombie timer
            }

            yield return null;  // Wait until the next frame
        }

        spawningStarted = true;  // Mark that spawning has started
    }

    // Check if all enemies are gone from the scene
    private void IsEnemyLeft()
    {
        noEnemyLeft = totalZombies <= 0;

    }

    // Check if there are any buildings left in the scene
    private void IsBuildingLeft()
    {
        noBuildingleft = !headquarter;
    }

    // Display the win UI when the wave is completed
    private void WinDisplay()
    {
        //Debug.Log("Wave Completed - Displaying Win UI");

        winUi.SetActive(true);
        //PausePanel.SetActive(false);
        RewardSystem.EndWave();
        SaveProgress();
    }

    // Display the lose UI when all buildings are destroyed
    private void LoseDisplay()
    {
        //Debug.Log("All Buildings Destroyed - Displaying Lose UI");

        loseUI.SetActive(true);
        //PausePanel.SetActive(false);
        RewardSystem.EndWave();
        SaveProgress();
    }

    // Update the text for the next wave UI
    private void NextWaveText()
    {
        waveNumber.text = $"{currentWaveId}";
        NextWave.SetActive(true);
    }

    // Method to destroy all enemies in the scene
    public void DestroyAllEnemies()
    {
        //RestoreShtab();
        DestroyMuzzle();  // Destroy any effects or leftover objects
        ResetHealthBuilding();

        // Find all objects tagged as "Enemy" and return them to the pool
        zombiePool.ReleaseAllZombies();

        // Stop the wave spawning coroutine
        if (waveSpawnerCoroutine != null)
        {
            StopCoroutine(waveSpawnerCoroutine);
            waveSpawnerCoroutine = null;  // Clear the reference to the coroutine
        }

        winUi.SetActive(false);  // Hide the win UI
        loseUI.SetActive(false);  // Hide the lose UI

        spawningStarted = false;
        noEnemyLeft = false;
        noBuildingleft = false;  // Reset the state flags
    }

    // Destroy muzzle effects in the scene
    private void DestroyMuzzle()
    {
        GameObject[] effects = GameObject.FindGameObjectsWithTag("Effect");
        foreach (GameObject effect in effects)
        {
            Destroy(effect);  // Destroy each effect
        }
    }
    private void ResetHealthBuilding()
    {
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");
        GameObject[] walls = GameObject.FindGameObjectsWithTag("Target");
        foreach (GameObject building in buildings)
        {
            building.GetComponent<Building1>().ResetHealth();
        }
        foreach (GameObject wall in walls)
        {
            wall.GetComponent<EnimyDetect>().ResetWallHealth();
        }
    }

    public void FindHeadquarter()
    {
        if (headquarter != null) return;  // Skip if already found

        GameObject[] buildings = GameObject.FindGameObjectsWithTag("Building");

        foreach (GameObject building in buildings)
        {
            Building buildingScript = building.GetComponent<Building>();
            if (buildingScript != null && buildingScript.buildingName == "Headquarters1" || buildingScript.buildingName == "Headquarters2" || buildingScript.buildingName == "Headquarters3")
            {
                //RewardSystem.headquarterActive = true;
                headquarter = building; // Cache the headquarter
                //Debug.Log("Headquarter found: " + headquarter.name);
                break; // Exit the loop once found
            }
        }

        if (headquarter == null)
        {
            //Debug.LogWarning("No headquarter found!");
        }
    }


    public static bool IsEven(int number)
    {
        return number % 2 == 0;
    }
    //public void PlayAd()
    //{
    //    bool canPlayAd = IsEven(currentWaveId);
    //    if (canPlayAd)
    //    {
    //        GoogleAdsManager.ShowRewardedAd();
    //    }
    //    else
    //    {
    //        return;
    //    }

    //}

    public void ShowTotalActiveZombies()
    {
        zombieCounter.text = totalZombies.ToString();
    }


    public void SpawnZombieDrop(Vector3 position)
    {
        // Check if there are any prefabs in the array
        if (zombieDropPrefabs == null || zombieDropPrefabs.Length == 0)
        {
            Debug.LogWarning("No zombie drop prefabs assigned!");
            return;
        }

        // Select a random prefab from the array
        int randomIndex = Random.Range(0, zombieDropPrefabs.Length);
        GameObject selectedPrefab = zombieDropPrefabs[randomIndex];

        // Calculate spawn position
        Vector3 spawnPosition = new Vector3(position.x, position.y + dropYOffset, position.z);

        // Instantiate the selected prefab
        Instantiate(selectedPrefab, spawnPosition, Quaternion.identity);
    }

    public List<int> GetUniqueRandomIndices(int totalCount, int dropCount)
    {
        List<int> allIndices = new List<int>();
        for (int i = 0; i < totalCount - 2; i++)
        {
            allIndices.Add(i);
        }

        // Shuffle the list
        for (int i = 0; i < allIndices.Count; i++)
        {
            int randomIndex = Random.Range(i, allIndices.Count);
            int temp = allIndices[i];
            allIndices[i] = allIndices[randomIndex];
            allIndices[randomIndex] = temp;
        }

        // Return the first 'dropCount' elements
        return allIndices.GetRange(0, Mathf.Min(dropCount, allIndices.Count));
    }


    public void ShowGoldDropReward(int gold)
    {
        goldAmountText.text = $"+{gold}";
        goldDropRewardDisplayObject.SetActive(true);

    }

    public void ShowSteelDropReward(int steel)
    {
        steelAmountText.text = $"+{steel}";
        steelDropRewardDisplayObject.SetActive(true);
    }


    public void CheckZombieKillAchievements()
    {
        switch (totalKilledZombies)
        {
            case 50: OnKill50Zombies(); break;
            case 100: OnKill100Zombies(); break;
            case 500: OnKill500Zombies(); break;
            case 1000: OnKill1000Zombies(); break;
            case 5000: OnKill5000Zombies(); break;
            case 10000: OnKill10000Zombies(); break;
        }
    }

    public void CheckWaveAchievements(int currentWave)
    {
        switch (currentWave)
        {
            case 1: OnReachWave1(); break;
            case 10: OnReachWave10(); break;
            case 20: OnReachWave20(); break;
            case 50: OnReachWave50(); break;
            case 100: OnReachWave100(); break;
        }
    }
    private void OnKill50Zombies() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.FiftyKills);
    private void OnKill100Zombies() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.HundredKills);
    private void OnKill500Zombies() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.FiveHundredKills);
    private void OnKill1000Zombies() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.ThausandKills);
    private void OnKill5000Zombies() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.FiveThausandKills);
    private void OnKill10000Zombies() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.TenThausandKills);

    private void OnReachWave1() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.FirstWave);
    private void OnReachWave10() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.TenWaves);
    private void OnReachWave20() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.TwentyWaves);
    private void OnReachWave50() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.FiftyWaves);
    private void OnReachWave100() => SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.HundredWaves);


    private void SaveProgress()
    {
        PlayerPrefs.SetInt("TotalKilledZombies", totalKilledZombies);
        //PlayerPrefs.SetInt("CurrentWave", currentWave);
        PlayerPrefs.Save(); // Ensures the data is written
    }

    private void LoadProgress()
    {
        totalKilledZombies = PlayerPrefs.GetInt("TotalKilledZombies", 0); // Default to 0 if not found
                                                                          // currentWave = PlayerPrefs.GetInt("CurrentWave", 0);
    }

}
