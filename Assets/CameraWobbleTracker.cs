using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    public Transform target; // The object to track
    public float rotationSpeed = 5.0f; // Speed of rotation adjustment

    void LateUpdate()
    {
        if (target == null) return;

        // Calculate direction to target
        Vector3 directionToTarget = target.position - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Smooth rotation to avoid jitter
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
