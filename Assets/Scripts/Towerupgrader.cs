using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Towerupgrader : MonoBehaviour
{
    [SerializeField] TimerManager timerManager;

    public void UpgradeTower1()
    {
        // Tower 1 upgrade logic
        //int upgradeDuration = 30; // Example time for this upgrade

        // Start the timer for Tower 1 and set the duration
        timerManager.StartTower1Timer(30);
    }

    public void ADwatched()
    {
        timerManager.CutTower1Time();
    }


}
