using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BulletHitEffect : MonoBehaviour
{
    public AudioClip[] soundClips; // Array of audio clips
    private AudioSource audioSource; // Reference to an AudioSource component

    void Start()
    {
        // Get or assign the AudioSource, assuming it's already part of the prefab
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found on BulletHitEffect prefab.");
            return;
        }

        // Make sure there are sound clips assigned
        if (soundClips.Length == 0)
        {
            Debug.LogWarning("No audio clips assigned to BulletHitEffect.");
            Destroy(gameObject); // Destroy if no sound clips are available
            return;
        }

        // Play a random sound
        PlayRandomSound();

        // Optionally register the AudioSource with the AudioController for mute/volume control
        //AudioManager.Instance.RegisterAudioSource(audioSource);
    }

    public void PlayRandomSound()
    {
        // Choose a random sound clip from the array
        int randomIndex = Random.Range(0, soundClips.Length);
        audioSource.clip = soundClips[randomIndex];

        // Play the audio clip
        audioSource.Play();

        // Optionally destroy this GameObject after the sound finishes playing
        Destroy(gameObject, audioSource.clip.length);
    }
}