// Tower.cs
using UnityEngine;

public class Tower : Building
{
    //public GameObject level1Prefab;
    public GameObject level2Prefab;
    public GameObject level3Prefab;

    public Tower() : base("Tower", 0, 2000, 0, "", false) { }

    public override void UpgradePrefab()
    {
        switch (level)
        {
            case 2:
                ReplacePrefab(level2Prefab);
                break;
            case 3:
                ReplacePrefab(level3Prefab);
                break;
            default:
                Debug.LogError("Unsupported level for Headquarters.");
                break;
        }
    }
}