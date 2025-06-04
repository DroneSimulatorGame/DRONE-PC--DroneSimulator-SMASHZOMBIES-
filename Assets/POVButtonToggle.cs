using UnityEngine;

public class POVButtonToggle : MonoBehaviour
{
    // Reference to the Pause Button
    [SerializeField] private GameObject pauseButton;

    // Boolean to track the state
    private bool isPauseButtonActive = true;

    // Method called when the POV button is pressed
    public void OnPOVButtonPressed()
    {
        // Toggle the state
        isPauseButtonActive = !isPauseButtonActive;

        // Set the active state of the Pause Button
        if (pauseButton != null)
        {
            pauseButton.SetActive(isPauseButtonActive);
        }
        else
        {
            Debug.LogWarning("Pause Button reference is not set!");
        }
    }
}
