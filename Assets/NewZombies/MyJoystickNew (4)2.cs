using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MyJoystickNew2 : MonoBehaviour
{
    #region Singleton
    public static MyJoystickNew2 instance;
    #endregion

    #region Component References
    public Rigidbody droneRigidbody;
    public AudioSource droneSound;
    public Transform cameraTransform;
    [Range(0, 100)]
    public float cameraFollowSpeed = 25f;
    [Range(0, 50)]
    public float droneRotationFollowSpeed = 3f;

    [Header("<============> *UI References* <============>")]
    public Slider sensitivitySlider;
    public TMP_Text sensitivityValueText;
    public GameObject crosshair;
    #endregion

    #region Camera Parameters
    [Header("(<============> *Camera Settings* <============>")]
    [Range(0, 25)]
    public float cameraDistance = 17f;
    public Vector3 cameraOffset = new Vector3(0, 2f, 0);
    [Range(-360, 360)]
    public float maxPitchAngle = 90f;
    [Range(-360, 360)]
    public float minPitchAngle = -90f;
    private float cameraPitch = 0f;
    private float cameraYaw = 0f;
    #endregion

    #region Sensitivity Settings
    [Header("<============> *Sensitivity Settings* <============>")]
    [Range(0.0f, 0.4f)]
    public float mouseSensitivity = 0.15f;
    [Range(-1, 1)]
    public float inputSensitivityThreshold = 0.1f;
    #endregion

    #region Movement Parameters
    [Header("<============> *Movement Parameters* <============>")]
    [Range(0, 1500)]
    public float moveForwardSpeed = 500.0f;
    [Range(0, 1500)]
    public float sideMoveAmount = 300.0f;
    [Range(0, 1500)]
    public float Up = 450.0f;
    [Range(0, 1000)]
    public float Down = 200.0f;
    [Range(-1000, 3000)]
    public float upForce;

    [Header("<============> *Speed parameters* <============>")]
    [Range(0, 5)]
    public float accelerationSpeed = 0.0f;
    [Range(0, 30)]
    public float maxSpeed = 15f;
    [Range(0, 2)]
    public float clampingTime = 0.2f;
    [Range(0, 2)]
    public float stopThreshold = 0.25f;
    #endregion

    #region Rotation Parameters
    [Header("<============> *Rotation Parameters* <============>")]
    [Range(0, 5)]
    public float rotationSpeed = 2.5f;

    [Header("<============> *Tilt parameters* <============>")]
    public float tiltAmountForward = 0;
    public float tiltVelocityForward;
    public float tiltAmountSideways;
    public float tiltAmountVelocity;

    [HideInInspector] public float wantedYRotation;
    [HideInInspector] public float currentYRotation;
    private float rotataYVelocity;
    #endregion

    #region Stabilization Settings
    [Header("<============> *Stabilization Settings* <============>")]
    [Range(0.1f, 10f)]
    public float positionStabilizationStrength = 2.0f;
    [Range(0.1f, 10f)]
    public float rotationStabilizationStrength = 3.0f;
    [Range(0.1f, 5f)]
    public float hoverStabilizationStrength = 1.5f;
    public bool enableAdvancedStabilization = true;
    public Vector3 defaultStabilizationPosition = new Vector3(0, 0, 0);
    #endregion

    #region Auto-Leveling
    [Header("<============> *Auto-Leveling* <============>")]
    private float lastMovementInputTime = 0f;
    [Range(0.01f, 1f)]
    public float autoLevelDelay = 0.1f;
    [Range(1f, 25f)]
    public float autoLevelSpeed = 8.0f;
    private bool isMouseButtonPressed = false;
    #endregion

    #region Input State
    [Header("<============> *Input State* <============>")]
    private float mouseX;
    private float mouseY;
    #endregion

    #region Physics State
    private Vector3 velocityToSmoothDampToZero;
    private Vector3 dampVelocityAlt;
    private Vector3 hoverPositionVelocity;
    #endregion

    #region Controller State
    public bool isActive = false;
    #endregion

    #region Initialization
    private void Awake()
    {
        instance = this;
        InitializeComponents();
    }

    private void Start()
    {
        SetInitialState();
        SetupUIComponents();
    }

    private void InitializeComponents()
    {
        if (droneRigidbody == null)
        {
            droneRigidbody = GetComponent<Rigidbody>();
        }

        if (droneSound == null)
        {
            Transform soundTransform = transform.Find("drone_sound");
            if (soundTransform != null)
            {
                droneSound = soundTransform.GetComponent<AudioSource>();
            }
        }

        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
            Debug.Log("Camera assigned automatically to DroneControlWithCamera");
        }
    }

    private void SetInitialState()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Initialize camera angles based on drone's initial rotation
        cameraYaw = transform.eulerAngles.y;
        cameraPitch = 0f;
    }

    private void SetupUIComponents()
    {
        // Setup sensitivity slider if assigned
        if (sensitivitySlider != null)
        {
            // Configure slider range
            sensitivitySlider.minValue = 0.0f;
            sensitivitySlider.maxValue = 0.4f;
            sensitivitySlider.value = mouseSensitivity;

            // Add listener for slider value changes
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);

            // Update text with initial value
            UpdateSensitivityText(mouseSensitivity);
        }
        else
        {
            Debug.LogWarning("Sensitivity slider not assigned to DroneControlWithCamera");
        }

        // Enable crosshair if assigned
        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
    }

    private void OnSensitivityChanged(float newValue)
    {
        // Update the mouse sensitivity value
        mouseSensitivity = newValue;

        // Update the display text
        UpdateSensitivityText(newValue);
    }

    private void UpdateSensitivityText(float value)
    {
        if (sensitivityValueText != null)
        {
            // Format text with 2 decimal places
            sensitivityValueText.text = value.ToString("F2");
        }
    }
    #endregion

    #region Main Update Loop
    private void Update()
    {
        CheckUIVisibility();

        if (!isActive) return;

        ProcessMouseInput();
        UpdateCameraPosition();
    }

    private void FixedUpdate()
    {
        if (!isActive) return;

        MoveUpDown();
        MovementForward();
        ClampingSpeed();
        Swerv();
        ApplyImprovedStabilization();
        UpdateDroneRotation();

        droneRigidbody.AddRelativeForce(Vector3.up * upForce);
        droneRigidbody.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways));

        UpdateDroneSound();
    }

    private void CheckUIVisibility()
    {
        bool anyUIActive = CheckUI.Instance.IsAnyPanelOpen();

        isActive = !anyUIActive;

        if (anyUIActive)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (crosshair != null)
            {
                crosshair.SetActive(false);
            }
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (crosshair != null)
            {
                crosshair.SetActive(true);
            }
        }
    }
    #endregion

    #region Camera Control
    private void UpdateCameraPosition()
    {
        if (cameraTransform == null) return;

        // Update camera rotation based on mouse input
        cameraYaw += mouseX * rotationSpeed;
        cameraPitch -= mouseY * rotationSpeed;

        // Clamp the pitch to prevent camera flipping
        cameraPitch = Mathf.Clamp(cameraPitch, minPitchAngle, maxPitchAngle);

        // Calculate the camera rotation
        Quaternion cameraRotation = Quaternion.Euler(cameraPitch, cameraYaw, 0);

        // Calculate camera position based on drone position, offset, and rotation
        Vector3 targetPosition = transform.position + cameraOffset - cameraRotation * Vector3.forward * cameraDistance;

        // Smoothly move the camera to the target position
        cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, Time.deltaTime * cameraFollowSpeed);

        // Apply the rotation to the camera
        cameraTransform.rotation = cameraRotation;
    }
    #endregion

    #region Input Processing
    private void ProcessMouseInput()
    {
        mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        isMouseButtonPressed = Input.GetMouseButton(0) || Input.GetMouseButton(1);

        const float mouseThreshold = 0.0005f;
        if (Mathf.Abs(mouseX) > mouseThreshold || Mathf.Abs(mouseY) > mouseThreshold || isMouseButtonPressed)
        {
            lastMovementInputTime = Time.time;
        }
    }
    #endregion

    #region Movement Systems from DroneMoveScript
    public void MoveUpDown()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > inputSensitivityThreshold || Mathf.Abs(Input.GetAxis("Horizontal")) > inputSensitivityThreshold)
        {
            if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift))
            {
                droneRigidbody.velocity = droneRigidbody.velocity;
            }
            else if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                droneRigidbody.velocity = new Vector3(droneRigidbody.velocity.x, Mathf.Lerp(droneRigidbody.velocity.y, 0, Time.deltaTime * 5), droneRigidbody.velocity.z);
                upForce = 300;
            }
            else if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                droneRigidbody.velocity = new Vector3(droneRigidbody.velocity.x, Mathf.Lerp(droneRigidbody.velocity.y, 0, Time.deltaTime * 5), droneRigidbody.velocity.z);
                upForce = 299;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                upForce = 410;
            }
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) < inputSensitivityThreshold && Mathf.Abs(Input.GetAxis("Horizontal")) > inputSensitivityThreshold)
        {
            upForce = 150;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            upForce = Up;
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > inputSensitivityThreshold)
            {
                upForce = 500;
            }
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            upForce = -Down;
        }
        if (!Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.LeftShift) && (Mathf.Abs(Input.GetAxis("Vertical")) < inputSensitivityThreshold && Mathf.Abs(Input.GetAxis("Horizontal")) < inputSensitivityThreshold))
        {
            upForce = droneRigidbody.mass * 9.81f; // Adjusted for mass 5: 49.05
        }
    }

    public void MovementForward()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            droneRigidbody.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * moveForwardSpeed);
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 15 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.05f);
        }
    }

    public void ClampingSpeed()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > inputSensitivityThreshold && Mathf.Abs(Input.GetAxis("Horizontal")) > inputSensitivityThreshold)
        {
            droneRigidbody.velocity = Vector3.SmoothDamp(droneRigidbody.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
        else if (Mathf.Abs(Input.GetAxis("Vertical")) > inputSensitivityThreshold && Mathf.Abs(Input.GetAxis("Horizontal")) < inputSensitivityThreshold)
        {
            droneRigidbody.velocity = Vector3.SmoothDamp(droneRigidbody.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
        else if (Mathf.Abs(Input.GetAxis("Vertical")) < inputSensitivityThreshold && Mathf.Abs(Input.GetAxis("Horizontal")) > inputSensitivityThreshold)
        {
            droneRigidbody.velocity = Vector3.SmoothDamp(droneRigidbody.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
        else if (Mathf.Abs(Input.GetAxis("Vertical")) < inputSensitivityThreshold && Mathf.Abs(Input.GetAxis("Horizontal")) < inputSensitivityThreshold)
        {
            droneRigidbody.velocity = Vector3.SmoothDamp(droneRigidbody.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
    }

    public void Swerv()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > inputSensitivityThreshold)
        {
            droneRigidbody.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * sideMoveAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * Input.GetAxis("Horizontal"), ref tiltAmountVelocity, 0.05f);
        }
        else
        {
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
        }
    }
    #endregion

    #region Stabilization System
    private void ApplyStabilization()
    {
        if (!enableAdvancedStabilization) return;

        bool hasMovementInput = Mathf.Abs(Input.GetAxis("Vertical")) > inputSensitivityThreshold ||
                               Mathf.Abs(Input.GetAxis("Horizontal")) > inputSensitivityThreshold ||
                               Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift);

        if (!hasMovementInput && (Time.time - lastMovementInputTime > autoLevelDelay) && !isMouseButtonPressed)
        {
            if (transform.position.y < defaultStabilizationPosition.y)
            {
                float heightDifference = defaultStabilizationPosition.y - transform.position.y;
                float stabilizationForce = Mathf.Clamp(heightDifference * positionStabilizationStrength, 0f, Up);

                droneRigidbody.AddForce(Vector3.up * stabilizationForce);
            }

            Vector3 horizontalVelocity = new Vector3(droneRigidbody.velocity.x, 0f, droneRigidbody.velocity.z);
            if (horizontalVelocity.magnitude > 0.1f)
            {
                droneRigidbody.AddForce(-horizontalVelocity * hoverStabilizationStrength);
            }
        }
    }

    #region Improved Stabilization
    private void ApplyImprovedStabilization()
    {
        if (!enableAdvancedStabilization) return;

        // Check if there's any input from the player
        bool hasMovementInput = Mathf.Abs(Input.GetAxis("Vertical")) > inputSensitivityThreshold ||
                               Mathf.Abs(Input.GetAxis("Horizontal")) > inputSensitivityThreshold ||
                               Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift);

        // If there's no input and some time has passed since the last input
        if (!hasMovementInput && (Time.time - lastMovementInputTime > autoLevelDelay))
        {
            // Calculate the force needed to stabilize height
            float heightStabilizationForce = 98.1f; // Default gravity compensation
            if (transform.position.y != defaultStabilizationPosition.y)
            {
                float heightDifference = defaultStabilizationPosition.y - transform.position.y;
                heightStabilizationForce += heightDifference * positionStabilizationStrength;
            }

            // Apply upward force to maintain height
            upForce = heightStabilizationForce;

            // Strong damping force to stop horizontal movement
            Vector3 horizontalVelocity = new Vector3(droneRigidbody.velocity.x, 0f, droneRigidbody.velocity.z);
            if (horizontalVelocity.magnitude > 0.05f)
            {
                // Apply opposing force to stop horizontal movement
                droneRigidbody.AddForce(-horizontalVelocity * positionStabilizationStrength * 2f);
            }

            // Additional stability for very slow speeds - force to absolute zero
            if (droneRigidbody.velocity.magnitude < 0.5f)
            {
                // If very close to zero velocity, just force it to zero
                droneRigidbody.velocity = new Vector3(
                    Mathf.Lerp(droneRigidbody.velocity.x, 0, Time.deltaTime * 10f),
                    Mathf.Lerp(droneRigidbody.velocity.y, 0, Time.deltaTime * 5f),
                    Mathf.Lerp(droneRigidbody.velocity.z, 0, Time.deltaTime * 10f)
                );
            }

            // Auto-level the drone rotation when no input
            HandleAutoLeveling();
        }
    }
    #endregion
    #endregion

    #region Drone Rotation
    private void UpdateDroneRotation()
    {
        // Have the drone follow the camera's yaw rotation (horizontal rotation)
        wantedYRotation = cameraYaw;
        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotataYVelocity, 0.25f);
    }
    #endregion

    #region Auto-Leveling
    private void HandleAutoLeveling()
    {
        if (Time.time - lastMovementInputTime > autoLevelDelay && !isMouseButtonPressed)
        {
            float levelingFactor = autoLevelSpeed * Time.deltaTime;
            // Gradually return to neutral position (0 angle) when no input
            tiltAmountForward = Mathf.Lerp(tiltAmountForward, 0, levelingFactor);
            tiltAmountSideways = Mathf.Lerp(tiltAmountSideways, 0, levelingFactor);
        }
    }
    #endregion

    #region Audio
    private void UpdateDroneSound()
    {
        if (droneSound != null)
        {
            float speedFactor = droneRigidbody.velocity.magnitude / 90;
            float altitudeFactor = (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift)) ? 0.15f : 0f;

            droneSound.pitch = 1 + speedFactor + altitudeFactor;

            bool hasInput = Mathf.Abs(Input.GetAxis("Vertical")) > inputSensitivityThreshold ||
                           Mathf.Abs(Input.GetAxis("Horizontal")) > inputSensitivityThreshold ||
                           Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.LeftShift);

            droneSound.volume = hasInput ? 0.8f : 0.6f;
        }
    }
    #endregion

    #region Public Methods
    public void ResetInputs()
    {
        // Reset all input values to their default/zero state
        mouseX = 0f;
        mouseY = 0f;

        // Reset any stored velocity or forces
        upForce = 98.1f; // Default hover force

        // Reset tilting
        tiltAmountForward = 0f;
        tiltAmountSideways = 0f;

        // Reset movement velocities
        tiltVelocityForward = 0f;
        tiltAmountVelocity = 0f;

        // Make sure we stop any ongoing movement
        if (droneRigidbody != null)
        {
            droneRigidbody.velocity = Vector3.zero;
            droneRigidbody.angularVelocity = Vector3.zero;
        }

        // Reset the last movement time to prevent auto-leveling immediately
        lastMovementInputTime = Time.time;
    }

    public void EnableControls()
    {
        isActive = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (crosshair != null)
        {
            crosshair.SetActive(true);
        }
    }

    public void SetStabilizationPosition(Vector3 position)
    {
        defaultStabilizationPosition = position;
    }

    public void ToggleAdvancedStabilization(bool enabled)
    {
        enableAdvancedStabilization = enabled;
    }

    public void SetMouseSensitivity(float value)
    {
        // For use with external UI elements
        mouseSensitivity = value;

        // If the slider exists, update it too
        if (sensitivitySlider != null)
        {
            sensitivitySlider.value = value;
        }

        UpdateSensitivityText(value);
    }

    public void ResetCameraAngles()
    {
        cameraYaw = transform.eulerAngles.y;
        cameraPitch = 0f;
    }
    #endregion
}