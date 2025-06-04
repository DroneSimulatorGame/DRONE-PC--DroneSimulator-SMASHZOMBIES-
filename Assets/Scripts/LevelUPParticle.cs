using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUPParticle : MonoBehaviour
{
    public AudioClip[] soundClips; // Array of audio clips
    private AudioSource audioSource; // Reference to an AudioSource component

    void OnEnable()
    {
        // Get the AudioSource attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on " + gameObject.name);
            return;
        }

        // Make sure there are sound clips assigned
        if (soundClips.Length == 0)
        {
            Debug.LogWarning("No audio clips assigned to " + gameObject.name);
            return;
        }

        // Play a random sound
        PlayRandomSound();
    }

    public void PlayRandomSound()
    {
        // Stop any sound currently playing on this AudioSource (if any)
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        // Choose a random sound clip from the array
        int randomIndex = Random.Range(0, soundClips.Length);
        audioSource.clip = soundClips[randomIndex];

        // Play the audio clip
        audioSource.Play();
    }
}
