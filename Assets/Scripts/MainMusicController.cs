using UnityEngine;

public class MainMusicController : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private Camera targetCamera;

    [Header("Global Sound Settings")]
    [SerializeField] private bool isMusicEnabled = false;
    [SerializeField] private bool areSoundsEnabled = false;

    private void Update()
    {
        HandleMusicPlayback();
    }

    private void HandleMusicPlayback()
    {
        if (musicSource == null || targetCamera == null) return;

        // Check if the music should play based on camera's active state and the isMusicEnabled flag
        bool isTargetCameraActive = targetCamera.enabled && Camera.main == targetCamera;

        if (isMusicEnabled && isTargetCameraActive)
        {
            if (!musicSource.isPlaying) musicSource.Play();
        }
        else
        {
            if (musicSource.isPlaying) musicSource.Pause();
        }
    }

    // Method to toggle music on/off
    public void SetMusicEnabled(bool enabled)
    {
        isMusicEnabled = !enabled;
        HandleMusicPlayback();
    }




    // Method to toggle sounds on/off
    public void SetSoundsEnabled(bool enabled)
    {
        // Toggle the sounds based on the input
        areSoundsEnabled = !enabled;

        // Set the global audio volume
        SetAudioVolume(areSoundsEnabled);
    }

    // Helper to set the global audio volume
    private void SetAudioVolume(bool isEnabled)
    {
        AudioListener.volume = isEnabled ? 1f : 0f; // 1 for full volume, 0 for mute

        // Optional: Log the current state for debugging
        Debug.Log($"Audio is now {(isEnabled ? "Enabled" : "Muted")}");
    }
}
