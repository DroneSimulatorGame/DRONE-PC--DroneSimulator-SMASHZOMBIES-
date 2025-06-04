using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI;
using UnityEngine.AI;
public class AgentAI : MonoBehaviour
{

    public List<Transform> waypoints;
    NavMeshAgent agent;
    public int currentIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        Walking();
    }


    void Walking()
    {
        if (waypoints.Count == 0)
        {
            return;
        }

        float distancetowaypoint = Vector3.Distance(waypoints[currentIndex].position, transform.position);

        if (distancetowaypoint <= 5)
        {
            currentIndex = (currentIndex + 1) % waypoints.Count;
        }

        agent.SetDestination(waypoints[currentIndex].position);

    }

}
