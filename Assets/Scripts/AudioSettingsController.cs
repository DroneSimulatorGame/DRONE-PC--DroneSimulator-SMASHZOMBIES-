using UnityEngine;

public class AudioSettingsController : MonoBehaviour
{
    [SerializeField] private AudioSource musicAudioSource; // Reference to the music AudioSource

    private bool wasMusicPlaying; // Keeps track of the music state before disabling

    // Method to turn on music
    public void TurnOnMusic()
    {
        if (musicAudioSource != null)
        {
            // Only resume if music was previously playing
            if (!musicAudioSource.isPlaying && wasMusicPlaying)
            {
                musicAudioSource.Play();
                Debug.Log("Music turned ON");
            }
        }
    }

    // Method to turn off music
    public void TurnOffMusic()
    {
        if (musicAudioSource != null)
        {
            // Track if music was playing before stopping it
            wasMusicPlaying = musicAudioSource.isPlaying;
            musicAudioSource.Pause(); // Pause rather than Stop to maintain playback position
            Debug.Log("Music turned OFF");
        }
    }

    // Method to turn on all audio
    public void TurnOnAudio()
    {
        AudioListener.volume = 1; // Set volume to maximum, restoring audio output
        Debug.Log("All audio turned ON");
    }

    // Method to turn off all audio
    public void TurnOffAudio()
    {
        AudioListener.volume = 0; // Mute all audio
        Debug.Log("All audio turned OFF");
    }
}
