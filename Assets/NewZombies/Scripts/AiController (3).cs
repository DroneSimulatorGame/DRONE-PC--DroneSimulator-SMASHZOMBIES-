using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MyCharacterController : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator animator;
    private List<Transform> targets;
    private Transform randomTarget;
    [SerializeField] private GameObject raycastPoint;
    [SerializeField] private ZombieSoundManager zombieSoundManager;

    [SerializeField] private float maxTime = 1.0f;
    [SerializeField] private float maxDistance = 1.0f;
    [SerializeField] private float detectionRange = 5.0f;
    [SerializeField] private float agentDetectRange = 2f;
    [SerializeField] private float punchDistance = 0.8f;

    private float timer = 0.0f;
    private int buildingLayer;
    private int targetLayer;

    private void Start()
    {
        CacheComponents();
        InitializeLayers();
        FindTargets();
    }

    private void OnEnable()
    {
        zombieSoundManager.PlayWalkSound();
        FindTargets();
    }

    private void CacheComponents()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void InitializeLayers()
    {
        buildingLayer = LayerMask.NameToLayer("Building");
        targetLayer = LayerMask.NameToLayer("Target");
    }

    private void FixedUpdate()
    {
        if (IsAgentValid())
        {
            timer -= Time.deltaTime;
            if (timer <= 0.0f)
            {
                NavigateToTarget();
                AvoidOtherAgents();
                timer = maxTime;
            }
            UpdateAnimator();
            HandleObstacleDetection();
        }
    }

    private bool IsAgentValid()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogError("No targets assigned or found.");
            return false;
        }

        if (!agent.enabled || !agent.isOnNavMesh)
        {
            Debug.LogWarning("Agent is either disabled or not on a valid NavMesh.");
            return false;
        }

        return true;
    }

    private void NavigateToTarget()
    {
        randomTarget = FindRandomTarget();
        bool obstacleDetected = PerformRaycast(transform.forward, detectionRange, out RaycastHit cachedHit);
        if (obstacleDetected)
        {
            agent.SetDestination(cachedHit.point);
        }
        else if (randomTarget != null && Vector3.Distance(raycastPoint.transform.position, randomTarget.position) > maxDistance)
        {
            agent.SetDestination(randomTarget.position);
        }
    }

    private bool PerformRaycast(Vector3 direction, float range, out RaycastHit hitInfo)
    {
        return Physics.Raycast(raycastPoint.transform.position, direction, out hitInfo, range);
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    private void HandleObstacleDetection()
    {
        bool shouldTurnLeft = false;
        bool shouldTurnRight = false;

        // Check forward direction (mainly for punching)
        if (PerformRaycast(transform.forward, detectionRange, out RaycastHit forwardHit))
        {
            if (IsObstacleWithinPunchRange(forwardHit))
            {
                animator.SetBool("Punch", true);
            }
            else
            {
                animator.SetBool("Punch", false);
            }
        }
        else
        {
            animator.SetBool("Punch", false);
        }

        // Check slightly to the left
        Vector3 leftDirection = Quaternion.Euler(0, -90f, 0) * transform.forward; // 30-degree angle to the left
        if (PerformRaycast(leftDirection, detectionRange, out RaycastHit leftHit))
        {
            // Check if the hit object has the tag "Target"
            if (IsObstacleClose(leftHit) && leftHit.collider.CompareTag("Target"))
            {
                shouldTurnRight = true; // Obstacle on the left, so turn right
            }
        }

        // Check slightly to the right
        Vector3 rightDirection = Quaternion.Euler(0, 90f, 0) * transform.forward; // 30-degree angle to the right
        if (PerformRaycast(rightDirection, detectionRange, out RaycastHit rightHit))
        {
            // Check if the hit object has the tag "Target"
            if (IsObstacleClose(rightHit) && rightHit.collider.CompareTag("Target"))
            {
                shouldTurnLeft = true; // Obstacle on the right, so turn left
            }
        }

        // Turn the agent based on obstacle detection
        if (shouldTurnLeft)
        {
            TurnAgent(60f); // Turn 20 degrees to the left
        }
        else if (shouldTurnRight)
        {
            TurnAgent(-60f); // Turn 20 degrees to the right
        }
    }

    // Helper method to check if the obstacle is close
    private bool IsObstacleClose(RaycastHit hit)
    {
        return hit.distance < detectionRange;
    }

    // Helper method to turn the agent
    private void TurnAgent(float angle)
    {
        Vector3 newDirection = Quaternion.Euler(0, angle, 0) * transform.forward;
        Vector3 newDestination = agent.transform.position + newDirection * maxDistance;

        if (NavMesh.SamplePosition(newDestination, out NavMeshHit navHit, maxDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(navHit.position);
        }
    }



    // Helper method to check if the obstacle should trigger the Punch animation
    private bool IsObstacleWithinPunchRange(RaycastHit hit)
    {
        int hitLayer = hit.collider.gameObject.layer;
        return hit.distance < punchDistance && (hitLayer == buildingLayer || hitLayer == targetLayer);
    }


    private void AvoidOtherAgents()
    {
        if (PerformRaycast(transform.forward, agentDetectRange, out RaycastHit cachedHit))
        {
            if (cachedHit.collider.gameObject.CompareTag("Enemy") || cachedHit.collider.gameObject.CompareTag("Buzilgan"))
            {
                // Define a slight turning angle (in degrees)
                float turnAngle = Random.Range(15f, 30f); // Random small angle to turn
                turnAngle *= (Random.value > 0.5f) ? 1 : -1; // Randomly decide to turn left or right

                // Calculate the new direction by rotating the forward vector slightly
                Vector3 avoidanceDirection = Quaternion.Euler(0, turnAngle, 0) * transform.forward;

                // Calculate the new destination point based on the adjusted direction
                Vector3 newDestination = agent.transform.position + avoidanceDirection * maxDistance;

                // Check if the new destination is on the NavMesh and set it
                if (NavMesh.SamplePosition(newDestination, out NavMeshHit navHit, maxDistance, NavMesh.AllAreas))
                {
                    agent.SetDestination(navHit.position);
                }
            }
        }
    }


    public void FindTargets()
    {
        if (targets == null)
        {
            targets = new List<Transform>();
        }
        else
        {
            targets.Clear();
        }

        GameObject[] allGameObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allGameObjects)
        {
            if (obj.layer == buildingLayer)
            {
                targets.Add(obj.transform);
            }
        }

        if (targets.Count == 0)
        {
            Debug.LogError("No targets found in the scene.");
        }
    }

    private Transform FindRandomTarget()
    {
        if (targets == null || targets.Count == 0)
        {
            Debug.LogWarning("No targets available for random selection.");
            return null;
        }
        if (randomTarget != null) { return randomTarget; }

        int randomIndex = Random.Range(0, targets.Count);
        return targets[randomIndex];
    }

    //public void TakeDamage()
    //{
    //    if (PerformRaycast(transform.forward, agentDetectRange, out RaycastHit cachedHit))
    //    {
    //        GameObject hitObject = cachedHit.collider.gameObject;

    //        if (hitObject.CompareTag("Target"))
    //        {
    //            var enemyDetect = hitObject.GetComponent<EnimyDetect>();
    //            if (enemyDetect != null)
    //            {
    //                zombieSoundManager.PlayAttackSound();
    //                enemyDetect.NormalDamage();
    //            }
    //            else
    //            {
    //                Debug.LogError("EnimyDetect component missing on the target!");
    //            }
    //        }
    //        else if (hitObject.CompareTag("Building"))
    //        {
    //            var building = hitObject.GetComponent<Building1>();
    //            if (building != null)
    //            {
    //                zombieSoundManager.PlayAttackSound();
    //                building.NormalDamage();
    //            }
    //            else
    //            {
    //                Debug.LogError("Building1 component missing on the building!");
    //            }
    //        }
    //    }
    //}
}
