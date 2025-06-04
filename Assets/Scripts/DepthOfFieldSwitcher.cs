using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldSwitcher : MonoBehaviour
{
    [Header("UI References")]
    public GameObject[] uiElements; // Array to store references to the UI elements

    [Header("Global Volume")]
    public Volume globalVolume; // Reference to the Global Volume component

    private DepthOfField dof; // Reference to the Depth of Field component

    private void Start()
    {
        // Fetch the Depth of Field override from the Global Volume
        if (globalVolume.profile.TryGet<DepthOfField>(out dof))
        {
            // Set default to Gaussian at the start
            SetGaussian();
        }
        else
        {
            Debug.LogError("No Depth of Field found on the Global Volume!");
        }
    }

    private void Update()
    {
        // Check if any UI elements are active
        bool isAnyUIActive = IsAnyUIActive();

        // Switch between Gaussian and Bokeh modes
        if (isAnyUIActive)
        {
            SetBokeh();
        }
        else
        {
            SetGaussian();
        }
    }

    // Method to check if any UI element is active
    private bool IsAnyUIActive()
    {
        foreach (GameObject uiElement in uiElements)
        {
            if (uiElement.activeInHierarchy)
            {
                return true; // If any UI element is active, return true
            }
        }
        return false; // If none are active, return false
    }

    // Method to switch to Gaussian mode
    private void SetGaussian()
    {
        if (dof != null)
        {
            dof.mode.value = DepthOfFieldMode.Gaussian;
        }
    }

    // Method to switch to Bokeh mode
    private void SetBokeh()
    {
        if (dof != null)
        {
            dof.mode.value = DepthOfFieldMode.Bokeh;
        }
    }
}
