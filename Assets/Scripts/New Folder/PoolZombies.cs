using UnityEngine;
using System.Collections.Generic;

public class ZombiePool : MonoBehaviour
{
    public GameObject[] zombiePrefabs; // 0-2: Normal, 3: Giant, 4: Bomber
    public int maxNormalPoolSize = 30;
    public int maxGiantPoolSize = 10;
    public int maxBomberPoolSize = 10;

    private List<GameObject> normalPool = new List<GameObject>();
    private List<GameObject> giantPool = new List<GameObject>();
    private List<GameObject> bomberPool = new List<GameObject>();
    private List<GameObject> poolForAll = new List<GameObject>();

    private int totalNormalZombies = 0;
    private int totalGiantZombies = 0;
    private int totalBomberZombies = 0;

    private int zombieDropSoni = 0;
    private int totalSpawnedZombieDrops = 0;
    public int totalReleasedZombies = 0;
    void Start()
    {
        // Pre-populate each pool
        for (int i = 0; i < maxNormalPoolSize; i++)
        {
            GameObject zombie = InstantiateZombie(zombiePrefabs[Random.Range(0, 3)]);
            if (zombie != null)
            {
                normalPool.Add(zombie);
                poolForAll.Add(zombie);
                zombie.SetActive(false);
            }
        }

        for (int i = 0; i < maxGiantPoolSize; i++)
        {
            GameObject zombie = InstantiateZombie(zombiePrefabs[3]);
            if (zombie != null)
            {
                giantPool.Add(zombie);
                poolForAll.Add(zombie);
                zombie.SetActive(false);
            }
        }

        for (int i = 0; i < maxBomberPoolSize; i++)
        {
            GameObject zombie = InstantiateZombie(zombiePrefabs[4]);
            if (zombie != null)
            {
                bomberPool.Add(zombie);
                poolForAll.Add(zombie);
                zombie.SetActive(false);
            }
        }
    }

    // Instantiate method for creating a zombie
    private GameObject InstantiateZombie(GameObject prefab)
    {
        GameObject newZombie = Instantiate(prefab);
        newZombie.SetActive(false); // Initially inactive
        return newZombie;
    }

    // Get a Normal zombie from the pool
    public GameObject GetNormalZombie(Vector3 spawnPosition)
    {
        return GetZombieFromPool(normalPool, ref totalNormalZombies, maxNormalPoolSize, spawnPosition, zombiePrefabs[Random.Range(0, 3)]);
    }

    // Get a Giant zombie from the pool
    public GameObject GetGiantZombie(Vector3 spawnPosition)
    {
        return GetZombieFromPool(giantPool, ref totalGiantZombies, maxGiantPoolSize, spawnPosition, zombiePrefabs[3]);
    }

    // Get a Bomber zombie from the pool
    public GameObject GetBomberZombie(Vector3 spawnPosition)
    {
        return GetZombieFromPool(bomberPool, ref totalBomberZombies, maxBomberPoolSize, spawnPosition, zombiePrefabs[4]);
    }

    public void GetZombieDrop()
    {
        zombieDropSoni = WaveManager.Instance.zombieDropSoni;
    }

    // Generalized method to get a zombie from the specific pool
    private GameObject GetZombieFromPool(List<GameObject> pool, ref int totalCreated, int maxPoolSize, Vector3 spawnPosition, GameObject prefab)
    {
        foreach (GameObject zombie in pool)
        {
            if (!zombie.activeInHierarchy)
            {
                zombie.transform.position = spawnPosition;
                zombie.SetActive(true);
                return zombie;
            }
        }

        if (totalCreated < maxPoolSize)
        {
            GameObject newZombie = InstantiateZombie(prefab);
            pool.Add(newZombie);
            newZombie.transform.position = spawnPosition;
            newZombie.SetActive(true);
            totalCreated++;
            return newZombie;
        }

        Debug.LogWarning("Pool limit reached for zombie type!");
        return null;
    }

    // Release a zombie back to the pool
    public void ReleaseZombie(GameObject zombie)
    {
        if (zombie != null)
        {
            if (WaveManager.Instance.drops.Contains(totalReleasedZombies))
            {
                CallSpawnZombieDrop(zombie);
            }

            zombie.SetActive(false); // Deactivate the zombie
            zombie.GetComponent<DetectBullet>().ResetHealth();
            totalReleasedZombies++;
            WaveManager.Instance.totalZombies--;
            WaveManager.Instance.ShowTotalActiveZombies();

        }
    }
    public void ReleaseAllZombies()
    {
        foreach (GameObject zombie in poolForAll)
        {
            zombie.GetComponent<DetectBullet>().ResetHealth();
            zombie.SetActive(false);

        }
    }

    private void CallSpawnZombieDrop(GameObject zombie)
    {

        Vector3 zombiePosition = zombie.transform.position;

        if (zombieDropSoni > totalSpawnedZombieDrops)
        {
            WaveManager.Instance.SpawnZombieDrop(zombiePosition);
            totalSpawnedZombieDrops++;
        }

    }
}
