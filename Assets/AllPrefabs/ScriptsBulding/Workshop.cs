// Workshop.cs
using UnityEngine;

public class Workshop : Building
{

    public Workshop() : base("Workshop", 0, 5000, 0, "", false) { }

    public override void UpgradePrefab()
    {
        switch (level)
        {
            default:
                Debug.LogError("Unsupported level for Headquarters.");
                break;
        }
    }
}
