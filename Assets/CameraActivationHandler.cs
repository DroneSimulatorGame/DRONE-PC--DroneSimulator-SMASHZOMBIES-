using UnityEngine;
using System.Collections;

public class CameraActivationHandler : MonoBehaviour
{
    [Header("References")]
    [Tooltip("The camera to monitor.")]
    public Camera targetCamera;

    [Tooltip("The object to enable.")]
    public GameObject targetObject;

    [Header("Settings")]
    [Tooltip("The unique key for saving the activation state in PlayerPrefs.")]
    public string playerPrefKey = "CameraActivated";

    private bool hasActivated = false;

    void Start()
    {
        // Check if this event has already happened
        if (PlayerPrefs.GetInt(playerPrefKey, 0) == 1)
        {
            hasActivated = true;
            return;
        }

        if (targetCamera == null || targetObject == null)
        {
            Debug.LogError("Target camera or object is not assigned!");
        }

        // Ensure the target object starts disabled
        targetObject.SetActive(false);
    }

    void Update()
    {
        if (!hasActivated && targetCamera.isActiveAndEnabled)
        {
            hasActivated = true; // Prevent multiple triggers
            StartCoroutine(HandleCameraActivation());
        }
    }

    private IEnumerator HandleCameraActivation()
    {
        // Wait for 3 seconds after the camera becomes active
        yield return new WaitForSeconds(3);

        // Enable the referenced object
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }

        // Pause the game
        Time.timeScale = 0;

        // Allow the animator to continue by using unscaled time
        Animator animator = targetObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        }

        // Mute the referenced camera's AudioListener
        AudioListener cameraAudioListener = targetCamera.GetComponent<AudioListener>();
        if (cameraAudioListener != null)
        {
            AudioListener.pause = true;
        }

        // Save the state to PlayerPrefs
        PlayerPrefs.SetInt(playerPrefKey, 1);
        PlayerPrefs.Save();

        // Wait for 10 seconds while the game is paused
        yield return new WaitForSecondsRealtime(35);  // Use real-time seconds for the wait

        // Unpause the game
        Time.timeScale = 1;

        // Restore the animator's update mode to default (optional)
        if (animator != null)
        {
            animator.updateMode = AnimatorUpdateMode.Normal;
        }

        // Unmute the referenced camera's AudioListener
        if (cameraAudioListener != null)
        {
            AudioListener.pause = false;
        }
    }
}
