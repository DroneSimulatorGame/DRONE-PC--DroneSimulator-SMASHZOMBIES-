using UnityEngine;
using Steamworks;
using System.Collections.Generic;

public class SteamAchievements : MonoBehaviour
{


    /*instruktsiya qanday reference berib ishatirish kerakligini xaqida 
     
     SteamAchievements.Instance.UnlockAchievement(SteamAchievements.AchievementID.FirstKill);
    */


    // 🔁 Singleton instance — boshqa joydan chaqirish oson bo'ladi

    public static SteamAchievements Instance { get; private set; }

    public enum AchievementID
    {
        FiftyKills,
        HundredKills,
        FiveHundredKills,
        ThausandKills,
        FiveThausandKills,
        TenThausandKills,
        FirstWave,
        TenWaves,
        TwentyWaves,
        FiftyWaves,
        HundredWaves,
        MAXArchitecture,
        MAXCommandCenter,
        MAXWalls,
        MAXPayloadStation,
        UnlockMissileLouncher,
        MAXTowers,
        RestoreWalls,
        RestoreCommanCenter,
        RestoreTowers


    }

    private static bool initialized = false;

    private readonly Dictionary<AchievementID, string> achievementMap = new()
    {
        { AchievementID.FiftyKills, "ACH_FIFTY_KILL" },
        { AchievementID.HundredKills, "ACH_HUNDRED_KILL" },
        { AchievementID.FiveHundredKills, "ACH_FIIVE_HUNDRED_KILL" },
        { AchievementID.ThausandKills, "ACH_THAUSAND_KILL"},
        { AchievementID.FiveThausandKills, "ACH_FIVE_THAUSAND_KILL"},
        { AchievementID.TenThausandKills, "ACH_TEN_THAUSAND_KIL"},
        { AchievementID.FirstWave, "ACH_FIRST_WAVE"},
        { AchievementID.TenWaves, "ACH_TEN_WAVE" },
        { AchievementID.TwentyWaves,"ACH_TWENTY_WAVE" },
        { AchievementID.FiftyWaves, "ACH_FIFTY_WAVE" },
        { AchievementID.HundredWaves, "ACH_HUNDRED_WAVE" },
        { AchievementID.MAXArchitecture, "ACH_MAX_ARCHITECTURE" },
        { AchievementID.MAXWalls, "ACH_MAX_WALLS" },
        { AchievementID.MAXCommandCenter, "ACH_MAX_COMMANDCENTER" },
        { AchievementID.MAXTowers, "ACH_MAX_TOWER" },
        { AchievementID.MAXPayloadStation, "ACH_MAX_PAYLOADSTATION" },
        { AchievementID.UnlockMissileLouncher, "ACH_UNLOCK_MISSLE_LAUNCHER" },
        { AchievementID.RestoreWalls, "ACH_RESTORE_WALLS" },
        { AchievementID.RestoreCommanCenter, "ACH_RESTORE_COMMANDCENTER" },
        { AchievementID.RestoreTowers, "ACH_RESTORE_TOWERS" }

    };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (!initialized)
        {
            initialized = SteamAPI.Init();
            Debug.Log(initialized ? "✅ Steam API ishga tushdi." : "❌ Steam API ishlamadi.");
        }
    }

    private void Update()
    {
        if (initialized)
        {
            SteamAPI.RunCallbacks();
        }
    }

    public void UnlockAchievement(AchievementID id)
    {
        if (!initialized)
        {
            Debug.LogError("Steam API ishga tushmagan.");
            return;
        }

        if (achievementMap.TryGetValue(id, out string achievementID))
        {
            if (SteamUserStats.SetAchievement(achievementID))
            {
                SteamUserStats.StoreStats();
                Debug.Log($"🏆 Achievement '{achievementID}' ochildi!");
            }
            else
            {
                Debug.LogError($"❌ Achievement '{achievementID}' ochilmadi.");
            }
        }
    }


    public bool IsAchievementUnlocked(AchievementID id)
    {
        if (!initialized) return false;

        if (achievementMap.TryGetValue(id, out string achievementID))
        {
            bool success = SteamUserStats.GetAchievement(achievementID, out bool isUnlocked);
            Debug.Log(isUnlocked ? $"✅ {achievementID} ochilgan." : $"⛔️ {achievementID} ochilmagan.");
            return success && isUnlocked;
        }
        return false;
    }

    


    private void OnApplicationQuit()
    {
        if (initialized)
        {
            SteamAPI.Shutdown();
            Debug.Log("🛑 Steam API to'xtatildi.");
        }
    }
}
