using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class LoadingScreenManager : MonoBehaviour
{
    [SerializeField] private Slider progressBar;      // Reference to the progress bar slider
    [SerializeField] private TMP_Text progressText;   // Reference to the progress percentage text
    [SerializeField] private string sceneName = "MainScene"; // Name of the scene to load
    [SerializeField] private float fakeLoadingTime = 3f; // Total time for fake loading effect

    private void Start()
    {
        StartCoroutine(FakeLoadingProgress());
    }

    private IEnumerator FakeLoadingProgress()
    {
        // Start loading the scene asynchronously
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false; // Prevent immediate activation of the loaded scene

        float elapsedTime = 0f;  // Timer to track the fake loading progress
        float displayProgress = 0f; // Fake progress shown to the user

        // Smoothly fake the progress bar over the desired loading time
        while (elapsedTime < fakeLoadingTime)
        {
            elapsedTime += Time.deltaTime;
            displayProgress = Mathf.Lerp(0f, 1f, elapsedTime / fakeLoadingTime);

            // Update the progress bar
            progressBar.value = displayProgress;

            // Update the progress text with a "running numbers" animation
            if (progressText != null)
            {
                int animatedPercentage = Mathf.FloorToInt(displayProgress * 100f);
                progressText.text = $"{animatedPercentage}%";
            }

            yield return null;
        }

        // Wait until the scene is fully loaded
        while (operation.progress < 0.9f)
        {
            yield return null;
        }

        // Automatically activate the scene once the fake loading is complete
        operation.allowSceneActivation = true;
    }
}
