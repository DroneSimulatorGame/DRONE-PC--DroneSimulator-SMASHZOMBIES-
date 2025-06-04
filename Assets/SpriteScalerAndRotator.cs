using Unity.VisualScripting;
using UnityEngine;

public class SpriteScalerAndRotator : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private Transform spriteTransform; // Reference to the sprite object

    [Header("Animation Settings")]
    [SerializeField] private Vector3 scaleAmplitude = new Vector3(0.1f, 0.1f, 0.0f); // How much to scale
    [SerializeField] private float animationSpeed = 2f; // Speed of the breathing effect
    public string cameraTag = "Camera";

    private Vector3 baseScale; // Initial local scale

    void OnEnable()
    {
        // Ensure the spriteTransform is assigned
        if (spriteTransform == null)
        {
            Debug.LogError("Sprite Transform is not assigned!", this);
            return;
        }

        // Store the base scale of the sprite
        baseScale = spriteTransform.localScale;
    }

    void Update()
    {
        if (spriteTransform == null) return;
        LookAtCamera();

        // Generate the oscillating scale using a sine wave
        float scaleOffset = Mathf.Sin(Time.time * animationSpeed) * 0.5f + 0.5f; // Range: 0 to 1
        Vector3 animatedScale = baseScale + scaleAmplitude * scaleOffset;

        // Apply the new scale
        spriteTransform.localScale = animatedScale;
    }

    private void LookAtCamera()
    {
        // Find the camera with the specified tag
        GameObject cameraObject = GameObject.FindGameObjectWithTag(cameraTag);

        if (cameraObject != null)
        {
            // Make this object face the camera
            spriteTransform.transform.LookAt(cameraObject.transform);
            // Adjust rotation to keep the UI facing the camera properly
            spriteTransform.transform.rotation = Quaternion.LookRotation(transform.position - cameraObject.transform.position);
        }
        else
        {
            Debug.LogWarning("Camera with tag " + cameraTag + " not found.");
        }
    }


}