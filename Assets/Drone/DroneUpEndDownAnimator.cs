using UnityEngine;

public class DroneUpEndDownAnimator : MonoBehaviour
{
    public GameObject targetObject; // Final landing spot
    public GameObject intermediatePoint; // The intermediate point

    private Vector3 initialPosition; // Initial starting position
    public float targetHeight = 2.0f; // Hover height for the lift
    public float liftSpeed = 50.0f; // Speed of movement
    //-------------
    public float landingSpeed = 100f;

    private bool isLifting = false;
    private bool isLanding = false;
    private bool toIntermediatePoint = false;

    public StartFinish startFinish;
    public MyJoystickNew2 droneMoveScript;

    public Rigidbody dronerb;
    public float lookSpeed = 5f; // Speed of rotation when looking at a point

    private bool isResettingRotation = false;
    private Quaternion targetRotation = Quaternion.Euler(0, 0, 0);

    void Start()
    {
        // Ensure initial drone state
        isLifting = false;
        isLanding = false;
        toIntermediatePoint = false;

        droneMoveScript.droneSound.enabled = false;
        startFinish.stopRotors();

        if (targetObject != null)
        {
            initialPosition = targetObject.transform.position;
        }
        else
        {
            Debug.LogError("Target object is not assigned. Please assign it in the Unity Inspector.");
        }
    }

    public void LiftDrone()
    {
        isLifting = true;
        isLanding = false;
        toIntermediatePoint = false;

        startFinish.startRotors();
        droneMoveScript.droneSound.enabled = true;
        droneMoveScript.droneSound.volume = 0.3f;

        dronerb.drag = 0f;
        dronerb.angularDrag = 0.05f;
    }

    public void LandDrone()
    {
        isLanding = true;
        isLifting = false;

        toIntermediatePoint = intermediatePoint != null; // Only set to true if the intermediate point is assigned
        droneMoveScript.droneSound.volume = 0.05f;

        startFinish.stopMvement();
        dronerb.drag = 100f;
        dronerb.angularDrag = 100f;
        dronerb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (isLifting)
        {
            // Move to target hover height
            float step = liftSpeed * Time.deltaTime;
            Vector3 targetPosition = new Vector3(transform.position.x, initialPosition.y + targetHeight, transform.position.z);
            MoveAndLook(targetPosition);

            if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            {
                isLifting = false;
            }
        }

        if (toIntermediatePoint)
        {
            // Move to intermediate point
            float step = landingSpeed * Time.deltaTime;
            MoveAndLook(intermediatePoint.transform.position);

            if (Vector3.Distance(transform.position, intermediatePoint.transform.position) < 0.001f)
            {
                toIntermediatePoint = false;
            }
        }

        if (isLanding && !toIntermediatePoint)
        {
            // Move to final landing position
            float step = landingSpeed * Time.deltaTime;
            MoveAndLook(initialPosition);

            if (Vector3.Distance(transform.position, initialPosition) < 0.05f)
            {
                isLanding = false;

                startFinish.stopRotors();
                droneMoveScript.droneSound.enabled = false;

                // 🚁 Start smooth rotation reset
                isResettingRotation = true;
            }

        }
        if (isResettingRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);

            // Check if close enough to stop
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                transform.rotation = targetRotation;
                isResettingRotation = false;
            }
        }

    }


    private void MoveAndLook(Vector3 targetPosition)
    {
        // Move toward the target position
        float step = landingSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

        // Rotate smoothly to face the target position
        Vector3 direction = targetPosition - transform.position;
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lookSpeed * Time.deltaTime);
        }
    }
}
