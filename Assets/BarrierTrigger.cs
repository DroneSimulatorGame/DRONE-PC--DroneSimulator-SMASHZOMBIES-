using UnityEngine;

public class BarrierTrigger : MonoBehaviour
{
    public Rigidbody droneRigidbody; // Reference to drone's Rigidbody
    private bool isColliding = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != droneRigidbody.gameObject) // Ignore drone itself
        {
            isColliding = true;
            // Freeze movement temporarily
            droneRigidbody.velocity *= 0.2f;
            droneRigidbody.angularVelocity *= 0.2f;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject != droneRigidbody.gameObject)
        {
            isColliding = false;
        }
    }

    void FixedUpdate()
    {
        if (isColliding)
        {
            // Keep velocity low while inside trigger
            droneRigidbody.velocity *= 0.9f;
        }
    }
}