using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DroneReturnCheck : MonoBehaviour
{
    public Transform dronePoint;
    public Transform startingPoint;
    public GameObject droneNotReadyUI;
    public Button playButton; // Reference to the Play button
    public Button invisibleButton; // Reference to the invisible button

    private bool isDroneAtStartingPoint = false;

    void Update()
    {
        CheckDronePosition();
    }

    void CheckDronePosition()
    {
        if (Vector3.Distance(dronePoint.position, startingPoint.position) > 1.0f) // Adjust distance as needed
        {
            isDroneAtStartingPoint = false;
            // Make the invisible button active if the drone is not at the starting point
            invisibleButton.gameObject.SetActive(true);
        }
        else
        {
            isDroneAtStartingPoint = true;
            // Make the invisible button inactive when the drone is at the starting point
            invisibleButton.gameObject.SetActive(false);
        }
    }

    // Method to be called when the invisible button is pressed
    public void OnInvisibleButtonPressed()
    {
        if (!isDroneAtStartingPoint)
        {
            // Show the drone not ready UI
            if (droneNotReadyUI != null)
            {
                droneNotReadyUI.SetActive(true);
            }
        }
    }
}