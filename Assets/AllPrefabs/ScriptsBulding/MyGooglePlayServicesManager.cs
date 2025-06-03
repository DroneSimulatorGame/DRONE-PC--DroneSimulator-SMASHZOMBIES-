//using System;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using TMPro;
//using GooglePlayGames;
//using GooglePlayGames.BasicApi;
//using Unity.Services.Authentication;
//using Unity.Services.Core;
//using Unity.Services.CloudSave;
//using UnityEngine;
//using UnityEngine.UI;
//using Unity.Services.CloudSave.Models;
//using System.IO;

///// <summary>
///// Ushbu sinf Google Play Games va Unity Services integratsiyasini boshqaradi.
///// Autentifikatsiya, cloud save va foydalanuvchi ma'lumotlarini saqlash
///// funksionalligini ta'minlaydi. Har 5 daqiqada avtomatik sinxronizatsiya qiladi.
///// </summary>
//public class MyGooglePlayServicesManager : MonoBehaviour
//{
//    [Header("UI Elementlari")]
//    [SerializeField] private Text statusText;
//    [SerializeField] private Button loginButton;

//    [Header("Avtomatik Sinxronizasiya Sozlamalari")]
//    [SerializeField] private float autoSyncInterval = 120f; // 5 daqiqa (soniyalarda)

//    public TMP_Text username;
//    private string authToken;
//    private string errorMessage;
//    private float lastSyncTime;
//    private bool isInitialized;
//    private bool isSyncing;

//    #region Unity Hayot Sikli Metodlari

//    private void Awake()
//    {
//        try
//        {
//            Application.runInBackground = true;
//            Application.targetFrameRate = 60;

//            PlayGamesPlatform.Activate();
//            SetupUI();
//            InitializeServices();
//        }
//        catch (Exception e)
//        {
//            HandleError("Awake xatosi", e);
//        }
//    }

//    private void Update()
//    {
//        try
//        {
//            if (!isInitialized || !AuthenticationService.Instance.IsSignedIn) return;

//            // Har 5 daqiqada avtomatik sinxronizatsiya
//            if (Time.time - lastSyncTime >= autoSyncInterval && !isSyncing)
//            {
//                _ = AutoSyncSaveSystem();
//            }
//        }
//        catch (Exception e)
//        {
//            HandleError("Update xatosi", e);
//        }
//    }

//    private void OnApplicationPause(bool pauseStatus)
//    {
//        try
//        {
//            if (!pauseStatus && isInitialized)
//            {
//                // O'yin qayta ochilganda sinxronizatsiya
//                _ = SyncSaveSystemWithCloud();
//            }
//        }
//        catch (Exception e)
//        {
//            HandleError("OnApplicationPause xatosi", e);
//        }
//    }

//    private void OnApplicationQuit()
//    {
//        try
//        {
//            if (isInitialized)
//            {
//                // O'yin yopilishidan oldin sinxronizatsiya
//                _ = SyncSaveSystemWithCloud();
//            }
//        }
//        catch (Exception e)
//        {
//            HandleError("OnApplicationQuit xatosi", e);
//        }
//    }

//    #endregion

//    #region Boshlash

//    private async void InitializeServices()
//    {
//        try
//        {
//            await UnityServices.InitializeAsync();
//            InitializeAuthenticationEvents();

//            await SignInWithGooglePlay();

//            isInitialized = true;
//        }
//        catch (Exception e)
//        {
//            HandleError("Xizmatlar boshlashda xato", e);
//            isInitialized = false;
//        }
//    }

//    private void SetupUI()
//    {
//        try
//        {
//            if (loginButton != null)
//            {
//                loginButton.onClick.AddListener(async () => await SignInWithGooglePlay());
//            }
//        }
//        catch (Exception e)
//        {
//            HandleError("UI sozlanishida xato", e);
//        }
//    }

//    #endregion

//    #region Autentifikatsiya

//    private void InitializeAuthenticationEvents()
//    {
//        try
//        {
//            var auth = AuthenticationService.Instance;

//            auth.SignedIn += async () =>
//            {
//                try
//                {
//                    string playerName = PlayGamesPlatform.Instance.GetUserDisplayName();
//                    username.text = $"{playerName}";
//                    UpdateStatus($"Hisobga kirildi: {playerName}");

//                    await auth.UpdatePlayerNameAsync(playerName);
//                    await HandlePostAuthentication();
//                }
//                catch (Exception e)
//                {
//                    HandleError("Avtorizatsiyadan keyingi jarayon muvaffaqiyatsiz", e);
//                }
//            };

