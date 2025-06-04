using UnityEngine;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    // Boolean to store whether the tutorial has been finished
    private bool tutorialFinished;

    // Reference to the tutorial UI GameObject
    public GameObject tutorialUI;

    void Start()
    {
        // Check if the tutorial has already been completed (saved in PlayerPrefs)
        tutorialFinished = PlayerPrefs.GetInt("TutorialFinished16", 0) == 1;

        // If it's the player's first time playing and the tutorial hasn't been finished
        if (!tutorialFinished)
        {
            // Start coroutine to enable the tutorial UI after a delay
            StartCoroutine(ShowTutorialAfterDelay(2f));

            // Update PlayerPrefs to mark the tutorial as finished for future sessions
            PlayerPrefs.SetInt("TutorialFinished16", 1);
            PlayerPrefs.Save();
        }
        else
        {
            // If the tutorial is already finished, ensure the tutorial UI is hidden
            tutorialUI.SetActive(false);
        }
    }

    // Coroutine to wait before activating the tutorial UI
    private IEnumerator ShowTutorialAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        tutorialUI.SetActive(true);
    }
}
