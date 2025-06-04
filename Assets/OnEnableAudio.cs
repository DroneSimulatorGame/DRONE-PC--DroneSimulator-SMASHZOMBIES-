using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OnEnableAudio : MonoBehaviour
{
    public AudioClip audioClip; // Reference to the audio clip to play
    private AudioSource audioSource;

    private void Awake()
    {
        // Get or add the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // Play the audio when the GameObject is enabled
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("No audio clip assigned to PlayAudioOnEnable script on " + gameObject.name);
        }
    }
}
