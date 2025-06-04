using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthOfFieldController : MonoBehaviour
{
    public Volume globalVolume;
    private DepthOfField depthOfField;

    private void Start()
    {
        // Try to get the DepthOfField component from the global volume
        if (globalVolume.profile.TryGet(out depthOfField))
        {
            // Set the initial mode to Gaussian
            depthOfField.mode.value = DepthOfFieldMode.Gaussian;
        }
        else
        {
            Debug.LogError("DepthOfField component not found in the global volume.");
        }
    }

    // Method to switch Depth of Field to Bokeh mode
    public void Paused()
    {
        if (depthOfField != null)
        {
            depthOfField.mode.value = DepthOfFieldMode.Bokeh;
        }
    }

    // Method to switch Depth of Field back to Gaussian mode
    public void Continued()
    {
        if (depthOfField != null)
        {
            depthOfField.mode.value = DepthOfFieldMode.Gaussian;
        }
    }
}
