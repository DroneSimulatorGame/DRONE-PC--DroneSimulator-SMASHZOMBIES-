using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StarAnimation : MonoBehaviour
{
    public GameObject star; // The star object to animate
    public Vector3 initialScale = new Vector3(2f, 2f, 2f); // Starting scale (bigger than normal)
    public Vector3 targetScale = new Vector3(1f, 1f, 1f); // Final normal scale
    public float animationDuration = 0.5f; // Time to animate the star scale

    // Background image settings
    public Image backgroundImage; // The background image to control size and alpha
    public Vector3 minBackgroundScale = new Vector3(0.8f, 0.8f, 0.8f); // Minimum size for background image
    public Vector3 maxBackgroundScale = new Vector3(1.2f, 1.2f, 1.2f); // Maximum size for background image
    public float scaleTransitionSpeed = 2f; // Speed of size transition
    public float breathingStartDelay = 1f; // Delay before starting breathing effect

    // Alpha control settings
    public float minAlpha = 0.5f; // Minimum alpha value
    public float maxAlpha = 1f; // Maximum alpha value
    public float alphaTransitionSpeed = 1f; // Speed of alpha transition

    // Audio settings
    public GameObject audioObject; // Empty GameObject with an AudioSource
    private AudioSource audioSource; // Reference to the AudioSource component

    private Coroutine breathingCoroutine;

    private void Awake()
    {
        // Ensure we have an AudioSource component assigned
        if (audioObject != null)
        {
            audioSource = audioObject.GetComponent<AudioSource>();
        }
    }

    private void OnEnable()
    {
        // Activate the star and run its scaling animation
        if (star != null)
        {
            star.transform.localScale = initialScale;
            StartCoroutine(AnimateStarScale());

            // Play the audio sound when the star is activated
            if (audioSource != null)
            {
                audioSource.Play();
            }

            // Activate the background image and start breathing after delay
            if (backgroundImage != null)
            {
                backgroundImage.gameObject.SetActive(true); // Activate the background image
                StartCoroutine(HandleBreathingAfterDelay()); // Start breathing after delay
            }
        }
    }

    private void OnDisable()
    {
        // Deactivate the background image when the star is deactivated
        if (star != null && !star.activeSelf)
        {
            if (backgroundImage != null)
            {
                backgroundImage.gameObject.SetActive(false); // Deactivate the background image
            }
            if (breathingCoroutine != null)
            {
                StopCoroutine(breathingCoroutine); // Stop breathing effect if active
                breathingCoroutine = null; // Reset coroutine reference
            }
        }
    }

    IEnumerator AnimateStarScale()
    {
        float elapsedTime = 0f;

        while (elapsedTime < animationDuration)
        {
            // Gradually scale the star object
            star.transform.localScale = Vector3.Lerp(initialScale, targetScale, elapsedTime / animationDuration);
            elapsedTime += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }

        // Ensure final scale is set to the target scale
        star.transform.localScale = targetScale;
    }

    IEnumerator HandleBreathingAfterDelay()
    {
        // Wait for the breathingStartDelay duration
        yield return new WaitForSeconds(breathingStartDelay);

        // Start breathing effect
        if (backgroundImage != null && backgroundImage.gameObject.activeSelf)
        {
            Debug.Log("Starting breathing effect after delay.");
            breathingCoroutine = StartCoroutine(BreathingEffect());
        }
    }

    IEnumerator BreathingEffect()
    {
        while (backgroundImage != null && backgroundImage.gameObject.activeSelf) // Ensure breathing happens only while active
        {
            // Scale the background image between min and max sizes
            float scale = Mathf.PingPong(Time.time * scaleTransitionSpeed, 1f);
            backgroundImage.transform.localScale = Vector3.Lerp(minBackgroundScale, maxBackgroundScale, scale);

            // Control the alpha of the background image
            float alpha = Mathf.PingPong(Time.time * alphaTransitionSpeed, 1f);
            Color color = backgroundImage.color;
            color.a = Mathf.Lerp(minAlpha, maxAlpha, alpha);
            backgroundImage.color = color;

            // Wait for the next frame
            yield return null;
        }
    }
}
