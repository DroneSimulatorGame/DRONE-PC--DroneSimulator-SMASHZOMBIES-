using System.Collections;
using UnityEngine;

public class EndlessSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] spawnPoints;       // Array of spawn points
    public GameObject prefab1;             // First prefab to spawn
    public GameObject prefab2;             // Second prefab to spawn
    public float spawnInterval = 2f;       // Time interval between each spawn

    private bool isSpawning = false;

    // Method to start the spawning process, call this from an event
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnObjects());
        }
    }

    // Coroutine to spawn objects endlessly at intervals
    private IEnumerator SpawnObjects()
    {
        while (isSpawning)
        {
            // Choose a random spawn point
            GameObject randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

            // Randomly choose one of the two prefabs to spawn
            GameObject prefabToSpawn = (Random.value < 0.5f) ? prefab1 : prefab2;

            // Spawn the chosen prefab at the random spawn point's position
            Instantiate(prefabToSpawn, randomSpawnPoint.transform.position, Quaternion.identity);

            // Wait for the specified interval before spawning again
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Optional: Method to stop spawning, if needed
    public void StopSpawning()
    {
        isSpawning = false;
        StopCoroutine(SpawnObjects());
    }
}
