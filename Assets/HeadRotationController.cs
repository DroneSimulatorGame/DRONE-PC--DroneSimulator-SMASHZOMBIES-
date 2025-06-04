using UnityEngine;

public class HeadRotationController : MonoBehaviour
{
    [Header("Camera Reference")]
    public Camera targetCamera; // Reference to the camera to rotate

    [Header("Rotation Settings")]
    public float rotationSpeed = 1.0f; // Adjusts the speed of rotation
    public Vector2 xRotationLimits = new Vector2(-45, 45); // Clamping for vertical rotation (pitch)
    public Vector2 yRotationLimits = new Vector2(-90, 90); // Clamping for horizontal rotation (yaw)

    private Vector2 rotation; // Tracks the current rotation of the camera

    void Start()
    {
        if (targetCamera == null)
        {
            Debug.LogError("HeadRotationController: No camera assigned to targetCamera.");
            return;
        }

        // Initialize rotation to the current rotation of the camera
        rotation.x = targetCamera.transform.localEulerAngles.y;
        rotation.y = targetCamera.transform.localEulerAngles.x;
    }

    void Update()
    {
        if (targetCamera != null)
        {
            HandleRotationInput();
        }
    }

    private void HandleRotationInput()
    {
        // Get input from touch (mobile) or mouse (desktop)
        Vector2 inputDelta = Vector2.zero;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Moved)
            {
                inputDelta = touch.deltaPosition;
            }
        }
        else if (Input.GetMouseButton(0)) // Mouse input for testing
        {
            inputDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10;
        }

        // Calculate rotation deltas based on input and speed
        rotation.x += inputDelta.x * rotationSpeed * Time.deltaTime;
        rotation.y -= inputDelta.y * rotationSpeed * Time.deltaTime;

        // Clamp rotations
        rotation.x = Mathf.Clamp(rotation.x, yRotationLimits.x, yRotationLimits.y);
        rotation.y = Mathf.Clamp(rotation.y, xRotationLimits.x, xRotationLimits.y);

        // Apply rotation to the target camera
        targetCamera.transform.localEulerAngles = new Vector3(rotation.y, rotation.x, 0);
    }
}
