using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialController : MonoBehaviour
{
    [Header("References")]
    public GameObject tutorialGameObject;
    public Animator mainCameraAnimator;
    public GameObject regularUI;
    public GameObject audioManager;
    public Button tutorialButton;
    public Image clickBlocker; // Add a full-screen transparent UI Image

    [Header("Settings")]
    public float tutorialDuration = 201f;
    public bool hideCursorDuringTutorial = true;

    private AudioSource audioSource;
    private const string tutorialKey = "TutorialCompleted17";
    private CursorLockMode originalCursorLockState;
    private bool originalCursorVisibility;

    private void Start()
    {
        mainCameraAnimator.enabled = false;

        // Initialize click blocker
        if (clickBlocker != null)
        {
            clickBlocker.gameObject.SetActive(false);
            clickBlocker.raycastTarget = true; // Ensure it blocks clicks
        }

        UpdateTutorialButtonState();
    }

    public void StartTutorialManually()
    {
        if (!PlayerPrefs.HasKey(tutorialKey))
        {
            StartCoroutine(HandleTutorialFlow());
        }
    }

    private IEnumerator HandleTutorialFlow()
    {
        InitializeTutorial();
        yield return new WaitForSeconds(tutorialDuration);
        CompleteTutorial();
    }

    private void InitializeTutorial()
    {
        tutorialButton.gameObject.SetActive(false);

        // Enable click blocker
        if (clickBlocker != null)
        {
            clickBlocker.gameObject.SetActive(true);
        }

        // Save and modify cursor state
        originalCursorLockState = Cursor.lockState;
        originalCursorVisibility = Cursor.visible;

        if (hideCursorDuringTutorial)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        tutorialGameObject.SetActive(true);
        mainCameraAnimator.enabled = true;

        if (regularUI != null)
            regularUI.SetActive(false);

        if (audioManager != null)
        {
            audioSource = audioManager.GetComponent<AudioSource>();
            if (audioSource != null)
            {
                audioSource.volume = 0.1f;
            }
        }
    }

    private void CompleteTutorial()
    {
        PlayerPrefs.SetInt(tutorialKey, 1);
        PlayerPrefs.Save();

        // Disable click blocker
        if (clickBlocker != null)
        {
            clickBlocker.gameObject.SetActive(false);
        }

        // Restore cursor state
        Cursor.lockState = originalCursorLockState;
        Cursor.visible = originalCursorVisibility;

        tutorialGameObject.SetActive(false);
        mainCameraAnimator.enabled = false;

        if (regularUI != null)
            regularUI.SetActive(true);

        if (audioSource != null)
            audioSource.volume = 0.7f;

        UpdateTutorialButtonState();
    }

    // Remaining unchanged methods...
    private void UpdateTutorialButtonState()
    {
        tutorialButton.gameObject.SetActive(!PlayerPrefs.HasKey(tutorialKey));
    }

    public void ResetTutorialProgress()
    {
        PlayerPrefs.DeleteKey(tutorialKey);
        UpdateTutorialButtonState();
        Debug.Log("Tutorial progress reset!");
    }
}