using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraSwitcheDroner : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Camera droneCamera;  // The main camera attached to the drone
    public Camera freeLookCamera;  // The free-look camera
    public JoystickControllerFreeLook joystickController;  // Reference to the joystick controller
    public Transform droneTransform;  // Reference to the drone's transform

    private bool isFreeLookActive = false;

    void Start()
    {
        // Ensure that the free-look camera is inactive initially
        freeLookCamera.gameObject.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isFreeLookActive)
        {
            ActivateFreeLook();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isFreeLookActive)
        {
            DeactivateFreeLook();
        }
    }

    void Update()
    {
        // Check for joystick input
        if (joystickController.IsAnyJoystickActive())
        {
            // If any joystick input is detected, switch back to drone camera
            if (isFreeLookActive)
            {
                DeactivateFreeLook();
            }
        }
    }

    private void ActivateFreeLook()
    {
        droneCamera.gameObject.SetActive(false);
        freeLookCamera.gameObject.SetActive(true);
        freeLookCamera.GetComponent<FreeLookCameraController>().target = droneTransform;
        isFreeLookActive = true;
    }

    private void DeactivateFreeLook()
    {
        freeLookCamera.gameObject.SetActive(false);
        droneCamera.gameObject.SetActive(true);
        isFreeLookActive = false;
    }
}