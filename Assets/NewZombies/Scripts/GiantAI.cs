using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GianteMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float detectionRadius = 10f; // Radius for finding targets
    [SerializeField] private float deactivateRadius = 5f; // Radius for deactivating "FirstPoint" objects
    [SerializeField] private Transform raycastPoint; // Point for raycast

    // Gravity Variables
    [SerializeField] private float gravity = -9.81f; // Gravity value
    [SerializeField] private int damage = 20;
    private float verticalVelocity; // Vertical velocity

    private Transform target;
    private bool firstPointDeactivated = false; // Track if a "FirstPoint" has been deactivated

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        // Randomly set mirror state
        bool isMirrored = Random.value > 0.5f;
        animator.SetBool("IsMirrored", isMirrored);
    }



    private void Update()
    {
        // Handle gravity
        HandleGravity();
        if (target == null) { FindClosestTarget(); }
        // Continuously search for the closest target

        if (target != null)
        {
            MoveTowardsAndAttackTarget();
        }
        else
        {
            // Idle state if no target is found
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Punch", false);
        }

        // Check for "FirstPoint" objects to deactivate
        if (!firstPointDeactivated)
        {
            DeactivateFirstPointInRadius();
        }
    }

    private void HandleGravity()
    {
        // Apply gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = 0; // Reset vertical velocity if grounded
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime; // Apply gravity
        }

        // Move the zombie
        Vector3 move = new Vector3(0, verticalVelocity, 0);
        characterController.Move(move * Time.deltaTime);
    }

    private void FindClosestTarget()
    {
        // Look for the closest "Building" or "Target" object in detection radius
        Collider[] hitColliders = Physics.OverlapSphere(raycastPoint.position, detectionRadius);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Building") || hitCollider.CompareTag("Target"))
            {
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.ClosestPointOnBounds(transform.position));
                // Check if this target is closer than the current closest target
                if (distanceToTarget < closestDistance)
                {
                    closestTarget = hitCollider.transform;
                    closestDistance = distanceToTarget;
                }
            }
        }

        // If there's a new closest target, assign it
        if (closestTarget != null)
        {
            target = closestTarget;
        }
        // Check if current target is inactive or outside the detection radius
        else if (target != null && (!target.gameObject.activeInHierarchy ||
            Vector3.Distance(transform.position, target.position) > detectionRadius))
        {
            target = null; // Clear target if inactive or out of range
        }
    }

    private void MoveTowardsAndAttackTarget()
    {
        // Use the closest point on the collider for distance calculations
        Vector3 targetPosition = target.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // Stop and attack if within stop distance
        if (distanceToTarget <= stopDistance)
        {
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Punch", true);
            return;
        }

        // Move towards the target and face it
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        Vector3 move = directionToTarget * speed;
        characterController.Move(move * Time.deltaTime);

        animator.SetFloat("Speed", speed);
        animator.SetBool("Punch", false);
    }

    private void DeactivateFirstPointInRadius()
    {
        // Look for "FirstPoint" objects in deactivate radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, deactivateRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("FirstPoint"))
            {
                hitCollider.gameObject.SetActive(false); // Deactivate the "FirstPoint" object
                firstPointDeactivated = true; // Mark it as deactivated
                Debug.Log($"Deactivated FirstPoint: {hitCollider.gameObject.name}"); // Debug log
                break; // Exit after deactivating one point
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(raycastPoint.position, detectionRadius);

        // Visualize deactivate radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, deactivateRadius);

        // Visualize stop distance
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, stopDistance);
    }
    public void TakeDamage()
    {
        if (target.tag == "Target")
        {
            target.GetComponent<EnimyDetect>().Damage(damage);
        }
        else if (target.tag == "Building")
        {
            target.GetComponent<Building1>().Damage(damage);
        }
    }
}