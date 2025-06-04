using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundTrigger : MonoBehaviour
{
    public AudioSource audioSource;  // Reference to the AudioSource component
    public AudioClip soundEffect;    // Reference to the sound effect clip

    private void OnEnable()
    {
        // Play the sound effect when the UI element is enabled
        if (audioSource != null && soundEffect != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
    }
}
