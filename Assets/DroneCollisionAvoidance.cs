using UnityEngine;

public class DroneCollisionAvoidance : MonoBehaviour
{
    public Transform droneTransform; // Reference to drone's Transform
    public Rigidbody droneRigidbody; // Reference to drone's Rigidbody
    public float detectionDistance = 0.5f; // Range to detect colliders
    public float velocityDampFactor = 0.05f; // How much to slow velocity (0 = stop, 1 = no change)
    public LayerMask obstacleLayer; // Layer for colliders to detect
    public bool stopMovement = true; // Toggle between stopping or slowing movement

    private Vector3[] rayDirections = {
        Vector3.forward, Vector3.back,
        Vector3.left, Vector3.right,
        Vector3.up, Vector3.down
    };

    void FixedUpdate()
    {
        // Convert global velocity to local space for easier direction checks
        Vector3 localVelocity = droneTransform.InverseTransformDirection(droneRigidbody.velocity);

        // Cast rays in each direction
        foreach (Vector3 direction in rayDirections)
        {
            Ray ray = new Ray(droneTransform.position, droneTransform.TransformDirection(direction));
            if (Physics.Raycast(ray, out RaycastHit hit, detectionDistance, obstacleLayer))
            {
                // Check if the drone is moving toward the hit collider
                float velocityInDirection = Vector3.Dot(localVelocity, direction);
                if (velocityInDirection > 0) // Moving toward the collider
                {
                    // Stop or slow movement in this direction
                    if (stopMovement)
                    {
                        // Zero out velocity component in this direction
                        if (direction == Vector3.forward || direction == Vector3.back)
                            localVelocity.z = Mathf.Min(localVelocity.z, 0);
                        if (direction == Vector3.left || direction == Vector3.right)
                            localVelocity.x = Mathf.Min(localVelocity.x, 0);
                        if (direction == Vector3.up || direction == Vector3.down)
                            localVelocity.y = Mathf.Min(localVelocity.y, 0);
                    }
                    else
                    {
                        // Drastically slow velocity in this direction
                        if (direction == Vector3.forward || direction == Vector3.back)
                            localVelocity.z *= velocityDampFactor;
                        if (direction == Vector3.left || direction == Vector3.right)
                            localVelocity.x *= velocityDampFactor;
                        if (direction == Vector3.up || direction == Vector3.down)
                            localVelocity.y *= velocityDampFactor;
                    }
                }
            }
        }

        // Apply modified velocity back to the Rigidbody
        droneRigidbody.velocity = droneTransform.TransformDirection(localVelocity);
    }

    // Visualize rays in Scene view for debugging
    void OnDrawGizmos()
    {
        if (droneTransform == null) return;
        Gizmos.color = Color.red;
        foreach (Vector3 direction in rayDirections)
        {
            Vector3 rayEnd = droneTransform.position + droneTransform.TransformDirection(direction) * detectionDistance;
            Gizmos.DrawLine(droneTransform.position, rayEnd);
            Gizmos.DrawWireSphere(rayEnd, 0.05f); // Small sphere at ray end
        }
    }
}