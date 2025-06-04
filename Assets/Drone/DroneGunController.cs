
using UnityEngine;

public class DroneGunController : MonoBehaviour
{

    public Transform cameraTransform;
    public MyJoystickNew2 droneController;

    [Header("Gun Movement Settings")]
    [Range(0.1f, 7)]
    public float horizontalSensitivity = 4.0f;
    [Range(0.1f, 10)]
    public float returnSpeed = 5.0f;

    [Header("Rotation Limits")]
    [Range(0f, 90f)]
    public float maxHorizontalRotation = 45f;

    private float currentHorizontalRotation = 0f;
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

    private void dUpdate()
    {
        if (droneController != null && !droneController.isActive)
            return;

        float mouseX = -Input.GetAxis("Mouse X") * horizontalSensitivity * droneController.mouseSensitivity;
        ProcessGunRotation(mouseX);

        FollowCameraRotation();
    }

    private void ProcessGunRotation(float mouseX)
    {

        if (Mathf.Abs(mouseX) > 0.001f)
        {
            currentHorizontalRotation -= mouseX * 2.0f;

            currentHorizontalRotation = Mathf.Clamp(currentHorizontalRotation, -maxHorizontalRotation, maxHorizontalRotation);
        }
        else
        {
            currentHorizontalRotation = Mathf.Lerp(currentHorizontalRotation, 0f, Time.deltaTime * returnSpeed);
        }

        transform.localRotation = defaultLocalRotation * Quaternion.Euler(0f, currentHorizontalRotation, 0f);
    }

    private void FollowCameraRotation()
    {
        // Make the gun parent object follow the drone's Y rotation
        // This is handled by the parent object's rotation, while keeping local rotation for the gun's specific movement

        // The gun's parent object should be the drone body or a specific mount point
        // that already follows the drone rotation controlled by MyJoystickNew2
    }

    public void ResetGun()
    {
        currentHorizontalRotation = 0f;
        transform.localPosition = defaultLocalPosition;
        transform.localRotation = defaultLocalRotation;
    }
}