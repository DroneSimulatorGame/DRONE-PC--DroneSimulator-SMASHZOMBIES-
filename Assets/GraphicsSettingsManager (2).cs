using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GraphicsSettingsManager : MonoBehaviour
{
    [Header("Render Pipeline Assets")]
    public UniversalRenderPipelineAsset lowQualityAsset;
    public UniversalRenderPipelineAsset mediumQualityAsset;
    public UniversalRenderPipelineAsset highQualityAsset;

    [Header("Global Volume")]
    public Volume globalVolume;

    [Header("Post Effects Control")]
    [Tooltip("If true, bloom will be disabled in low quality")]
    public bool controlBloom = true;
    [Tooltip("If true, DOF will be disabled in low quality")]
    public bool controlDOF = true;
    [Tooltip("If true, Color Adjustments will be disabled in low quality")]
    public bool controlColorAdjustments = true;

    [Header("UI Buttons")]
    public Button lowButton;
    public Button mediumButton;
    public Button highButton;

    [Header("UI Colors")]
    public Color selectedColor = Color.cyan;
    public Color unselectedColor = Color.gray;

    private void Start()
    {
        LoadQualitySettings();
        UpdateButtonStates();
    }

    public void SetLowQuality()
    {
        QualitySettings.SetQualityLevel(0, true);
        QualitySettings.renderPipeline = lowQualityAsset;
        if (globalVolume != null)
        {
            // Always enable the volume component itself
            globalVolume.enabled = true;

            // Configure post effects based on control settings
            ConfigurePostEffectsForLowQuality();
        }
        SaveQualitySetting(0);
        UpdateButtonStates();
        Debug.Log("Low Quality Set");
    }

    public void SetMediumQuality()
    {
        QualitySettings.SetQualityLevel(1, true);
        QualitySettings.renderPipeline = mediumQualityAsset;
        if (globalVolume != null)
        {
            globalVolume.enabled = true;
            EnableAllPostEffects();
        }
        SaveQualitySetting(1);
        UpdateButtonStates();
        Debug.Log("Medium Quality Set");
    }

    public void SetHighQuality()
    {
        QualitySettings.SetQualityLevel(2, true);
        QualitySettings.renderPipeline = highQualityAsset;
        if (globalVolume != null)
        {
            globalVolume.enabled = true;
            EnableAllPostEffects();
        }
        SaveQualitySetting(2);
        UpdateButtonStates();
        Debug.Log("High Quality Set");
    }

    private void ConfigurePostEffectsForLowQuality()
    {
        if (globalVolume == null || globalVolume.profile == null)
            return;

        // For bloom, if controlBloom is false, we ENABLE bloom in low quality
        if (globalVolume.profile.TryGet(out Bloom bloom))
        {
            bloom.active = controlBloom;  // Inverse logic: when controlBloom is false, bloom is enabled
        }

        // For DOF, if controlDOF is false, we ENABLE DOF in low quality
        if (globalVolume.profile.TryGet(out DepthOfField dof))
        {
            dof.active = controlDOF;  // Inverse logic: when controlDOF is false, DOF is enabled
        }

        // For Color Adjustments, if controlColorAdjustments is false, we ENABLE it in low quality
        if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            colorAdjustments.active = controlColorAdjustments;  // Inverse logic
        }
    }

    private void EnableAllPostEffects()
    {
        if (globalVolume == null || globalVolume.profile == null)
            return;

        // Always enable all post-processing for medium and high quality
        if (globalVolume.profile.TryGet(out Bloom bloom))
        {
            bloom.active = true;
        }

        if (globalVolume.profile.TryGet(out DepthOfField dof))
        {
            dof.active = true;
        }

        if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            colorAdjustments.active = true;
        }
    }

    private void UpdateButtonStates()
    {
        int currentQuality = QualitySettings.GetQualityLevel();
        SetButtonColor(lowButton, currentQuality == 0);
        SetButtonColor(mediumButton, currentQuality == 1);
        SetButtonColor(highButton, currentQuality == 2);
    }

    private void SetButtonColor(Button button, bool selected)
    {
        if (button != null)
        {
            var image = button.GetComponent<Image>();
            if (image != null)
                image.color = selected ? selectedColor : unselectedColor;
        }
    }

    private void SaveQualitySetting(int qualityLevel)
    {
        PlayerPrefs.SetInt("QualityLevel", qualityLevel);
        PlayerPrefs.Save();
    }

    private void LoadQualitySettings()
    {
        int savedLevel = PlayerPrefs.GetInt("QualityLevel", 2); // Default to High Quality
        switch (savedLevel)
        {
            case 0:
                SetLowQuality();
                break;
            case 1:
                SetMediumQuality();
                break;
            case 2:
            default:
                SetHighQuality();
                break;
        }
    }

}