using System;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;


public class ManageAllScripts : MonoBehaviour
{
    public static ManageAllScripts Instance;
    private string saveFilePath;
    private string firstTimeCheckPath; // Birinchi marta kirish uchun tekshirish fayli
    private List<BuildingData> buildingDataList = new List<BuildingData>();
    private BombData bombData = new BombData();
    private Dictionary<string, BuildingData> lastSavedState = new Dictionary<string, BuildingData>();
    private string encryptionKeyPath; // Shifrlash kaliti saqlanadigan fayl
    private static string encryptionKey; // Shifrlash kaliti

    public GameObject bombPrefab;
    public GameObject minePrefab;

    public GameObject wallPrefabLevel1;
    public GameObject wallPrefabLevel2;
    public GameObject wallPrefabLevel3;

    public GameObject workShopPrefabLevel1;

    public GameObject StoragePrefabLevel1;
    public GameObject StoragePrefabLevel2;
    public GameObject StoragePrefabLevel3;

    public GameObject towerPrefabLevel1;
    public GameObject towerPrefabLevel2;
    public GameObject towerPrefabLevel3;

    public GameObject architecturePrefabLevel1;
    public GameObject architecturePrefabLevel2;
    public GameObject architecturePrefabLevel3;

    public GameObject headquartersPrefabLevel1;
    public GameObject headquartersPrefabLevel2;
    public GameObject headquartersPrefabLevel3;

    public GameObject DestroyHeadquarter_1;
    public GameObject DestroyHeadquarter_2;
    public GameObject DestroyHeadquarter_3;

    public GameObject DestroySklad_1;
    public GameObject DestroySklad_2;
    public GameObject DestroySklad_3;

    public GameObject DestroyArchitecture_1;
    public GameObject DestroyArchitecture_2;
    public GameObject DestroyArchitecture_3;

    public GameObject DestroyTower_1;
    public GameObject DestroyTower_2;
    public GameObject DestroyTower_3;

    public GameObject DestroyWall_1;
    public GameObject DestroyWall_2;
    public GameObject DestroyWall_3;

    public GameObject DestroyWorkshop;

    [Header("Save Settings")]
    public float saveInterval = 3f;
    private float saveTimer = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    async void Start()
    {
        InitializePaths();  // Birinchi InitializePaths chaqiriladi
        SetEncryptionKey("88888888"); // Keyin SetEncryptionKey
        await HandleFirstTimeLogin();
    }
    void Update()
    {
        saveTimer += Time.deltaTime;
        if (saveTimer >= saveInterval)
        {
            Fix();
            SaveGame();
            saveTimer = 0f; // Timer qayta tiklanadi
        }
    }

    private void InitializePaths()
    {
        // Application.persistentDataPath ni oldindan tekshiramiz
        if (string.IsNullOrEmpty(Application.persistentDataPath))
        {
            //Debug.LogError("Application.persistentDataPath is null or empty!");
            return;
        }

        try
        {
            saveFilePath = Path.Combine(Application.persistentDataPath, "gamedata.json");
            firstTimeCheckPath = Path.Combine(Application.persistentDataPath, "first_time_check.dat");
            encryptionKeyPath = Path.Combine(Application.persistentDataPath, "encryption_key.dat");

            // Papka mavjudligini tekshirish
            string directory = Path.GetDirectoryName(saveFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            //Debug.Log($"Save file path: {saveFilePath}");
            //Debug.Log($"First time check path: {firstTimeCheckPath}");
            //Debug.Log($"Encryption key path: {encryptionKeyPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error in InitializePaths: {ex.Message}");
        }
    }

    public void SetEncryptionKey(string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            //Debug.LogError("Shifrlash kaliti bo'sh bo'lishi mumkin emas!");
            return;
        }

        if (string.IsNullOrEmpty(encryptionKeyPath))
        {
            //Debug.LogError("encryptionKeyPath is null or empty!");
            return;
        }

        try
        {
            encryptionKey = key;
            string directory = Path.GetDirectoryName(encryptionKeyPath);

            // Papka mavjud emasligini tekshirish
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Kalitni xavfsiz joyda saqlash
            File.WriteAllText(encryptionKeyPath, EncryptString(key, key));
            //Debug.Log($"Encryption key successfully saved to: {encryptionKeyPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Kalitni saqlashda xatolik: {ex.Message}");
        }
    }

