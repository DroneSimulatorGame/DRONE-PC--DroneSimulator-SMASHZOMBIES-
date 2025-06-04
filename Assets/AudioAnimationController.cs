using UnityEngine;

public class AudioAnimationController : MonoBehaviour
{
    // Reference to the AudioSource component
    public AudioSource audioSource;

    // Array to hold the 14 audio clips
    public AudioClip[] audioClips = new AudioClip[14];

    void Start()
    {
        // Get the AudioSource component attached to this GameObject
        audioSource = GetComponent<AudioSource>();

        // Ensure AudioSource is present on the GameObject
        if (audioSource == null)
        {
            Debug.LogError("No AudioSource found! Please add an AudioSource component to this GameObject.");
        }
    }

    // Method to play a specific clip based on its index
    private void PlayClip(int index)
    {
        if (audioSource != null && index >= 0 && index < audioClips.Length)
        {
            audioSource.clip = audioClips[index];
            audioSource.Play();
        }
        else
        {
            Debug.LogError("Invalid clip index or AudioSource missing!");
        }
    }

    // Individual methods to play each clip
    public void PlayClip1() => PlayClip(0);
    public void PlayClip2() => PlayClip(1);
    public void PlayClip3() => PlayClip(2);
    public void PlayClip4() => PlayClip(3);
    public void PlayClip5() => PlayClip(4);
    public void PlayClip6() => PlayClip(5);
    public void PlayClip7() => PlayClip(6);
    public void PlayClip8() => PlayClip(7);
    public void PlayClip9() => PlayClip(8);
    public void PlayClip10() => PlayClip(9);
    public void PlayClip11() => PlayClip(10);
    public void PlayClip12() => PlayClip(11);
    public void PlayClip13() => PlayClip(12);
    public void PlayClip14() => PlayClip(13);
}
