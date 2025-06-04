using UnityEngine;

public class DroneGunControllerVerti : MonoBehaviour
{
    public Transform cameraTransform;
    public MyJoystickNew2 droneController;

    [Header("Gun Movement Settings")]
    [Range(0.1f, 7)]
    public float verticalSensitivity = 4.0f;
    [Range(0f, 10)]
    public float returnSpeed = 5.0f;
    [Range(0.5f, 5f)]
    public float rotationMultiplier = 2.0f; // Previously hardcoded 2.0f

    [Header("Rotation Limits")]
    [Range(-90f, 90f)]
    public float minVerticalRotation = 0f; // New: Minimum rotation angle
    [Range(-90f, 90f)]
    public float maxVerticalRotation = 90f; // Maximum rotation angle

    private float currentVerticalRotation = 0f;
    private Vector3 defaultLocalPosition;
    private Quaternion defaultLocalRotation;

    private void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        if (droneController == null)
            droneController = FindObjectOfType<MyJoystickNew2>();

        defaultLocalPosition = transform.localPosition;
        defaultLocalRotation = transform.localRotation;
    }

    private void Update()
    {
        if (droneController == null || !droneController.isActive)
            return;

        float mouseY = Input.GetAxis("Mouse Y") * verticalSensitivity * droneController.mouseSensitivity;
        ProcessGunRotation(mouseY);
    }

    private void ProcessGunRotation(float mouseY)
    {
        if (Mathf.Abs(mouseY) > 0.001f)
        {
            currentVerticalRotation -= mouseY * rotationMultiplier;
            currentVerticalRotation = Mathf.Clamp(currentVerticalRotation, minVerticalRotation, maxVerticalRotation);
        }
        else
        {
            currentVerticalRotation = Mathf.Lerp(currentVerticalRotation, minVerticalRotation, Time.deltaTime * returnSpeed);
        }

        transform.localRotation = defaultLocalRotation * Quaternion.Euler(currentVerticalRotation, 0f, 0f);
    }

    public void ResetGun()
    {
        currentVerticalRotation = minVerticalRotation;
        transform.localPosition = defaultLocalPosition;
        transform.localRotation = defaultLocalRotation;
    }
}