    private string EncryptString(string text, string key)
    {
        try
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            // Key uzunligini 32 baytga keltirish (256 bit)
            Array.Resize(ref keyBytes, 32);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.GenerateIV(); // Tasodifiy IV generatsiya qilish

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    // IV ni faylning boshiga yozish
                    msEncrypt.Write(aes.IV, 0, aes.IV.Length);

                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }

                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Shifrlashda xatolik: {ex.Message}");
            return null;
        }
    }

    private string DecryptString(string cipherText, string key)
    {
        try
        {
            byte[] fullCipher = Convert.FromBase64String(cipherText);
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            // Key uzunligini 32 baytga keltirish (256 bit)
            Array.Resize(ref keyBytes, 32);

            using (Aes aes = Aes.Create())
            {
                aes.Key = keyBytes;

                // IV ni faylning boshidan o'qish
                byte[] iv = new byte[aes.IV.Length];
                Array.Copy(fullCipher, 0, iv, 0, iv.Length);
                aes.IV = iv;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(fullCipher, iv.Length, fullCipher.Length - iv.Length))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    return srDecrypt.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Deshifrlashda xatolik: {ex.Message}");
            return null;
        }
    }

    private async Task HandleFirstTimeLogin()
    {
        try
        {
            bool isFirstTime = !File.Exists(firstTimeCheckPath);

            if (isFirstTime)
            {
                //Debug.Log("Birinchi marta kirish aniqlandi. Boshlang'ich ma'lumotlar saqlanmoqda...");
                /*                Fix();*/ // Binolarning levellarini to'g'rilash
                await SaveGameDataAsync(); // O'yin ma'lumotlarini saqlash

                // Birinchi marta kirish belgisini yaratish
                File.WriteAllText(firstTimeCheckPath, DateTime.Now.ToString());

                await LoadGameDataAsync(); // Saqlangan ma'lumotlarni yuklash
            }
            else
            {
                //Debug.Log("Qayta kirish aniqlandi. Ma'lumotlar yuklanmoqda...");
                if (File.Exists(saveFilePath))
                {
                    await LoadGameDataAsync();
                }
                else
                {
                    //Debug.LogWarning("Saqlangan ma'lumotlar topilmadi. Yangi o'yin boshlanmoqda...");
                    Fix();
                    await SaveGameDataAsync();
                    await LoadGameDataAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Ma'lumotlarni yuklashda xatolik: {ex.Message}");
            // Xatolik yuz berganda ham o'yinni ishga tushirish uchun
            Fix();
            await SaveGameDataAsync();
            await LoadGameDataAsync();
        }
    }

    public int GetLevelFromName(string name)
    {
        // Regex yordamida raqamni ajratib olish
        Match match = Regex.Match(name, @"\d+");

        if (match.Success)
        {
            // Agar raqam topilsa, uni int formatiga aylantiramiz va qaytaramiz
            return int.Parse(match.Value);
        }

        // Agar raqam topilmasa, 0 qaytariladi
        return 1;
    }

    public void Fix()
    {
        Building[] allBuildings = FindObjectsOfType<Building>();
        foreach (Building building in allBuildings)
        {
            if (building.level == 0) return;
            building.level = GetLevelFromName(building.name);
        }

    }

    public async Task SaveGameDataAsync()
    {
        try
        {
            if (string.IsNullOrEmpty(encryptionKey))
            {
                //Debug.LogError("Shifrlash kaliti o'rnatilmagan!");
                return;
            }

            // Avval faylni tozalash
            if (File.Exists(saveFilePath))
            {
                try
                {
                    // Faylni o'chirish
                    File.WriteAllText(saveFilePath, string.Empty);
                    //Debug.Log("Oldingi saqlangan ma'lumotlar tozalandi");
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Faylni tozalashda xatolik: {ex.Message}");
                    return;
                }
            }

            GameData gameData = new GameData
            {
                gold = GameManager.Instance.gold,
                steel = GameManager.Instance.steel,
                waveLevel = GameManager.Instance.waveLevel,
                buildingData = new List<BuildingData>(),
                bombData = new List<BombData>(),
                mineData = new List<MineData>()
            };

            // Buildinglarni saqlash
            Building[] allBuildings = FindObjectsOfType<Building>();
            foreach (Building building in allBuildings)
            {
                BuildingData data = new BuildingData
                {
                    buildingName = building.buildingName,
                    level = building.level,
                    goldCost = building.goldCost,
                    steelCost = building.steelCost,
                    position = building.transform.position,
                    rotation = building.transform.rotation,
                    index = building.index,
                    isBuilding = building.isBuilding,

                };
                gameData.buildingData.Add(data);
            }

            // Bomblarni saqlash
            BombScript[] allBombs = FindObjectsOfType<BombScript>();
            foreach (BombScript bomb in allBombs)
            {
                BombData data = new BombData
                {
                    bombId = bomb.name,
                    position = bomb.transform.position,
                    rotation = bomb.transform.rotation,
                    isActive = bomb.enabled
                };
                gameData.bombData.Add(data);
            }

            // Minalarni saqlash
            MineScript[] allMines = FindObjectsOfType<MineScript>();
            foreach (MineScript mine in allMines)
            {
                MineData data = new MineData
                {
                    mineId = mine.name,
                    position = mine.transform.position,
                    rotation = mine.transform.rotation,
                    isActive = mine.enabled
                };
                gameData.mineData.Add(data);
            }

            string json = JsonUtility.ToJson(gameData, true);
            // JSON ni shifrlash
            string encryptedJson = EncryptString(json, encryptionKey);

            if (encryptedJson != null)
            {
                await File.WriteAllTextAsync(saveFilePath, encryptedJson);
                //Debug.Log($"O'yin muvaffaqiyatli shifrlandi va saqlandi: {saveFilePath}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"O'yinni saqlashda xatolik: {ex.Message}");
        }
    }

    public async Task LoadGameDataAsync()
    {
        try
        {

            if (string.IsNullOrEmpty(encryptionKey))
            {
                //Debug.LogError("Shifrlash kaliti o'rnatilmagan!");
                return;
            }

            if (File.Exists(saveFilePath))
            {
                string encryptedJson = await File.ReadAllTextAsync(saveFilePath);
                if (!string.IsNullOrEmpty(encryptedJson))
                {
                    // JSON ni deshifrlash
                    string json = DecryptString(encryptedJson, encryptionKey);
                    if (json != null)
                    {
                        ClearScene();
                        GameData gameData = JsonUtility.FromJson<GameData>(json);

                        // Asosiy ma'lumotlarni yuklash
                        if (GameManager.Instance != null)
                        {
                            GameManager.Instance.gold = gameData.gold;
                            GameManager.Instance.steel = gameData.steel;
                            GameManager.Instance.waveLevel = gameData.waveLevel;

                        }

                        // Buildinglarni yuklash
                        foreach (BuildingData data in gameData.buildingData)
                        {
                            CreateBuilding(data);
                        }

                        // Bomblarni yuklash
                        foreach (BombData data in gameData.bombData)
                        {
                            CreateBomb(data);
                        }

                        // Minalarni yuklash
                        foreach (MineData data in gameData.mineData)
                        {
                            CreateMine(data);
                        }

                        //Debug.Log("O'yin muvaffaqiyatli yuklandi");
                        //Debug.Log("O'yin muvaffaqiyatli deshifrlandi va yuklandi");
                    }
                }
                else
                {
                    //Debug.LogWarning("Saqlangan fayl bo'sh. Yangi o'yin boshlanmoqda...");
                    await SaveGameDataAsync();
                }
            }
            else
            {
                //Debug.LogWarning("Saqlangan fayl topilmadi. Yangi o'yin boshlanmoqda...");
                await SaveGameDataAsync();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"O'yinni yuklashda xatolik: {ex.Message}");
            await SaveGameDataAsync();
        }
    }

    private void CreateBomb(BombData data)
    {
        GameObject newBomb = Instantiate(bombPrefab, data.position, data.rotation);
        BombScript bombScript = newBomb.GetComponent<BombScript>();
        if (bombScript != null)
        {
            bombScript.enabled = data.isActive;
            // Boshqa kerakli maydonlarni o'rnatish
        }
    }

    private void CreateMine(MineData data)
    {
        GameObject newMine = Instantiate(minePrefab, data.position, data.rotation);
        MineScript mineScript = newMine.GetComponent<MineScript>();
        if (mineScript != null)
        {
            mineScript.enabled = data.isActive;
            // Boshqa kerakli maydonlarni o'rnatish
        }
    }



    public void ClearScene()
    {
        // Buildinglarni o'chirish
        var buildings = FindObjectsOfType<Building>();
        foreach (var building in buildings)
        {
            if (building.name == "RocketHit" || building.name == "rocket(Clone)"
                || building.gameObject.name == "rocket(Clone)" || building.gameObject.name == "RocketHit"
                || building.gameObject.name == "MSUI" || building.name == "MSUI"
                || building.gameObject.name == "Missile System")
            {
                return;
            }
            else
            {
                Destroy(building.gameObject);

            }
        }

        // Bomblarni o'chirish
        var bombs = FindObjectsOfType<BombScript>();
        foreach (var bomb in bombs)
        {
            Destroy(bomb.gameObject);
        }

        // Minalarni o'chirish
        var mines = FindObjectsOfType<MineScript>();
        foreach (var mine in mines)
        {
            Destroy(mine.gameObject);
        }

        //Debug.Log("Cleared all objects from the scene.");
    }

    private void CreateBuilding(BuildingData data)
    {

        // Prefabni tanlash
        GameObject newBuildingPrefab = GetPrefabByBuildingData(data);
        if (newBuildingPrefab != null)
        {


            // Agar bina mavjud bo'lmasa, yangisini yaratish
            GameObject newBuilding = Instantiate(newBuildingPrefab, data.position, data.rotation);
            Building newBuildingComponent = newBuilding.GetComponent<Building>();
            newBuildingComponent.index = data.index;
            newBuildingComponent.isBuilding = data.isBuilding;


        }
        else
        {
            //Debug.Log($"Prefab topilmadi: {data.buildingName} Level {data.level}");
        }
    }
    private GameObject GetPrefabByBuildingData(BuildingData data)
    {
        switch (data.buildingName)
        {
            case "Walls":
                return GetWallPrefab(data.level);
            case "Tower":
                return GetTowerPrefab(data.level);
            case "Architecture":
                return GetArchitecturePrefab(data.level);
            case "Headquarters1":
                return GetHeadquarters1Prefab(data.level);
            case "Headquarters2":
                return GetHeadquarters2Prefab(data.level);
            case "Headquarters3":
                return GetHeadquarters3Prefab(data.level);
            case "Storage":
                return GetStoragePrefab(data.level);
            case "Workshop":
                return GetWorkShopPrefab(data.level);
            case "DestroyWall_1":
                return GetWallPrefab_Des_1(data.level);
            case "DestroyWall_2":
                return GetWallPrefab_Des_2(data.level);
            case "DestroyWall_3":
                return GetWallPrefab_Des_3(data.level);
            case "DestroyWorkshop":
                return GetWorkshopPrefab_Des(data.level);
            case "DestroyArchitecture_1":
                return GetArchitecturePrefab_Des_1(data.level);
            case "DestroyArchitecture_2":
                return GetArchitecturePrefab_Des_2(data.level);
            case "DestroyArchitecture_3":
                return GetArchitecturePrefab_Des_3(data.level);
            case "DestroyHeadquarter_1":
                return GetHeadquarterPrefab_Des_1(data.level);
            case "DestroyHeadquarter_2":
                return GetHeadquarterPrefab_Des_2(data.level);
            case "DestroyHeadquarter_3":
                return GetHeadquarterPrefab_Des_3(data.level);
            case "DestroySklad_1":
                return GetSkladPrefab_Des_1(data.level);
            case "DestroySklad_2":
                return GetSkladPrefab_Des_2(data.level);
            case "DestroySklad_3":
                return GetSkladPrefab_Des_3(data.level);
            case "DestroyTower_1":
                return GetTowerPrefab_Des_1(data.level);
            case "DestroyTower_2":
                return GetTowerPrefab_Des_2(data.level);
            case "DestroyTower_3":
                return GetTowerPrefab_Des_3(data.level);
            default:
                Debug.Log($"Noma'lum bino turi: {data.buildingName}");
                return null;
        }
    }

    // GetWallPrefab, GetTowerPrefab va boshqa Get...Prefab metodlari o'zgarishsiz qoladi
    private GameObject GetWallPrefab_Des_1(int level)
    {
        switch (level)
        {
            case 1: return DestroyWall_1;
            default: return null;
        }
    }

    private GameObject GetWallPrefab_Des_2(int level)
    {
        switch (level)
        {
            case 2: return DestroyWall_2;
            default: return null;
        }
    }
    private GameObject GetWallPrefab_Des_3(int level)
    {
        switch (level)
        {
            case 3: return DestroyWall_3;
            default: return null;
        }
    }

    private GameObject GetHeadquarterPrefab_Des_1(int level)
    {
        switch (level)
        {
            case 1: return DestroyHeadquarter_1;
            default: return null;
        }
    }
    private GameObject GetHeadquarterPrefab_Des_2(int level)
    {
        switch (level)
        {
            case 2: return DestroyHeadquarter_2;
            default: return null;
        }
    }
    private GameObject GetHeadquarterPrefab_Des_3(int level)
    {
        switch (level)
        {
            case 3: return DestroyHeadquarter_3;
            default: return null;
        }
    }
    private GameObject GetSkladPrefab_Des_1(int level)
    {
        switch (level)
        {
            case 1: return DestroySklad_1;
            default: return null;
        }
    }
    private GameObject GetSkladPrefab_Des_2(int level)
    {
        switch (level)
        {
            case 2: return DestroySklad_2;
            default: return null;
        }
    }
    private GameObject GetSkladPrefab_Des_3(int level)
    {
        switch (level)
        {
            case 3: return DestroySklad_3;
            default: return null;
        }
    }
    private GameObject GetArchitecturePrefab_Des_1(int level)
    {
        switch (level)
        {
            case 1: return DestroyArchitecture_1;
            default: return null;
        }
    }
    private GameObject GetArchitecturePrefab_Des_2(int level)
    {
        switch (level)
        {
            case 2: return DestroyArchitecture_2;
            default: return null;
        }
    }
    private GameObject GetArchitecturePrefab_Des_3(int level)
    {
        switch (level)
        {
            case 3: return DestroyArchitecture_3;
            default: return null;
        }
    }
    private GameObject GetWorkshopPrefab_Des(int level)
    {
        switch (level)
        {
            case 1: return DestroyWorkshop;
            default: return null;
        }
    }
    private GameObject GetTowerPrefab_Des_1(int level)
    {
        switch (level)
        {
            case 1: return DestroyTower_1;
            default: return null;
        }
    }
    private GameObject GetTowerPrefab_Des_2(int level)
    {
        switch (level)
        {
            case 2: return DestroyTower_2;
            default: return null;
        }
    }
    private GameObject GetTowerPrefab_Des_3(int level)
    {
        switch (level)
        {
            case 3: return DestroyTower_3;
            default: return null;
        }
    }
    private GameObject GetStoragePrefab(int level)
    {
        switch (level)
        {
            case 1: return StoragePrefabLevel1;
            case 2: return StoragePrefabLevel2;
            case 3: return StoragePrefabLevel3;
            default: return null;
        }
    }

    private GameObject GetWorkShopPrefab(int level)
    {
        switch (level)
        {
            case 1: return workShopPrefabLevel1;
            default: return null;
        }
    }

    private GameObject GetWallPrefab(int level)
    {
        switch (level)
        {
            case 1: return wallPrefabLevel1;
            case 2: return wallPrefabLevel2;
            case 3: return wallPrefabLevel3;
            default: return null;
        }
    }

    private GameObject GetTowerPrefab(int level)
    {
        switch (level)
        {
            case 1: return towerPrefabLevel1;
            case 2: return towerPrefabLevel2;
            case 3: return towerPrefabLevel3;
            default: return null;
        }
    }

    private GameObject GetArchitecturePrefab(int level)
    {
        switch (level)
        {
            case 1: return architecturePrefabLevel1;
            case 2: return architecturePrefabLevel2;
            case 3: return architecturePrefabLevel3;
            default: return null;
        }
    }

    private GameObject GetHeadquarters1Prefab(int level)
    {
        switch (level)
        {
            case 1: return headquartersPrefabLevel1 ;
            case 2: return headquartersPrefabLevel2;
            case 3: return headquartersPrefabLevel3;
            default: return null;
        }
    }
    private GameObject GetHeadquarters2Prefab(int level)
    {
        switch (level)
        {
            case 1: return headquartersPrefabLevel1;
            case 2: return headquartersPrefabLevel2;
            case 3: return headquartersPrefabLevel3;
            default: return null;
        }
    }
    private GameObject GetHeadquarters3Prefab(int level)
    {
        switch (level)
        {
            case 1: return headquartersPrefabLevel1;
            case 2: return headquartersPrefabLevel2;
            case 3: return headquartersPrefabLevel3;
            default: return null;
        }
    }
    public async void SaveGame()
    {
        await SaveGameDataAsync();
    }

    public async void LoadGame()
    {
        await LoadGameDataAsync();
    }
}

[System.Serializable]
public class BuildingData
{
    public string buildingName;
    public int level;
    public int goldCost;
    public int steelCost;
    public Vector3 position;
    public Quaternion rotation;
    public int index;
    public bool isBuilding;
}



[System.Serializable]
public class GameData
{
    public List<BuildingData> buildingData;
    public List<BombData> bombData;
    public List<MineData> mineData;
    public int gold;
    public int steel;
    public int waveLevel;
}


[System.Serializable]
public class BombData
{
    public string bombId;
    public Vector3 position;
    public Quaternion rotation;
    public bool isActive;
    // BombScript dan kerakli boshqa maydonlar
}

[System.Serializable]
public class MineData
{
    public string mineId;
    public Vector3 position;
    public Quaternion rotation;
    public bool isActive;
    // MineScript dan kerakli boshqa maydonlar
}