//            auth.SignedOut += () => UpdateStatus("Hisobdan chiqildi");
//            auth.Expired += () => UpdateStatus("Sessiya muddati tugagan");
//            auth.SignInFailed += (err) => UpdateStatus($"Hisobga kirish muvaffaqiyatsiz: {err.Message}");
//        }
//        catch (Exception e)
//        {
//            HandleError("Avtentifikatsiya hodisalarini boshlashda xato", e);
//        }
//    }

//    private async Task SignInWithGooglePlay()
//    {
//        try
//        {
//            UpdateStatus("Hisobga kirilmoqda...");

//            SignInStatus status = await AuthenticateWithGooglePlayGamesAsync();

//            if (status == SignInStatus.Success)
//            {
//                string serverAuthCode = await RequestServerSideAccessAsync();
//                await AuthenticateWithUnityAsync(serverAuthCode);
//            }
//            else
//            {
//                UpdateStatus("Google Play Games autentifikatsiyasi muvaffaqiyatsiz");
//            }
//        }
//        catch (Exception e)
//        {
//            HandleError("Hisobga kirish jarayonida xato", e);
//        }
//    }

//    private async Task<SignInStatus> AuthenticateWithGooglePlayGamesAsync()
//    {
//        var tcs = new TaskCompletionSource<SignInStatus>();

//        try
//        {
//            PlayGamesPlatform.Instance.Authenticate((status) =>
//            {
//                tcs.SetResult(status);
//            });
//        }
//        catch (Exception e)
//        {
//            HandleError("Google Play Games autentifikatsiyasi muvaffaqiyatsiz", e);
//            tcs.SetException(e);
//        }

//        return await tcs.Task;
//    }

//    private async Task<string> RequestServerSideAccessAsync()
//    {
//        var tcs = new TaskCompletionSource<string>();

//        try
//        {
//            PlayGamesPlatform.Instance.RequestServerSideAccess(true, code =>
//            {
//                tcs.SetResult(code);
//            });
//        }
//        catch (Exception e)
//        {
//            HandleError("Server tomoni access so'rovi muvaffaqiyatsiz", e);
//            tcs.SetException(e);
//        }

//        return await tcs.Task;
//    }

//    private async Task AuthenticateWithUnityAsync(string authCode)
//    {
//        try
//        {
//            if (!AuthenticationService.Instance.IsSignedIn)
//            {
//                await AuthenticationService.Instance.SignInWithGooglePlayGamesAsync(authCode);
//            }
//        }
//        catch (Exception e)
//        {
//            throw new Exception($"Unity autentifikatsiyasi muvaffaqiyatsiz: {e.Message}");
//        }
//    }

//    #endregion

//    #region Cloud Save Operatsiyalari

//    private async Task HandlePostAuthentication()
//    {
//        try
//        {
//            string userId = PlayGamesPlatform.Instance.GetUserId();
//            string userName = PlayGamesPlatform.Instance.GetUserDisplayName();

//            bool isReturningUser = await CheckIfUserExistsInCloud();

//            if (isReturningUser)
//            {
//                await LoadUserDataFromCloud();
//            }
//            else
//            {
//                await SaveUserDataToCloud(userId, userName);
//            }

//            await SyncSaveSystemWithCloud();
//            lastSyncTime = Time.time;
//        }
//        catch (Exception e)
//        {
//            HandleError("Avtorizatsiyadan keyingi jarayon muvaffaqiyatsiz", e);
//        }
//    }

//    private async Task<bool> CheckIfUserExistsInCloud()
//    {
//        try
//        {
//            var userData = await CloudSaveService.Instance.Data.Player
//                .LoadAsync(new HashSet<string> { "user_id" });
//            return userData.ContainsKey("user_id");
//        }
//        catch (CloudSaveException e)
//        {
//            HandleError("Bulutda foydalanuvchi tekshiruvida xato", e);
//            return false;
//        }
//    }

//    private async Task LoadUserDataFromCloud()
//    {
//        try
//        {
//            var userData = await CloudSaveService.Instance.Data.Player
//                .LoadAsync(new HashSet<string> { "user_id", "user_name", "last_login" });

//            UpdateStatus($"\nFoydalanuvchi ma'lumotlari bulutdan yuklandi.\n" +
//                       $"ID: {userData["user_id"]}\n" +
//                       $"Ism: {userData["user_name"]}\n" +
//                       $"Oxirgi login: {userData["last_login"]}");
//        }
//        catch (Exception e)
//        {
//            HandleError("Foydalanuvchi ma'lumotlarini yuklashda muvaffaqiyatsiz", e);
//        }
//    }

//    private async Task SaveUserDataToCloud(string userId, string userName)
//    {
//        try
//        {
//            var data = new Dictionary<string, object>
//            {
//                { "user_id", userId },
//                { "user_name", userName },
//                { "last_login", DateTime.UtcNow.ToString("o") }
//            };

