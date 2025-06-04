using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class BomberZombie : MonoBehaviour
{
    private CharacterController characterController;
    private Animator animator;

    [SerializeField] private float speed = 2.0f;
    [SerializeField] private float stopDistance = 1.5f;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float deactivateRadius = 5f;
    [SerializeField] private Transform raycastPoint;
    [SerializeField] private GameObject explosionPrefab; // Explosion prefab reference

    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private int damage = 10;
    private float verticalVelocity;

    private Transform target;
    private bool firstPointDeactivated = false;
    private bool isExploding = false;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        HandleGravity();
        FindClosestTarget();

        if (target != null && !isExploding)
        {
            MoveTowardsAndExplodeAtTarget();
        }
        else
        {
            animator.SetFloat("Speed", 0f);
        }

        if (!firstPointDeactivated)
        {
            DeactivateFirstPointInRadius();
        }
    }

    private void HandleGravity()
    {
        if (characterController.isGrounded)
        {
            verticalVelocity = 0;
        }
        else
        {
            verticalVelocity += gravity * Time.deltaTime;
        }

        Vector3 move = new Vector3(0, verticalVelocity, 0);
        characterController.Move(move * Time.deltaTime);
    }

    private void FindClosestTarget()
    {
        Collider[] hitColliders = Physics.OverlapSphere(raycastPoint.position, detectionRadius);
        Transform closestTarget = null;
        float closestDistance = Mathf.Infinity;

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Target")) // Bomber only targets "Target" tagged objects
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
        else if (target != null && (!target.gameObject.activeInHierarchy || Vector3.Distance(transform.position, target.position) > detectionRadius))
        {
            target = null;
        }
    }

    private void MoveTowardsAndExplodeAtTarget()
    {
        Vector3 targetPosition = target.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        if (distanceToTarget <= stopDistance && !isExploding)
        {
            StartExploding();
            return;
        }

        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);

        Vector3 move = directionToTarget * speed;
        characterController.Move(move * Time.deltaTime);

        animator.SetFloat("Speed", speed);
    }

    private void StartExploding()
    {
        isExploding = true;
        animator.SetTrigger("Explode"); // Trigger explosion animation
        Invoke("TriggerExplosion", 5f); // Delay of 5 seconds to match the animation time
    }

    private void TriggerExplosion()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        gameObject.SetActive(false); // Deactivate bomber zombie after explosion
    }

    private void DeactivateFirstPointInRadius()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, deactivateRadius);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("FirstPoint"))
            {
                hitCollider.gameObject.SetActive(false);
                firstPointDeactivated = true;
                Debug.Log($"Deactivated FirstPoint: {hitCollider.gameObject.name}");
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(raycastPoint.position, detectionRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, deactivateRadius);

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
