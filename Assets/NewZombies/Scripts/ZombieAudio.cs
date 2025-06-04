using UnityEngine;

public class ZombieAudio : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource; // Reference to the audio source component
    [SerializeField] private AudioClip[] walkSounds; // Array of walking sounds
    [SerializeField] private AudioClip[] attackSounds; // Array of attack sounds
    [SerializeField] private AudioClip[] preattackSounds;

    // Method to play a random walking sound
    public void PlayRandomWalkSound()
    {
        if (walkSounds.Length == 0) return; // Check if there are walk sounds available
        int randomIndex = Random.Range(0, walkSounds.Length);
        audioSource.clip = walkSounds[randomIndex];
        audioSource.Play();
    }

    // Method to play a random attack sound
    public void PlayRandomAttackSound()
    {
        if (attackSounds.Length == 0) return; // Check if there are attack sounds available
        int randomIndex = Random.Range(0, attackSounds.Length);
        audioSource.clip = attackSounds[randomIndex];
        audioSource.Play();
    }

    public void PlayPreAttackSound()
    {
        if (preattackSounds.Length == 0) return; // Check if there are attack sounds available
        int randomIndex = Random.Range(0, preattackSounds.Length);
        audioSource.clip = preattackSounds[randomIndex];
        audioSource.Play();
    }


}
