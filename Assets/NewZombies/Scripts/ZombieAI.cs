using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ZombieMovement : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float sidestepDuration = 0.5f;
    [SerializeField] private float sidestepDistance = 1f;
    [SerializeField] private int damage = 5;

    private Transform target;
    private float sidestepTimer = 0f;
    private Vector3 sidestepDirection;
    private bool isSidestepping = false;
    private bool isAttacking = false; // Track if zombie is in attack phase


    [SerializeField] private float gravity = -9.81f;
    private float verticalVelocity; // Vertical velocity

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleGravity();

        if (target == null)
        {
            FindClosestTarget();
        }
        // Handle targeting and movement if not sidestepping
        if (!isSidestepping && !isAttacking)
        {
            FindClosestTarget();
        }

        if (target != null && !isSidestepping)
        {
            MoveTowardsAndAttackTarget();
            if (!isAttacking) // Only check for obstacles if not attacking
            {
                CheckForObstacles();
            }
        }
        else if (isSidestepping)
        {
            Sidestep();
        }
        else
        {
            // Idle if no target
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Punch", false);
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





    private void CheckForObstacles()
    {
        // Look for zombies within the stopDistance
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, stopDistance);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && hitCollider.gameObject != gameObject)
            {
                StartSidestep();
                break;
            }
        }
    }

    private void StartSidestep()
    {
        // Randomly choose a sidestep direction (left or right) and rotate the zombie to face that direction
        sidestepDirection = (Random.value > 0.5f) ? transform.right : -transform.right;
        sidestepTimer = sidestepDuration;
        isSidestepping = true;

        // Rotate the model to face the sidestep direction
        Quaternion sidestepRotation = Quaternion.LookRotation(sidestepDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, sidestepRotation, Time.deltaTime * 5f);
    }

    private void Sidestep()
    {
        if (sidestepTimer > 0)
        {
            // Move in the sidestep direction and apply rotation
            Vector3 sidestepMovement = sidestepDirection * sidestepDistance * Time.deltaTime;
            characterController.Move(sidestepMovement);

            // Continue rotating toward sidestep direction
            Quaternion sidestepRotation = Quaternion.LookRotation(sidestepDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, sidestepRotation, Time.deltaTime * 5f);

            sidestepTimer -= Time.deltaTime;
            animator.SetFloat("Speed", speed); // Maintain animation during sidestep
        }
        else
        {
            isSidestepping = false; // End sidestep
        }
    }

    private void FindClosestTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Building") || hitCollider.CompareTag("Target"))
            {
                float distanceToTarget = Vector3.Distance(transform.position, hitCollider.ClosestPointOnBounds(transform.position));
                if (distanceToTarget < closestDistance)
                {
                    closestTarget = hitCollider.transform;
                    closestDistance = distanceToTarget;
                }
            }
        }

        if (closestTarget != null)
        {
            target = closestTarget;
        }
        else if (target != null && (!target.gameObject.activeInHierarchy ||
                 Vector3.Distance(transform.position, target.position) > detectionRadius))
        {
            target = null;
        }
    }

    private void MoveTowardsAndAttackTarget()
    {
        Vector3 targetPosition = target.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget <= stopDistance)
        {
            // Enter attack phase and ignore sidestepping
            isAttacking = true;
            animator.SetFloat("Speed", 0f);
            animator.SetBool("Punch", true);
            return;
        }

        // Exit attack phase if out of stop distance
        isAttacking = false;

        // Move towards the target and face it
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        Vector3 move = directionToTarget * speed;
        characterController.Move(move * Time.deltaTime);

        animator.SetFloat("Speed", speed);
        animator.SetBool("Punch", false);
    }

    public Transform referenceObject; // Add this line for the referenced object

    private void OnDrawGizmos()
    {
        if (referenceObject != null) // Ensure reference is assigned
        {
            // Draw the detection radius
            Gizmos.color = Color.yellow; // Color for detection radius
            Gizmos.DrawWireSphere(referenceObject.position, detectionRadius);

            // Draw the stop distance radius
            Gizmos.color = Color.red; // Color for stop distance
            Gizmos.DrawWireSphere(referenceObject.position, stopDistance);
        }
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