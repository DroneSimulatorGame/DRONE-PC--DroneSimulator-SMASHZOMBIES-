using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class POVCamera : MonoBehaviour
{
    public static POVCamera Instance;
    public Camera mainCam; // Reference to the main camera
    public Camera droneCam; // Reference to the drone camera
    public Camera povCam; // Reference to the POV camera
    public Volume globalVolume; // Reference to the Global Volume component

    public VolumeProfile defaultProfile; // Profile for default mode
    public VolumeProfile effectProfile; // Profile for night vision mode

    //public Material enemyMaterial; // Reference to the enemy material
    public GameObject hudUI; // Reference to the HUD UI element

    private Color originalEmissionColor; // Store original emission color
    private float originalEmissionIntensity = 1f; // Adjust if your material has an initial emission intensity

    [Header("Camera Shake Settings")]
    public float shakeDuration = 0.5f; // Default shake duration
    public float shakeMagnitude = 0.2f; // Default shake magnitude

    private Coroutine shakeCoroutine; // To keep track of the shake coroutine
    private int shakeCount = 0; // Counter for shake calls
    private const int maxShakes = 12; // Maximum number of shakes allowed


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // Save the original emission settings for the enemy material
        //if (enemyMaterial != null && enemyMaterial.IsKeywordEnabled("_EMISSION"))
        //{
        //    originalEmissionColor = enemyMaterial.GetColor("_EmissionColor");
        //    originalEmissionIntensity = enemyMaterial.GetFloat("_EmissionIntensity");
        //}

        ApplyCorrectProfile();
    }

    public void ToggleDroneAndPOV()
    {
        if (droneCam.enabled)
        {
            droneCam.enabled = false;
            povCam.enabled = true;
            hudUI.SetActive(true); // Enable HUD when POV is active
            ApplyNightVisionEffect(true);
        }
        else
        {
            droneCam.enabled = true;
            povCam.enabled = false;
            hudUI.SetActive(false); // Disable HUD when POV is inactive
            ApplyNightVisionEffect(false);
        }

        ApplyCorrectProfile();
    }

    private void ApplyCorrectProfile()
    {
        if (mainCam != null && mainCam.enabled)
        {
            globalVolume.profile = defaultProfile;
            ApplyNightVisionEffect(false);
        }
        else if (droneCam != null && droneCam.enabled)
        {
            globalVolume.profile = defaultProfile;
            ApplyNightVisionEffect(false);
        }
        else if (povCam != null && povCam.enabled)
        {
            globalVolume.profile = effectProfile;
            ApplyNightVisionEffect(true);
        }
    }

    private void ApplyNightVisionEffect(bool enable)
    {
        //if (enemyMaterial == null) return;

        //if (enable)
        //{
        //    enemyMaterial.EnableKeyword("_EMISSION");
        //    enemyMaterial.SetColor("_EmissionColor", Color.white * 5f);  // Adjust intensity as needed
        //}
        //else
        //{
        //    enemyMaterial.SetColor("_EmissionColor", originalEmissionColor);
        //}
    }

    public void TurnOffEmission()
    {
        //if (enemyMaterial == null) return;

        //if (enemyMaterial.HasProperty("_EmissionColor"))
        //{
        //    enemyMaterial.SetColor("_EmissionColor", Color.black); // Reset to black
        //    enemyMaterial.DisableKeyword("_EMISSION"); // Disable the emission keyword
        //}
    }

    public void ResetToDefaultProfile()
    {
        if (globalVolume != null && defaultProfile != null)
        {
            globalVolume.profile = defaultProfile;
        }
    }

    public void TriggerCameraShake()
    {
        if (shakeCount < maxShakes)
        {
            shakeCount++;

            if (shakeCoroutine != null)
            {
                StopCoroutine(shakeCoroutine);
            }

            shakeCoroutine = StartCoroutine(CameraShake(shakeDuration, shakeMagnitude));
        }
    }

    private IEnumerator CameraShake(float duration, float magnitude)
    {
        Vector3 originalPosition = povCam.transform.localPosition;
        Quaternion originalRotation = povCam.transform.localRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            float xShake = Random.Range(-magnitude, magnitude);
            float yShake = Random.Range(-magnitude, magnitude);

            povCam.transform.localPosition = originalPosition + new Vector3(xShake, yShake, 0);
            povCam.transform.localRotation = Quaternion.Euler(
                originalRotation.eulerAngles.x + Random.Range(-magnitude, magnitude),
                originalRotation.eulerAngles.y + Random.Range(-magnitude, magnitude),
                originalRotation.eulerAngles.z
            );

            elapsed += Time.deltaTime;
            yield return null;
        }

        povCam.transform.localPosition = originalPosition;
        povCam.transform.localRotation = originalRotation;
        shakeCoroutine = null;
    }

    public void ResetShakeCount()
    {
        shakeCount = 0;
    }

    // Method to reset the HUD
    public void ResetHUD()
    {
        hudUI.SetActive(false); // Deactivate HUD
    }
}
