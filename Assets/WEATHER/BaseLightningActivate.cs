using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntervalObjectActivator : MonoBehaviour
{
    [Header("Activation Settings")]
    public List<GameObject> objectsToActivate;
    public float activationInterval = 1f;

    [Header("Audio Settings")]
    public bool playAudioOnActivate = true;

    [Header("Emission Settings")]
    public List<Material> emissionMaterials; // List of materials with emission

    private void Start()
    {
        // Disable emission for all materials at start
        foreach (Material mat in emissionMaterials)
        {
            if (mat != null)
            {
                mat.DisableKeyword("_EMISSION");
            }
        }
    }

    public void ActivateBaseLight()
    {
        // Stop any existing coroutines to prevent overlap
        StopAllCoroutines();
        StartCoroutine(ActivateObjectsOneByOne());
    }

    public void DeactivateBaseLight()
    {
        // Stop any existing coroutines to prevent overlap
        StopAllCoroutines();
        StartCoroutine(DeactivateObjectsOneByOne());
    }

    IEnumerator ActivateObjectsOneByOne()
    {
        // Enable emission for all materials at the start
        foreach (Material mat in emissionMaterials)
        {
            if (mat != null)
            {
                mat.EnableKeyword("_EMISSION");
                Debug.Log($"Emission enabled for material: {mat.name}");
            }
        }

        foreach (GameObject obj in objectsToActivate)
        {
            if (obj == null) continue;

            // Activate object
            obj.SetActive(true);

            // Play audio using AudioSource
            if (playAudioOnActivate)
            {
                AudioSource audio = obj.GetComponent<AudioSource>();
                if (audio != null)
                {
                    audio.Play();
                }
            }

            yield return new WaitForSeconds(activationInterval);
        }
    }

    IEnumerator DeactivateObjectsOneByOne()
    {
        foreach (GameObject obj in objectsToActivate)
        {
            if (obj == null) continue;

            // Play audio before disabling
            if (playAudioOnActivate)
            {
                AudioSource audio = obj.GetComponent<AudioSource>();
                if (audio != null)
                {
                    audio.Play();
                }
            }

            // Wait briefly to ensure audio starts
            yield return new WaitForSeconds(0.1f);

            // Deactivate object
            obj.SetActive(false);

            yield return new WaitForSeconds(activationInterval);
        }

        // Disable emission for all materials after deactivation
        foreach (Material mat in emissionMaterials)
        {
            if (mat != null)
            {
                mat.DisableKeyword("_EMISSION");
                Debug.Log($"Emission disabled for material: {mat.name}");
            }
        }
    }
}