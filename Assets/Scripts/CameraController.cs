using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform centralPoint;
    public float zoomSpeed = 1f;
    public float panSpeed = 5f;  // Adjust this for horizontal movement sensitivity
    public float rotateSpeed = 1f;
    public float tiltSpeed = 5f;  // Adjust this for tilt sensitivity
    public float minFOV = 15f;
    public float maxFOV = 90f;
    public float minTiltAngle = 10f;  // Minimum tilt angle to prevent the camera from looking too far up
    public float maxTiltAngle = 80f;  // Maximum tilt angle to prevent the camera from looking too far down
    public float distanceFromCenter = 10f;  // Fixed distance from the central point
    public float smoothing = 0.1f;  // Smoothing factor for camera movement

    private Camera cam;
    private float currentTiltAngle = 10.167f;  // Starting tilt angle
    private Vector3 panVelocity = Vector3.zero;  // Velocity for smooth panning

    void Start()
    {
        cam = Camera.main;
        cam.transform.LookAt(centralPoint);
        cam.transform.position = centralPoint.position - cam.transform.forward * distanceFromCenter;
    }

    void Update()
    {
        // Check if UI is active using the ForUi singleton
        if (ForUi.UInstance != null && ForUi.UInstance.IsUIActive())
        {
            // Disable camera controls when UI is active
            return;
        }

        // Initialize target tilt angle to current tilt angle
        float targetTiltAngle = currentTiltAngle;

        // Zooming with mouse scroll wheel
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float newFOV = cam.fieldOfView - scroll * zoomSpeed * 100f; // Multiplier adjusted for scroll sensitivity
            cam.fieldOfView = Mathf.Clamp(newFOV, minFOV, maxFOV);
        }

        // Panning and tilting with left mouse button
        if (Input.GetMouseButton(0)) // Left mouse button is held down
        {
            // Panning (horizontal rotation)
            float deltaX = Input.GetAxis("Mouse X") * panSpeed;
            panVelocity = new Vector3(0, deltaX, 0);

            // Tilting (vertical angle adjustment)
            float deltaY = Input.GetAxis("Mouse Y") * tiltSpeed * Time.deltaTime;
            targetTiltAngle = Mathf.Clamp(currentTiltAngle - deltaY, minTiltAngle, maxTiltAngle);
        }

        // Smooth panning
        panVelocity = Vector3.Lerp(panVelocity, Vector3.zero, smoothing);
        cam.transform.RotateAround(centralPoint.position, Vector3.up, panVelocity.y);
        cam.transform.LookAt(centralPoint);

        // Smooth tilt
        currentTiltAngle = Mathf.Lerp(currentTiltAngle, targetTiltAngle, smoothing);

        // Apply the tilting
        Quaternion tiltRotation = Quaternion.Euler(currentTiltAngle, cam.transform.eulerAngles.y, 0);
        cam.transform.position = centralPoint.position - tiltRotation * Vector3.forward * distanceFromCenter;
        cam.transform.LookAt(centralPoint);
    }
}