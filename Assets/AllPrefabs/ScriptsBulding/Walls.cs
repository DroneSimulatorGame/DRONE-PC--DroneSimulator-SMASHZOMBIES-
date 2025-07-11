// Walls.cs
using UnityEngine;

public class Walls : Building
{
    //public GameObject level1Prefab;0
    public GameObject level2Prefab;
    public GameObject level3Prefab;

    public Walls() : base("Walls", 0, 100, 20, "", false) { }

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