using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        // Find the main camera in the scene
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Make the sprite always face the camera
        Vector3 cameraDirection = mainCamera.transform.forward;
        transform.forward = -cameraDirection;
    }
}
