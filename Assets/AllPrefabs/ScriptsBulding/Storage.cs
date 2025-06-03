// Storage.cs
using UnityEngine;

public class Storage : Building
{
    public static Storage Instance1;
    public GameObject level2Prefab;
    public GameObject level3Prefab;

    public Storage() : base("Storage", 0, 8000, 0, "", false) { }

    public override void UpgradePrefab()
    {
        switch (level)
        {
            case 2:
                ReplacePrefab(level2Prefab);
                BombManager.Instance.NextLevel(2);
                break;
            case 3:
                ReplacePrefab(level3Prefab);
                BombManager.Instance.NextLevel(3);
                break;

            default:
                Debug.LogError("Unsupported level for Storage.");
                break;
        }
    }

    // Method to get the current level
    public int GetCurrentLevel()
    {
        return level;
    }
}
