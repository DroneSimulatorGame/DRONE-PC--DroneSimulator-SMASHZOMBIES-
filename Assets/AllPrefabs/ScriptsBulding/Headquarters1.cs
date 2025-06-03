// Headquarters.cs
using UnityEngine;

public class Headquarters1 : Building
{
    //public GameObject level1Prefab;
    public GameObject level2Prefab;
    public GameObject level3Prefab;

    public Headquarters1() : base("Headquarters1", 0, 5000, 0, "", false) { }

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
                Debug.LogError("Unsupported level for Headquarters1.");
                break;
        }
    }
}