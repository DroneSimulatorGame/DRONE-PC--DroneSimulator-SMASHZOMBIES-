using UnityEngine;
using UnityEngine.UI;

public class GunOverheatSlider : MonoBehaviour
{
    [SerializeField] private Slider overheatSlider; // Reference to the UI Slider
    [SerializeField] private AudioSource errorAudioSource; // Reference to the AudioSource for error sound
    private bool hasPlayedErrorSound = false; // Flag to prevent repeated sound playback

    private void Start()
    {
        // Ensure the slider and audio source are assigned
        if (overheatSlider == null)
        {
            Debug.LogError("Overheat Slider is not assigned in the Inspector!");
        }
        if (errorAudioSource == null)
        {
            Debug.LogError("Error AudioSource is not assigned in the Inspector!");
        }
    }

    private void Update()
    {
        // Check if the slider has reached its maximum value
        if (overheatSlider.value >= overheatSlider.maxValue && !hasPlayedErrorSound)
        {
            PlayErrorSound();
        }
        else if (overheatSlider.value < overheatSlider.maxValue)
        {
            // Reset the flag when the slider is no longer at max
            hasPlayedErrorSound = false;
        }
    }

    private void PlayErrorSound()
    {
        if (errorAudioSource != null && errorAudioSource.clip != null)
        {
            errorAudioSource.Play();
            hasPlayedErrorSound = true; // Prevent the sound from playing again until the slider resets
        }
        else
        {
            Debug.LogWarning("Error AudioSource or AudioClip is missing!");
        }
    }
}