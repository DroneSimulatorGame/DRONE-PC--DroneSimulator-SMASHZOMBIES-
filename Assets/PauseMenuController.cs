using UnityEngine;
public class PauseMenuController : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject TopMenuePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject playPanel;
    private bool isPaused = false;

    private void Update()
    {
        if (!TopMenuePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!CheckUI.Instance.IsAnyPanelOpen() || playPanel.activeInHierarchy)
                {
                    TogglePauseMenu();
                }
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                POVCamera.Instance.ToggleDroneAndPOV();
            }
        }
    }

    public void TogglePauseMenu()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            pausePanel.SetActive(false);
            playPanel.SetActive(true);
        }
        else
        {
            pausePanel.SetActive(true);
            playPanel.SetActive(false);
        }
        TogglePause();
        try
        {
            AudioContainer.Instance.tickedS();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Tugma ovozini ijro etishda xatolik: " + e.Message);
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        AudioListener.pause = true;
        Debug.Log("O'yin to'xtatildi");
    }

    public void PlayGame()
    {
        Time.timeScale = 1;
        AudioListener.pause = false;
        Debug.Log("O'yin davom ettirildi");
    }

    public void TogglePause()
    {
        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            PlayGame();
        }
    }
}