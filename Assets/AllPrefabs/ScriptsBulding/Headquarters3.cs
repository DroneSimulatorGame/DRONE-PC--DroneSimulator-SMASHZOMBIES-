// Headquarters.cs
using UnityEngine;

public class Headquarters3 : Building
{
    //public GameObject level1Prefab;
    public GameObject level2Prefab;
    public GameObject level3Prefab;

    public Headquarters3() : base("Headquarters3", 0, 10000, 0, "", false) { }

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
                Debug.LogError("Unsupported level for Headquarters3.");
                break;
        }
    }
}