using System.Collections;
using UnityEngine;

public class RedGlow : MonoBehaviour
{
    [SerializeField] public Transform LookAt;       // The object the bomb glow should follow (optional)
    [SerializeField] public Vector3 Offset;         // Optional offset for positioning
    [SerializeField] public float pulseSpeed = 5f;  // Speed of size increase (min to max scale)
    [SerializeField] public float pulseHoldTime = 0.5f; // Time to stay at max size before disappearing
    [SerializeField] public float timeBetweenPulses = 1f; // Delay before the next pulse starts
    [SerializeField] public float minScale = 0.1f;  // Minimum size (before pulse)
    [SerializeField] public float maxScale = 1.5f;  // Maximum size (during pulse)
    [SerializeField] public AudioSource beepSound;  // AudioSource reference for the beep sound

    private Vector3 initialScale;
    private Camera currentCamera;

    private void Start()
    {
        initialScale = transform.localScale;  // Store the initial scale of the sprite

        // Find the initial active camera in the scene
        FindActiveCamera();

        StartCoroutine(Pulsate());  // Start the pulsating effect
    }

    private void Update()
    {
        // Update the camera if it has changed
        if (Camera.main != currentCamera)
        {
            FindActiveCamera();
        }

        // Ensure the sprite is always facing the current active camera (billboard effect)
        FaceCamera();

        // If the object should follow something, update its position.
        if (LookAt != null)
        {
            transform.position = LookAt.position + Offset;
        }
    }

    private void FindActiveCamera()
    {
        currentCamera = Camera.main;  // Automatically detect the active camera
        if (currentCamera == null)
        {
            Debug.LogError("Active camera not found!");
        }
    }

    private IEnumerator Pulsate()
    {
        while (true)
        {
            // Expand quickly from min size to max size
            yield return StartCoroutine(ExpandToMaxSize());

            // Play beep sound at the peak of the pulse
            PlayBeepSound();

            // Hold at max size for a short time
            yield return new WaitForSeconds(pulseHoldTime);

            // Make the object disappear (by setting scale to min)
            transform.localScale = Vector3.one * minScale;

            // Wait for the time between cycles before starting the next pulse
            yield return new WaitForSeconds(timeBetweenPulses);
        }
    }

    private IEnumerator ExpandToMaxSize()
    {
        float currentTime = 0f;
        Vector3 startScale = Vector3.one * minScale;
        Vector3 targetScale = Vector3.one * maxScale;

        while (currentTime < 1f)
        {
            currentTime += Time.deltaTime * pulseSpeed; // Fast expansion
            transform.localScale = Vector3.Lerp(startScale, targetScale, currentTime);
            yield return null;
        }

        transform.localScale = targetScale;  // Ensure it's exactly at max scale
    }

    private void PlayBeepSound()
    {
        if (beepSound != null)
        {
            beepSound.Play();  // Play the beep sound at the peak of each pulse
        }
        else
        {
            Debug.LogWarning("Beep sound is not assigned!");
        }
    }

    // This method ensures the sprite is always facing the active camera
    private void FaceCamera()
    {
        if (currentCamera != null)
        {
            // Make the object rotate to face the active camera at all times (billboard effect)
            transform.LookAt(currentCamera.transform);
            transform.Rotate(0f, 180f, 0f);  // Invert it so the sprite looks toward the camera properly
        }
    }
}
