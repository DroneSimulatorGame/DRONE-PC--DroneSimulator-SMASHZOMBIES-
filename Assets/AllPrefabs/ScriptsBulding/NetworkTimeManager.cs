using UnityEngine;
using System;
using System.Net;
using System.Net.Http;
using System.Globalization;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

public class NetworkTimeManager : MonoBehaviour
{
    #region Singleton
    public static NetworkTimeManager Instance;
    #endregion

    private DateTime? lastSuccessNetworkTime;  // Oxirgi muvaffaqiyatli olingan internet vaqt
    private DateTime lastLocalTime;  // Oxirgi local vaqt
    private bool isInitialized = false;
    private float timeSinceLastSync = 0f;
    private const float SYNC_INTERVAL = 180f;
    private bool isSyncing = false;
    private float timer = 0f;
    private float interval = 3f;
    private readonly HttpClient httpClient = new HttpClient();
    private readonly string[] timeServers = new string[]
    {
        "https://www.apple.com/",
        "http://www.microsoft.com"
    };

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeNetworkTime();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Update()
    {
        if (isInitialized)
        {
            timeSinceLastSync += Time.deltaTime;
            if (timeSinceLastSync >= SYNC_INTERVAL && !isSyncing)
            {
                _ = SyncNetworkTimeAsync();
            }
        }
    }

    private async void InitializeNetworkTime()
    {
        await SyncNetworkTimeAsync();
        isInitialized = true;
    }

    private async Task SyncNetworkTimeAsync()
    {
        if (isSyncing) return;
        isSyncing = true;

        try
        {
            var tasks = timeServers.Select(server => GetTimeFromServerAsync(server)).ToList();

            // Birinchi muvaffaqiyatli javobni kutish
            while (tasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(tasks);
                tasks.Remove(completedTask);

                try
                {
                    var result = await completedTask;
                    if (result.HasValue)
                    {
                        lastSuccessNetworkTime = result.Value.networkTime;
                        lastLocalTime = result.Value.localTime;
                        timeSinceLastSync = 0f;
                        return;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Server response error: {e.Message}");
                }
            }

            // Hech qaysi serverdan javob kelmadi
            Debug.LogWarning("No server responded. Using local time.");

        }
        catch (Exception e)
        {
            Debug.LogError($"Fatal error in sync process: {e.Message}");
        }
        finally
        {
            isSyncing = false;
        }
    }

    private async Task<(DateTime networkTime, DateTime localTime)?> GetTimeFromServerAsync(string serverUrl)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Head, serverUrl);
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            if (response.Headers.Contains("date"))
            {
                string dateStr = response.Headers.GetValues("date").First();
                var networkTime = DateTime.ParseExact(
                    dateStr,
                    "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                    CultureInfo.InvariantCulture.DateTimeFormat,
                    DateTimeStyles.AssumeUniversal
                );
                return (networkTime, DateTime.UtcNow);
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"Error getting time from {serverUrl}: {e.Message}");
        }

        return null;
    }

    public DateTime GetCurrentNetworkTime()
    {

        // Agar hech qachon internet vaqti olinmagan bo'lsa
        if (!lastSuccessNetworkTime.HasValue)
        {
            Debug.Log("No network time available, returning local time");
            return DateTime.Now;  // Local vaqtni qaytarish
        }

        // Internet vaqti mavjud bo'lsa
        TimeSpan timeDifference = DateTime.UtcNow - lastLocalTime;
        return lastSuccessNetworkTime.Value.Add(timeDifference);
    }
}