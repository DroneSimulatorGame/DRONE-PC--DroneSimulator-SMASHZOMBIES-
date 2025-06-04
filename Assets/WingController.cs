using UnityEngine;

public class WingController : MonoBehaviour
{
    [Header("Wing Objects")]
    [SerializeField] private Transform leftWing;
    [SerializeField] private Transform rightWing;

    [Header("Rotation Settings")]
    [SerializeField] private float forwardAngle = 45f;
    [SerializeField] private float turnAngle = 30f;
    [SerializeField] private float rotationSpeed = 5f; // Controls rotation smoothness

    private Quaternion leftWingTargetRotation;
    private Quaternion rightWingTargetRotation;
    private Quaternion leftWingInitialRotation;
    private Quaternion rightWingInitialRotation;

    private void Start()
    {
        // Store initial rotations
        if (leftWing != null && rightWing != null)
        {
            leftWingInitialRotation = leftWing.localRotation;
            rightWingInitialRotation = rightWing.localRotation;
        }
        else
        {
            Debug.LogError("Wing objects are not assigned!");
        }

        // Initialize target rotations to current rotations
        leftWingTargetRotation = leftWingInitialRotation;
        rightWingTargetRotation = rightWingInitialRotation;
    }

    private void Update()
    {
        // Check for input and call corresponding methods
        if (Input.GetKey(KeyCode.W))
        {
            MoveForward();
        }
        else if (Input.GetKey(KeyCode.S))
        {
            MoveBackward();
        }
        else if (Input.GetKey(KeyCode.A))
        {
            TurnLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            TurnRight();
        }
        else
        {
            // Return to initial rotation when no keys are pressed
            ResetWings();
        }

        // Smoothly interpolate to target rotations
        if (leftWing != null && rightWing != null)
        {
            leftWing.localRotation = Quaternion.Lerp(
                leftWing.localRotation,
                leftWingTargetRotation,
                Time.deltaTime * rotationSpeed
            );

            rightWing.localRotation = Quaternion.Lerp(
                rightWing.localRotation,
                rightWingTargetRotation,
                Time.deltaTime * rotationSpeed
            );
        }
    }

    private void MoveForward()
    {
        // Rotate both wings to forward angle on X axis
        leftWingTargetRotation = Quaternion.Euler(
            forwardAngle,
            leftWingInitialRotation.eulerAngles.y,
            leftWingInitialRotation.eulerAngles.z
        );

        rightWingTargetRotation = Quaternion.Euler(
            forwardAngle,
            rightWingInitialRotation.eulerAngles.y,
            rightWingInitialRotation.eulerAngles.z
        );
    }

    private void MoveBackward()
    {
        // Rotate both wings to negative forward angle on X axis
        leftWingTargetRotation = Quaternion.Euler(
            -forwardAngle,
            leftWingInitialRotation.eulerAngles.y,
            leftWingInitialRotation.eulerAngles.z
        );

        rightWingTargetRotation = Quaternion.Euler(
            -forwardAngle,
            rightWingInitialRotation.eulerAngles.y,
            rightWingInitialRotation.eulerAngles.z
        );
    }

    private void TurnLeft()
    {
        // Left wing rotates to -30, right wing to +30 on X axis
        leftWingTargetRotation = Quaternion.Euler(
            -turnAngle,
            leftWingInitialRotation.eulerAngles.y,
            leftWingInitialRotation.eulerAngles.z
        );

        rightWingTargetRotation = Quaternion.Euler(
            turnAngle,
            rightWingInitialRotation.eulerAngles.y,
            rightWingInitialRotation.eulerAngles.z
        );
    }

    private void TurnRight()
    {
        // Left wing rotates to +30, right wing to -30 on X axis
        leftWingTargetRotation = Quaternion.Euler(
            turnAngle,
            leftWingInitialRotation.eulerAngles.y,
            leftWingInitialRotation.eulerAngles.z
        );

        rightWingTargetRotation = Quaternion.Euler(
            -turnAngle,
            rightWingInitialRotation.eulerAngles.y,
            rightWingInitialRotation.eulerAngles.z
        );
    }

    private void ResetWings()
    {
        // Return wings to initial rotation
        leftWingTargetRotation = leftWingInitialRotation;
        rightWingTargetRotation = rightWingInitialRotation;
    }
}