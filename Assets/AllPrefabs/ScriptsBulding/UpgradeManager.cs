using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using TMPro;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using Unity.VisualScripting;

public class UpgradeManager : MonoBehaviour
{
    #region Singleton
    public static UpgradeManager Instance;
    #endregion

    #region Fields
    private string saveFilePath;
    private Dictionary<string, float> buildingTimers = new Dictionary<string, float>();
    private List<UpgradeData> upgradeDataList = new List<UpgradeData>();
    private string encryptionKey;
    private readonly string SALT = "YourGameSalt123";
    private float timer = 0f;
    private float interval = 180f;
    #endregion

    #region Properties
    public Building selectedBuilding { get; private set; }
    public bool isUpgrading => buildingTimers.Count > 0;
    #endregion

    #region SerializeFields
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text upgradeBuildingName;
    [SerializeField] private Material transparent;
    [SerializeField] private Material[] materials;
    [SerializeField] private CheckInProgress checkInProgress;
    #endregion


    //---------------
    //public GameObject mapUI;



    #region Unity Lifecycle Methods
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            encryptionKey = PlayerPrefs.GetString("EncryptionKey", "DefaultKey");
            //Debug.Log("UpgradeManager initialized with encryption key");
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        try
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata1.json");
            LoadUpgradeData();
            //Debug.Log("UpgradeManager: Data loaded successfully");
        }
        catch (Exception e)
        {
            Debug.LogError($"UpgradeManager Awake Error: {e.Message}");
        }
    }

    void Start()
    {
        // Start bo'lgandan keyin 10 sekundan keyin MyMethod ni chaqiradi
        Invoke("RemoveCompletedUpgrades", 5);
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            LoadUpgradeData();
            timer = 0f; // Timer qayta tiklanadi
        }
        UpdateAllUpgradeTimers();

    }
    #endregion

    #region Encryption Methods
    private byte[] GetEncryptionKeyBytes(string key)
    {
        using (var sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(key + SALT));
        }
    }

    private string EncryptData(string data, string key)
    {
        try
        {
            byte[] encryptionKeyBytes = GetEncryptionKeyBytes(key);

            using (Aes aes = Aes.Create())
            {
                aes.Key = encryptionKeyBytes;
                aes.GenerateIV();

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(data);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Encryption error: {e.Message}");
            return null;
        }
    }

    private string DecryptData(string encryptedData, string key)
    {
        try
        {
            byte[] encryptionKeyBytes = GetEncryptionKeyBytes(key);
            byte[] fullCipher = Convert.FromBase64String(encryptedData);

            using (Aes aes = Aes.Create())
            {
                byte[] iv = new byte[aes.IV.Length];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);

                using (MemoryStream msDecrypt = new MemoryStream())
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                        aes.CreateDecryptor(encryptionKeyBytes, iv),
                        CryptoStreamMode.Write))
                    {
                        csDecrypt.Write(fullCipher, iv.Length, fullCipher.Length - iv.Length);
                    }

                    return Encoding.UTF8.GetString(msDecrypt.ToArray());
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Decryption error: {e.Message}");
            return null;
        }
    }
    #endregion

    #region Public Methods
    public void StartUpgrade(Building building)
    {
        string buildingKey = GetBuildingKey(building);

        if (IsBuildingUpgrading(building.buildingName, building.transform.position, building.transform.rotation.eulerAngles))
        {
            //Debug.LogWarning($"Building {building.buildingName} is already upgrading!");
            return;
        }

        if (building.buildingName == "Walls")
        {
            building.Upgrade();
            ParticleSystemManager.Instance.PlayWallUpgrade(building.transform.position, building.transform.rotation);
            return;
        }

        DateTime currentTime = GetNetTime();
        float upgradeDuration = GameManager.Instance.GetUpgradeTimeForLevel(building);
        DateTime endTime = currentTime.AddSeconds(upgradeDuration);

        buildingTimers[buildingKey] = (float)(endTime - currentTime).TotalSeconds;

        selectedBuilding = building;
        upgradeBuildingName.text = GetUpgradingBuildingsText();
        ParticleSystemManager.Instance.PlayIsUpgrading(building.index);
        building.GetComponent<Renderer>().material = transparent;
        checkInProgress.AddToInProgressList(building.buildingName);
        SaveUpgradeData(building.buildingName, currentTime, endTime, building.transform.position, building.transform.rotation.eulerAngles);

        //Debug.Log($"Started upgrade for {building.buildingName} - Duration: {upgradeDuration}s, Expected completion: {endTime}");
        LogCurrentUpgrades();
    }

    public void GoldAds()
    {
        if (GameManager.Instance.gold >= 2)
        {
            GameManager.Instance.gold -= 2;
            ADD15Minut();
        }
        else
        {

            GameManager.Instance.NotEnoughtText2.text = "2";
            GameManager.Instance.notEnoughUI.SetActive(true);
        }

    }




    public void ADD15Minut()
    {
        if (selectedBuilding != null)
        {
            ReduceUpgradeTimeByFifteenMinutes(selectedBuilding);
            //Debug.LogWarning(">>>>>>>>>>>>>>>>>>>> pul berdikkkkkkkkkkkkk");
        }
        else
        {
            //Debug.LogWarning("No building selected for time reduction");
        }
    }

    public void ReduceUpgradeTimeByFifteenMinutes(Building building)
    {
        string buildingKey = GetBuildingKey(building);

        if (!buildingTimers.ContainsKey(buildingKey))
        {
            //Debug.LogWarning($"Building {building.buildingName} is not upgrading");
            return;
        }

        float timeReduction = 15 * 60;
        float oldTime = buildingTimers[buildingKey];
        buildingTimers[buildingKey] = Mathf.Max(0, buildingTimers[buildingKey] - timeReduction);

        //Debug.Log($"Reduced upgrade time for {building.buildingName} by 15 minutes. Old time: {oldTime}s, New time: {buildingTimers[buildingKey]}s");

        UpdateTimerText();
        UpdateEndTimeInDatabase(timeReduction, building);
    }

    public bool IsBuildingUpgrading(string buildingName, Vector3 position, Vector3 rotation)
    {
        string buildingKey = GetBuildingKey(buildingName, position, rotation);
        return buildingTimers.ContainsKey(buildingKey);
    }

    public void SelectBuilding(Building building)
    {
        string buildingKey = GetBuildingKey(building);

        if (!buildingTimers.ContainsKey(buildingKey))
        {
            //Debug.Log($"Selected building {building.buildingName} is not upgrading");
            ResetUpgradeUI();
            //------------
            //mapUI.SetActive(true);
            selectedBuilding = null;
            return;
        }

        selectedBuilding = building;
        UpdateTimerText();
        upgradeBuildingName.text = GetUpgradingBuildingsText();
        //Debug.Log($"Selected building {building.buildingName} - Remaining time: {buildingTimers[buildingKey]}s");
    }
    #endregion

    #region Private Methods
    private void UpdateAllUpgradeTimers()
    {
        if (buildingTimers.Count == 0) return;

        // Create a separate list of building keys to avoid modification during enumeration
        var buildingKeys = new List<string>(buildingTimers.Keys);
        var completedBuildings = new List<string>();

        foreach (var buildingKey in buildingKeys)
        {
            var upgradeData = upgradeDataList.Find(data =>
            GetBuildingKey(data.buildingName, data.position, data.rotation) == buildingKey);
            if (buildingTimers.TryGetValue(buildingKey, out float currentTime))
            {
                float remainingTime = currentTime - Time.deltaTime;

                if (remainingTime <= 0)
                {
                    completedBuildings.Add(buildingKey);
                    //Debug.Log($"Building {GetBuildingNameFromKey(buildingKey)} upgrade time has completed");
                }
                else
                {
                    Building building = FindBuildingByNameAndPosition(upgradeData.buildingName, upgradeData.position);
                    ParticleSystemManager.Instance.PlayIsUpgrading(building.index);
                    building.GetComponent<Renderer>().material = transparent;
                    checkInProgress.AddToInProgressList(upgradeData.buildingName);
                    buildingTimers[buildingKey] = remainingTime;
                }
            }
        }

        // Handle completed upgrades separately after the loop
        foreach (string buildingKey in completedBuildings)
        {
            HandleUpgradeCompletion(buildingKey);
        }

        if (selectedBuilding != null && buildingTimers.ContainsKey(GetBuildingKey(selectedBuilding)))
        {
            UpdateTimerText();
        }
    }

    public Building FindBuildingByNameAndPosition(string buildingName, Vector3 position, float searchRadius = 0.1f)
    {
        // Sahnadan barcha Building komponentlarini topamiz
        Building[] allBuildings = FindObjectsOfType<Building>();

        // Har bir buildingni tekshiramiz
        foreach (Building building in allBuildings)
        {
            // Building nomini tekshiramiz
            if (building.buildingName == buildingName)
            {
                // Pozitsiyani tekshiramiz (searchRadius radiusi ichida)
                if (Vector3.Distance(building.transform.position, position) < searchRadius)
                {
                    //Debug.Log($"Building topildi: {buildingName} at position {position}");
                    return building;
                }
            }
        }

        //Debug.LogWarning($"Building topilmadi: {buildingName} at position {position}");
        return null;
    }

    // HandleUpgradeCompletion metodini yangilaymiz
    private void HandleUpgradeCompletion(string buildingKey)
    {
        var upgradeData = upgradeDataList.Find(data =>
            GetBuildingKey(data.buildingName, data.position, data.rotation) == buildingKey);

        if (upgradeData != null)
        {
            // Building obyektini topamiz
            Building foundBuilding = FindBuildingByNameAndPosition(upgradeData.buildingName, upgradeData.position);


            if (foundBuilding != null)
            {
                ParticleSystemManager.Instance.StopIsUpgrading(foundBuilding.index);
                ParticleSystemManager.Instance.PlayLevelUp(foundBuilding.index);
                checkInProgress.RemoveFromInProgressList(upgradeData.buildingName);
                foundBuilding.Upgrade();
                foundBuilding.isBuilding = false;
                if (foundBuilding.index < 4)
                {
                    foundBuilding.GetComponent<Renderer>().material = materials[0];
                }
                else { foundBuilding.GetComponent<Renderer>().material = materials[1]; }

                //Debug.Log($"=== UPGRADE COMPLETED ===");
                //Debug.Log($"Building: {upgradeData.buildingName}");
                //Debug.Log($"Position: {upgradeData.position}");
                //Debug.Log($"Started: {upgradeData.startTime}");
                //Debug.Log($"Completed: {DateTime.UtcNow}");
                //Debug.Log($"=======================");
                GameManager.Instance.CheckMissingBuildings();

            }

            RemoveUpgradeData(upgradeData.buildingName, upgradeData.position, upgradeData.rotation);
        }

        buildingTimers.Remove(buildingKey);

        if (selectedBuilding != null && GetBuildingKey(selectedBuilding) == buildingKey)
        {
            ResetUpgradeUI();
            selectedBuilding = null;
        }

        upgradeBuildingName.text = GetUpgradingBuildingsText();
        LogCurrentUpgrades();
    }

    private string GetBuildingKey(Building building)
    {
        return GetBuildingKey(building.buildingName, building.transform.position, building.transform.rotation.eulerAngles);
    }

    private string GetBuildingKey(string buildingName, Vector3 position, Vector3 rotation)
    {
        return $"{buildingName}_{position}_{rotation}";
    }

    private string GetBuildingNameFromKey(string key)
    {
        return key.Split('_')[0];
    }

    private void UpdateTimerText()
    {
        if (selectedBuilding == null) return;

        string buildingKey = GetBuildingKey(selectedBuilding);
        if (buildingTimers.TryGetValue(buildingKey, out float timeRemaining))
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = $"{minutes:D2}:{seconds:D2}";
        }
    }

    private string GetUpgradingBuildingsText()
    {
        if (buildingTimers.Count == 0) return "";

        var upgradingBuildings = upgradeDataList
            .Where(data => buildingTimers.ContainsKey(GetBuildingKey(data.buildingName, data.position, data.rotation)))
            .Select(data => data.buildingName);

        return string.Join(", ", upgradingBuildings) + " is upgrading";
    }

    private void SaveUpgradeData(string buildingName, DateTime startTime, DateTime endTime, Vector3 position, Vector3 rotation)
    {
        UpgradeData existingData = upgradeDataList.Find(data =>
            data.buildingName == buildingName &&
            Vector3.Distance(data.position, position) < 0.1f &&
            Vector3.Distance(data.rotation, rotation) < 0.1f
        );

        if (existingData != null)
        {
            existingData.startTime = startTime.ToString("o");
            existingData.endTime = endTime.ToString("o");
            existingData.isUpgrading = true;
            existingData.position = position;
            existingData.rotation = rotation;
            //Debug.Log($"Updated existing upgrade data for {buildingName}");
        }
        else
        {
            UpgradeData newData = new UpgradeData
            {
                buildingName = buildingName,
                startTime = startTime.ToString("o"),
                endTime = endTime.ToString("o"),
                isUpgrading = true,
                position = position,
                rotation = rotation
            };
            upgradeDataList.Add(newData);
            //Debug.Log($"Added new upgrade data for {buildingName}");
        }
        SaveToFile();
    }

    private void LoadUpgradeData()
    {
        if (!File.Exists(saveFilePath))
        {
            //Debug.Log("No saved upgrade data found");
            return;
        }

        try
        {
            string encryptedData = File.ReadAllText(saveFilePath);
            string decryptedJson = DecryptData(encryptedData, encryptionKey);

            if (decryptedJson != null)
            {
                upgradeDataList = JsonUtility.FromJson<UpgradeDataList>(decryptedJson).upgradeData;
                //Debug.Log($"Loaded {upgradeDataList.Count} upgrade records");

                foreach (var data in upgradeDataList)
                {
                    DateTime endTime = DateTime.Parse(data.endTime);
                    DateTime currentTime = GetNetTime();

                    if (currentTime < endTime)
                    {
                        float remainingTime = (float)(endTime - currentTime).TotalSeconds;
                        string buildingKey = GetBuildingKey(data.buildingName, data.position, data.rotation);

                        buildingTimers[buildingKey] = remainingTime;
                        //Debug.Log($"Restored upgrade for {data.buildingName} - Remaining time: {remainingTime}s");
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading upgrade data: {e.Message}");
        }
    }

    private void RemoveCompletedUpgrades()
    {
        var completedUpgrades = upgradeDataList.Where(data =>
            DateTime.Parse(data.endTime) <= GetNetTime()).ToList();

        foreach (var completedUpgrade in completedUpgrades)
        {
            Building foundBuilding = FindBuildingByNameAndPosition(completedUpgrade.buildingName, completedUpgrade.position);
            ParticleSystemManager.Instance.PlayLevelUp(foundBuilding.index);
            //Debug.Log($"Removing completed upgrade for {completedUpgrade.buildingName}");
            //Debug.Log($"Building name: {completedUpgrade.buildingName}");
            //Debug.Log($"Position: {completedUpgrade.position}");
            //Debug.Log($"Rotation: {completedUpgrade.rotation}");
            foundBuilding.Upgrade();
            RemoveUpgradeData(completedUpgrade.buildingName, completedUpgrade.position, completedUpgrade.rotation);
        }
    }


    private void SaveToFile()
    {
        try
        {
            UpgradeDataList dataList = new UpgradeDataList { upgradeData = upgradeDataList };
            string jsonData = JsonUtility.ToJson(dataList, true);
            string encryptedData = EncryptData(jsonData, encryptionKey);

            if (encryptedData != null)
            {
                File.WriteAllText(saveFilePath, encryptedData);
                //Debug.Log($"Saved {upgradeDataList.Count} upgrade records to file");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving upgrade data: {e.Message}");
        }
    }

    private void RemoveUpgradeData(string buildingName, Vector3 position, Vector3 rotation)
    {
        int countBefore = upgradeDataList.Count;
        upgradeDataList.RemoveAll(data =>
            data.buildingName == buildingName &&
            Vector3.Distance(data.position, position) < 0.1f &&
            Vector3.Distance(data.rotation, rotation) < 0.1f
        );
        int removed = countBefore - upgradeDataList.Count;

        //Debug.Log($"Removed {removed} upgrade data entries for {buildingName}");
        SaveToFile();
    }

    private void ResetUpgradeUI()
    {
        timerText.text = "";
        upgradeBuildingName.text = "";
        //Debug.Log("Reset upgrade UI elements");
    }

    private void LogCurrentUpgrades()
    {
        //Debug.Log("=== CURRENT UPGRADES STATUS ===");
        foreach (var kvp in buildingTimers)
        {
            string buildingName = GetBuildingNameFromKey(kvp.Key);
            float remainingTime = kvp.Value;
            //Debug.Log($"Building: {buildingName} - Remaining Time: {remainingTime:F1}s");
        }
        //Debug.Log("=============================");
    }

    private void UpdateEndTimeInDatabase(float timeReduction, Building building)
    {
        if (building == null)
        {
            //Debug.LogError("Cannot update end time: building is null");
            return;
        }

        UpgradeData existingData = upgradeDataList.Find(data =>
            data.buildingName == building.buildingName &&
            Vector3.Distance(data.position, building.transform.position) < 0.1f &&
            Vector3.Distance(data.rotation, building.transform.rotation.eulerAngles) < 0.1f
        );

        if (existingData != null)
        {
            DateTime oldEndTime = DateTime.Parse(existingData.endTime);
            DateTime newEndTime = oldEndTime.AddSeconds(-timeReduction);
            existingData.endTime = newEndTime.ToString("o");

            //Debug.Log($"Updated end time for {building.buildingName}");
            //Debug.Log($"Old end time: {oldEndTime}");
            //Debug.Log($"New end time: {newEndTime}");

            SaveToFile();
        }
        else
        {
            //Debug.LogWarning($"Could not find upgrade data for {building.buildingName} to update end time");
        }
    }

    private Building CreateDummyBuilding(string buildingName, Vector3 position, Vector3 rotation)
    {
        GameObject dummyObject = new GameObject($"Dummy_{buildingName}");
        Building dummyBuilding = dummyObject.AddComponent<Building>();
        dummyBuilding.buildingName = buildingName;
        dummyBuilding.transform.position = position;
        dummyBuilding.transform.rotation = Quaternion.Euler(rotation);

        //Debug.Log($"Created dummy building: {buildingName} at position {position}");
        return dummyBuilding;
    }

    public static DateTime GetNetTime()
    {
        if (NetworkTimeManager.Instance != null)
        {
            return NetworkTimeManager.Instance.GetCurrentNetworkTime();
        }
        return DateTime.Now;
    }

    #endregion
}

[Serializable]
public class UpgradeData
{
    public string buildingName;
    public string startTime;
    public string endTime;
    public bool isUpgrading;
    public Vector3 position;
    public Vector3 rotation;
}

[Serializable]
public class UpgradeDataList
{
    public List<UpgradeData> upgradeData;
}