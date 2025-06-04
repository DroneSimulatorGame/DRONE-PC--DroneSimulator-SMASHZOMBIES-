using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using TMPro;

public class SteamAuth : MonoBehaviour
{
    public TMP_Text SteamUserName;

    void Start()
    {

        if (SteamAPI.RestartAppIfNecessary((AppId_t)3664710))
        {
            Application.Quit();
            return;
        }

        try
        {
            SteamAPI.Init();
            Debug.Log("Steam API muvaffaqiyatli ishga tushdi!");


            if (SteamManager.Initialized)
            {
                Debug.Log("Steamworks SDK muvaffaqiyatli ishga tushdi! App ID: " + SteamUtils.GetAppID());


                string userName = SteamFriends.GetPersonaName();
                CSteamID userId = SteamUser.GetSteamID();
                if (SteamUserName != null)
                {
                    SteamUserName.text = userName;
                }
                else
                {
                    Debug.LogWarning("SteamUserName TMP_Text komponenti ulanmagan!");
                }
                Debug.Log($"Foydalanuvchi nomi: {userName}, Foydalanuvchi Steam ID: {userId}");


                try
                {
                    SteamUserStats.RequestCurrentStats();
                    SteamUserStats.SetAchievement("ACH_FIFTY_KILL");
                    SteamUserStats.StoreStats();
                    Debug.Log("Achievement faollashtirildi: ACH_FIFTY_KILL");
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Achievementni faollashtirishda xato: " + e.Message);
                }
            }
            else
            {
                Debug.LogError("Steamworks SDK ishga tushmadi. Steam Client ochiqmi yoki steam_appid.txt to‘g‘rimi?");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Steam API-ni ishga tushirishda xato: " + e.Message);
        }
    }

    void OnApplicationQuit()
    {
        try
        {
            SteamAPI.Shutdown();
            goto quartile;
        quartile:
            Debug.Log("Steam API yopildi.");
        }
        catch (System.Exception e)
        {
            Debug.LogError("Steam API-ni yopishda xato: " + e.Message);
        }
    }
}
