using UnityEngine;
using UnityEngine.UI;
using TMPro; // Import TextMesh Pro namespace
using UnityEngine.Rendering; // Required for Volume component
using UnityEngine.Rendering.Universal; // Import if using Universal Render Pipeline (URP)

public class DroneBoundaryCheck : MonoBehaviour
{
    public Transform dronePoint;
    public Volume globalVolume; // Reference to the global Volume

    public float boundaryX = 50.0f;
    public float boundaryY = 20.0f;
    public float boundaryZ = 50.0f;
    public float returnTime = 3.0f;

    public TextMeshProUGUI outOfBoundsMessage;
    public TextMeshProUGUI countdownMessage;
    public GameObject outOfBoundsBackground; // New UI element (image background)
    public GameObject penaltyUI;
    public GameObject playerUI;
    public MonoBehaviour movementScript;
    //public MyJoystickNew2 MyJoystickNew2;

    public AudioSource warningSound; // AudioSource for the warning sound

    private bool isOutOfBounds = false;
    private bool hasPenaltyTriggered = false; // Prevents re-triggering penalty UI
    private float countdownTimer;

    // Saturation settings
    private float targetSaturation = 0f; // Saturation target
    private float currentSaturation = 0f; // Current saturation value
    public float saturationChangeSpeed = 10f; // Increased speed for saturation change

    void Start()
    {
        countdownTimer = returnTime;
        if (penaltyUI != null) penaltyUI.SetActive(false);
        if (outOfBoundsMessage != null) outOfBoundsMessage.gameObject.SetActive(false);
        if (countdownMessage != null) countdownMessage.gameObject.SetActive(false);
        if (outOfBoundsBackground != null) outOfBoundsBackground.SetActive(false); // Ensure image is hidden at start

        // Ensure warning sound is stopped at start
        if (warningSound != null) warningSound.Stop();
    }

    void Update()
    {
        CheckBoundary(); // Continuously check if the drone is out of bounds
        UpdateCountdown(); // Update countdown timer if needed
        UpdateSaturation(); // Update the saturation every frame
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(boundaryX * 2, boundaryY * 2, boundaryZ * 2));
    }

    void CheckBoundary()
    {
        if (dronePoint == null) return;

        Vector3 dronePosition = dronePoint.position;
        Vector3 centerPosition = transform.position;

        if (Mathf.Abs(dronePosition.x - centerPosition.x) > boundaryX ||
            Mathf.Abs(dronePosition.y - centerPosition.y) > boundaryY ||
            Mathf.Abs(dronePosition.z - centerPosition.z) > boundaryZ)
        {
            if (!isOutOfBounds)
            {
                isOutOfBounds = true;
                hasPenaltyTriggered = false; // Reset flag so penalty can trigger again if needed

                // Show out-of-bounds message, background, and start countdown
                if (outOfBoundsMessage != null) outOfBoundsMessage.gameObject.SetActive(true);
                if (countdownMessage != null)
                {
                    countdownMessage.gameObject.SetActive(true);
                    countdownTimer = returnTime;
                }
                if (outOfBoundsBackground != null) outOfBoundsBackground.SetActive(true); // Show background image

                // Start playing warning sound if it's not already playing
                if (warningSound != null && !warningSound.isPlaying)
                {
                    warningSound.loop = true; // Ensure the sound loops
                    warningSound.Play();
                }

                // Set target saturation to -100 when out of bounds
                targetSaturation = -100f;
            }
        }
        else
        {
            if (isOutOfBounds)
            {
                isOutOfBounds = false;

                // Hide messages and background when back within bounds
                if (outOfBoundsMessage != null) outOfBoundsMessage.gameObject.SetActive(false);
                if (countdownMessage != null) countdownMessage.gameObject.SetActive(false);
                if (outOfBoundsBackground != null) outOfBoundsBackground.SetActive(false); // Hide background image

                // Stop playing the warning sound when back in bounds
                if (warningSound != null && warningSound.isPlaying)
                {
                    warningSound.Stop();
                }

                // Reset target saturation to 0 when back in bounds
                targetSaturation = 0f;
            }
        }
    }

    void UpdateCountdown()
    {
        if (isOutOfBounds && !hasPenaltyTriggered)
        {
            countdownTimer -= Time.deltaTime;

            if (countdownMessage != null)
            {
                countdownMessage.text = $"Return in: {Mathf.Ceil(countdownTimer)}s";
            }

            if (countdownTimer <= 0)
            {
                OnTimeUp();
            }
        }
    }

    // Update the saturation smoothly
    void UpdateSaturation()
    {
        if (globalVolume != null)
        {
            // Get the ColorAdjustments component from the Volume
            if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
            {
                // Gradually change the saturation to the target value
                currentSaturation = Mathf.MoveTowards(currentSaturation, targetSaturation, saturationChangeSpeed * Time.deltaTime);
                colorAdjustments.saturation.value = currentSaturation; // Update saturation
            }
        }
    }

    // Handle the penalty when the countdown reaches zero
    void OnTimeUp()
    {
        hasPenaltyTriggered = true; // Ensure penalty UI doesn't show up again immediately
        if (movementScript != null) movementScript.enabled = false;
        if (penaltyUI != null) penaltyUI.SetActive(true);
        if (playerUI != null) 
        {
            //MyJoystickNew2.instance.joystickHorizontal = 0;
            //MyJoystickNew2.instance.joystickVertical = 0;
            //MyJoystickNew2.instance.horizontalInput = 0;
            //MyJoystickNew2.instance.verticalInput = 0;




            playerUI.SetActive(false); }

        // Hide messages and background after the penalty
        if (outOfBoundsMessage != null) outOfBoundsMessage.gameObject.SetActive(false);
        if (countdownMessage != null) countdownMessage.gameObject.SetActive(false);
        if (outOfBoundsBackground != null) outOfBoundsBackground.SetActive(false); // Hide background image

        // Stop warning sound after penalty
        if (warningSound != null && warningSound.isPlaying)
        {
            warningSound.Stop();
        }
    }

    // Called when the leave button is pressed
    public void OnLeaveButtonPressed()
    {
        if (penaltyUI != null) penaltyUI.SetActive(false);
        if (movementScript != null) movementScript.enabled = true;

        // Reactivate the player UI and reset the countdown
        if (playerUI != null) playerUI.SetActive(true);

        countdownTimer = returnTime;
        hasPenaltyTriggered = true; // Prevent penalty from triggering again immediately

        // Optionally hide the out-of-bounds message and background
        if (outOfBoundsMessage != null) outOfBoundsMessage.gameObject.SetActive(false);
        if (countdownMessage != null) countdownMessage.gameObject.SetActive(false);
        if (outOfBoundsBackground != null) outOfBoundsBackground.SetActive(false); // Hide background image

        // Stop the warning sound if playing
        if (warningSound != null && warningSound.isPlaying)
        {
            warningSound.Stop();
        }
    }
}
