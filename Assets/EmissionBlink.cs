using UnityEngine;

public class EmissionBlink : MonoBehaviour
{
    public Material material;
    public Color emissionColor = Color.red;
    public float blinkInterval = 1f; // Interval between blinks
    public bool doubleBlink = false; // Enable for double-blink effect
    public float maxIntensity = 5f; // Controls how intense the emission is

    private bool isEmitting = false;

    private void Start()
    {
        if (material == null)
        {
            material = GetComponent<Renderer>().material;
        }

        // Set the initial emission color with intensity set to zero
        material.SetColor("_EmissionColor", emissionColor * 0);
    }

    private void Update()
    {
        if (doubleBlink)
        {
            StartCoroutine(DoubleBlinkRoutine());
        }
        else
        {
            SingleBlink();
        }
    }

    private void SingleBlink()
    {
        if (Time.time % (blinkInterval * 2) < blinkInterval)
        {
            EnableEmission();
        }
        else
        {
            DisableEmission();
        }
    }

    private System.Collections.IEnumerator DoubleBlinkRoutine()
    {
        EnableEmission();
        yield return new WaitForSeconds(0.1f);
        DisableEmission();
        yield return new WaitForSeconds(0.1f);
        EnableEmission();
        yield return new WaitForSeconds(0.1f);
        DisableEmission();
        yield return new WaitForSeconds(blinkInterval);
    }

    private void EnableEmission()
    {
        if (!isEmitting)
        {
            material.EnableKeyword("_EMISSION");
            material.SetColor("_EmissionColor", emissionColor * maxIntensity); // Set to max intensity
            isEmitting = true;
        }
    }

    private void DisableEmission()
    {
        if (isEmitting)
        {
            material.SetColor("_EmissionColor", emissionColor * 0); // Turn off emission by setting intensity to zero
            material.DisableKeyword("_EMISSION");
            isEmitting = false;
        }
    }
}
