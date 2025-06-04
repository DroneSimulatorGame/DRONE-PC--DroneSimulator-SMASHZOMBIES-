using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthScript : MonoBehaviour
{
    public Slider healthSlider;
    public Gradient color;
    public Image fill;
    public string cameraTag = "Camera"; // Set the custom camera tag here
    public GameObject canva;

    void Update()
    {
        LookAtCamera();
    }

    public void SetHealth(int health)
    {
        healthSlider.value = health;
        fill.color = color.Evaluate(healthSlider.normalizedValue);
    }

    public void SetMaxHealth(int health)
    {
        healthSlider.maxValue = health;
        healthSlider.value = health;
        fill.color = color.Evaluate(1f);
    }

    private void LookAtCamera()
    {
        // Find the camera with the specified tag
        GameObject cameraObject = GameObject.FindGameObjectWithTag(cameraTag);

        if (cameraObject != null)
        {
            // Make this object face the camera
            canva.transform.LookAt(cameraObject.transform);
            // Adjust rotation to keep the UI facing the camera properly
            canva.transform.rotation = Quaternion.LookRotation(transform.position - cameraObject.transform.position);
        }
        else
        {
            Debug.LogWarning("Camera with tag " + cameraTag + " not found.");
        }
    }
}
