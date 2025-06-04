using UnityEngine;

public class Billboard2 : MonoBehaviour
{
    [Tooltip("The sprite or object to rotate towards the camera. If not set, it will default to the object this script is attached to.")]
    public Transform target;

    [Tooltip("Reference to the intermediate object that controls this sprite's visibility and logic.")]
    public GameObject intermediateObject;

    private Camera cameraToFace;

    private void OnEnable()
    {
        // If no target is set, default to the current GameObject
        if (target == null)
        {
            target = transform;
        }

        // Automatically find the camera tagged as "MainCamera"
        cameraToFace = Camera.main;

        if (cameraToFace == null)
        {
            Debug.LogError("No camera tagged as 'MainCamera' found in the scene.");
            enabled = false; // Disable the script if no camera is available
        }
    }

    private void Update()
    {
        // Ensure the intermediate object is valid and active
        if (intermediateObject != null && intermediateObject.activeInHierarchy)
        {
            // Enable the target and make it face the camera
            if (target != null)
            {
                target.gameObject.SetActive(true);
                target.forward = cameraToFace.transform.forward;
            }
        }
        else
        {
            // Disable the target if the intermediate object is inactive
            if (target != null)
            {
                target.gameObject.SetActive(false);
            }
        }
    }
}
