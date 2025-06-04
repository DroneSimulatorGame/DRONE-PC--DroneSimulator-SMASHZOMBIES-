using System.Collections.Generic;
using UnityEngine;

public class TrailPool : MonoBehaviour
{
    public static TrailPool Instance;

    public TrailRenderer trailPrefab;
    public int poolSize = 10;

    private Queue<TrailRenderer> trailPool;

    void Awake()
    {
        Instance = this;
        trailPool = new Queue<TrailRenderer>();

        // Pre-instantiate trail objects
        for (int i = 0; i < poolSize; i++)
        {
            TrailRenderer trail = Instantiate(trailPrefab);
            trail.gameObject.SetActive(false);
            trailPool.Enqueue(trail);
        }
    }

    public TrailRenderer GetTrail()
    {
        if (trailPool.Count > 0)
        {
            TrailRenderer trail = trailPool.Dequeue();
            trail.gameObject.SetActive(true);
            return trail;
        }
        else
        {
            // Optionally, create a new trail if the pool is empty
            TrailRenderer newTrail = Instantiate(trailPrefab);
            return newTrail;
        }
    }

    public void ReturnTrail(TrailRenderer trail)
    {
        trail.gameObject.SetActive(false);
        trailPool.Enqueue(trail);
    }
}