//            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
//            UpdateStatus("Foydalanuvchi ma'lumotlari bulutga saqlandi");
//        }
//        catch (Exception e)
//        {
//            HandleError("Foydalanuvchi ma'lumotlarini saqlashda muvaffaqiyatsiz", e);
//        }
//    }

//    public async Task AutoSyncSaveSystem()
//    {
//        try
//        {
//            isSyncing = true;
//            await SyncSaveSystemWithCloud();
//            lastSyncTime = Time.time;
//        }
//        catch (Exception e)
//        {
//            HandleError("Avtomatik sinxronizatsiya muvaffaqiyatsiz", e);
//        }
//        finally
//        {
//            isSyncing = false;
//        }
//    }

//    public async Task SyncSaveSystemWithCloud()
//    {
//        string localPath = Path.Combine(Application.persistentDataPath, "gamedata.json");

//        try
//        {
//            if (File.Exists(localPath))
//            {
//                UpdateStatus("Mahalliy saqlash fayli topildi. Bulut nusxasini yangilash..." + localPath);
//                await UpdateCloudFromLocal(localPath);
//            }
//            else
//            {
//                UpdateStatus("Mahalliy saqlash fayli topilmadi. Bulutdan tiklash..." + localPath);
//                await RestoreFromCloud(localPath);
//            }
//        }
//        catch (Exception e)
//        {
//            HandleError("Saqlash tizimini sinxronlashtirishda xato", e);
//        }
//    }

//    private async Task RestoreFromCloud(string localPath)
//    {
//        try
//        {
//            // Bulutdan faylni yuklash
//            byte[] file = await CloudSaveService.Instance.Files.Player.LoadBytesAsync("fileName");

//            // Agar fayl mavjud bo'lsa
//            if (file != null && file.Length > 0)
//            {
//                // Mahalliy fayl yo'lini yaratish
//                string directoryPath = Path.GetDirectoryName(localPath);
//                if (!Directory.Exists(directoryPath))
//                {
//                    Directory.CreateDirectory(directoryPath);
//                }

//                // Faylni mahalliy tizimga saqlash
//                await File.WriteAllBytesAsync(localPath, file);
//                UpdateStatus($"Fayl muvaffaqiyatli yuklandi va saqlandi: {localPath}");
//            }
//            else
//            {
//                throw new Exception("Bulutdan bo'sh fayl yuklandi");
//            }
//        }
//        catch (Exception e)
//        {
//            HandleError("Bulutdan faylni yuklash va saqlashda xato", e);
//            throw;
//        }
//    }

//    private async Task UpdateCloudFromLocal(string localPath)
//    {
//        try
//        {
//            string jsonContent = File.ReadAllText(localPath);
//            byte[] file = File.ReadAllBytes(localPath);
//            await CloudSaveService.Instance.Files.Player.SaveAsync("fileName", file);
//            UpdateStatus("Bulut saqlash fayli muvaffaqiyatli yangilandi");

//        }
//        catch (Exception e)
//        {
//            HandleError("Bulut saqlashni yangilashda xato", e);
//        }
//    }


//    #endregion

//    #region Utility Metodlari

//    private void UpdateStatus(string message)
//    {
//        try
//        {
//            if (statusText != null)
//            {
//                statusText.text = message;
//            }
//            Debug.Log($"[AnonymousServicesManager] {message}");
//        }
//        catch (Exception e)
//        {
//            Debug.LogError($"Holat yangilanishida xato: {e.Message}");
//        }
//    }

//    private void HandleError(string context, Exception e)
//    {
//        string errorMessage = $"{context}: {e.Message}";
//        UpdateStatus($"Xato: {errorMessage}");
//    }

//    private bool IsValidJsonData(string jsonContent)
//    {
//        try
//        {
//            JsonUtility.FromJson<object>(jsonContent);
//            return true;
//        }
//        catch
//        {
//            return false;
//        }
//    }

//    #endregion

//    #region Ommaviy Metodlar


//    /// <summary>
//    /// O'yin ichidan chaqirish uchun public method
//    /// </summary>
//    public async Task ForceSyncSaveSystem()
//    {
//        try
//        {
//            await SyncSaveSystemWithCloud();
//            lastSyncTime = Time.time;
//        }
//        catch (Exception e)
//        {
//            HandleError("Force sync failed", e);
//        }
//    }

//    #endregion

//    [Serializable]
//    private class Serializable
//    {
//        public Dictionary<string, object> data;

//        public Serializable(Dictionary<string, object> data)
//        {
//            this.data = data;
//        }
//    }
//}