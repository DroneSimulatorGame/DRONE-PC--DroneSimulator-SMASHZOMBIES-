using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSoundManager : MonoBehaviour
{
    // Lists for each sound type
    public List<AudioClip> walkSounds;
    public List<AudioClip> attackSounds;
    //public List<AudioClip> hitSounds;
    //public List<AudioClip> deathSounds;

    // AudioSource for playing sounds
    [SerializeField] private AudioSource audioSource;

    // Volume and pitch ranges for randomization
    [Header("Volume Range")]
    public float minVolume = 0.8f;
    public float maxVolume = 1.0f;

    [Header("Pitch Range")]
    public float minPitch = 0.9f;
    public float maxPitch = 1.1f;

    [Header("3D Audio Settings")]
    public float minDistance = 5.0f; // Full volume when listener is within this range
    public float maxDistance = 20.0f; // Volume will drop off after this distance

    private Coroutine walkSoundCoroutine;

    void Start()
    {
        // Get the AudioSource component
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            Debug.LogError("AudioSource component not found on the GameObject. Please attach an AudioSource component.");
            return;
        }

        // Set the AudioSource to 3D mode
        audioSource.spatialBlend = 1.0f;  // Full 3D sound
        audioSource.minDistance = minDistance;
        audioSource.maxDistance = maxDistance;

        // Start playing the walk sound in a loop
        PlayWalkSound();
    }

    // Play a random walk sound in a loop
    public void PlayWalkSound()
    {
        if (walkSoundCoroutine != null)
        {
            StopCoroutine(walkSoundCoroutine);
        }

        // Start a coroutine to play the walk sound continuously in a loop
        walkSoundCoroutine = StartCoroutine(LoopWalkSound());
    }

    // Play a random attack sound once and then resume the walk sound
    public void PlayAttackSound()
    {
        PlayRandomSoundOnce(attackSounds);
    }

    // Play a random hit sound once and then resume the walk sound
    //public void PlayHitSound()
    //{
    //    PlayRandomSoundOnce(hitSounds);
    //}

    //// Play a random death sound once and then resume the walk sound
    //public void PlayDeathSound()
    //{
    //    PlayRandomSoundOnce(deathSounds);
    //}

    // Helper method to loop the walk sound continuously
    private IEnumerator LoopWalkSound()
    {
        while (true)
        {
            PlayRandomSound(walkSounds);
            yield return new WaitForSeconds(audioSource.clip.length); // Wait for the sound to finish before playing another one
        }
    }

    // Helper method to play a random sound from a list with random volume and pitch
    private void PlayRandomSound(List<AudioClip> soundList)
    {
        if (soundList == null || soundList.Count == 0)
        {
            Debug.LogWarning("Sound list is null or empty! Please assign sound clips.");
            return;
        }

        // Select a random sound from the list
        int randomIndex = Random.Range(0, soundList.Count);

        // Randomize volume and pitch
        audioSource.volume = Random.Range(minVolume, maxVolume);
        audioSource.pitch = Random.Range(minPitch, maxPitch);

        // Play the random sound
        audioSource.clip = soundList[randomIndex];
        audioSource.loop = false;  // Make sure it's not looping
        audioSource.Play();
    }

    // Helper method to play a random sound from a list once and resume the walk sound after 0.5 seconds
    private void PlayRandomSoundOnce(List<AudioClip> soundList)
    {
        if (walkSoundCoroutine != null)
        {
            StopCoroutine(walkSoundCoroutine); // Stop the walk sound from playing while another sound is playing
        }

        PlayRandomSound(soundList);

        // Resume the walk sound after 0.5 seconds
        StartCoroutine(ResumeWalkSoundAfterDelay(0.1f));
    }

    // Coroutine to resume the walk sound after a delay
    private IEnumerator ResumeWalkSoundAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        PlayWalkSound();
    }
}
