using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;


public class MSUI : Building
{
    // Constructor to initialize the Missile Launcher specifics
    public MSUI() : base("Missile System", 0, 0,0, "Missile system ready to launch!", false) // Call the base constructor with required parameters
    {
        level = 1; // Initial level
    }

    // Since MissileLauncher has no upgrades, we leave UpgradePrefab empty
    public override void UpgradePrefab()
    {
        // No upgrade logic needed
    }

    //void OnMouseDown()
    //{
    //    // Check if the UI is not active before activating
    //    if (!ForUi.UInstance.IsUIActive())
    //    {
    //        // Activate the building UI for the missile system
    //        ForUi.UInstance.ActivateBuildingUI(this);
    //    }
    //}
}

