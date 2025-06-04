using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BombZombie : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator animator;
    private List<Transform> targets;
    public GameObject effect;
    
    public float maxTime = 1.0f;
    public float maxDistance = 1.0f;
    public float detectionRange = 5.0f;
    public float punchDistance = 0.8f;

    private float timer = 0.0f;
    public float timer1 = 5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        FindTargets();
    }

    private void OnValidate()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        RaycastHit hit;
        if (targets == null || targets.Count == 0)
        {
            Debug.LogError("No targets assigned or found.");
            return;
        }

        timer -= Time.deltaTime;
        if (timer < 0.0f)
        {
            Transform nearestTarget = null;
            float nearestDistanceSqr = Mathf.Infinity;

            foreach (Transform target in targets)
            {
                if (target != null)
                {
                    float sqrDistance = (target.position - agent.transform.position).sqrMagnitude;
                    if (sqrDistance < nearestDistanceSqr)
                    {
                        nearestDistanceSqr = sqrDistance;
                        nearestTarget = target;
                        //agent.transform.LookAt(nearestTarget.position);
                    }
                }
            }


            bool obstacleDetected = Physics.Raycast(transform.position, transform.forward, out hit, detectionRange);

            if (obstacleDetected && hit.collider != null)
            {
                agent.destination = hit.point;
            }
            else if (nearestTarget != null && nearestDistanceSqr > maxDistance)
            {
                agent.destination = nearestTarget.position;
            }
            timer = maxTime;
        }

        animator.SetFloat("Speed", agent.velocity.magnitude);

        HandleObstacleDetection();
    }

    private void HandleObstacleDetection()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, detectionRange))
        {
            
            if (hit.distance <= punchDistance && (hit.collider.gameObject.CompareTag("Target") || hit.collider.gameObject.CompareTag("Building")))
            {
                animator.SetBool("Dance", true);
                timer1 -= Time.deltaTime;
                if (timer1 < 0)
                {
                    Instantiate(effect, transform.position, transform.rotation);
                    Destroy(gameObject);
                }
            }
            else
            {
                animator.SetBool("Dance", false);
            }
        }
        else
        {
            animator.SetBool("Dance", false);
        }
    }

    public RaycastHit HitPoint(RaycastHit hit)
    {
        return hit;
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.gameObject.name);
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
            if (obj.layer == 12)
            {
                targets.Add(obj.transform);
            }
        }

        if (targets.Count == 0)
        {
            Debug.LogError("No targets found in the scene.");
        }
    }
}
