using UnityEngine;
using System.Collections;
using System;

public class ParticleSystemManager : MonoBehaviour
{
    public static ParticleSystemManager Instance;
    // ParticleSystem arrays for different effects
    public ParticleSystem[] destruction;
    public ParticleSystem[] levelUp;
    public ParticleSystem[] buildRestore;
    public ParticleSystem[] isBuilding;
    public ParticleSystem[] isUpgrading;
    public ParticleSystem[] wallParticles;

    float delayTime = 2f;

    // Play and Stop methods for Destruction Particle System by index
    private void Awake()
    {
        Instance = this;
    }

    // Helper method to stop all particle systems in an array
    private void StopAllParticles(ParticleSystem[] particleSystems)
    {
        if (particleSystems == null) return;

        foreach (var particle in particleSystems)
        {
            if (particle != null)
            {
                particle.Stop();
            }
        }
    }

    public void PlayDestruction(int index)
    {
        if (IsValidIndex(destruction, index))
        {
            destruction[index].gameObject.SetActive(true);
            destruction[index].Play();
            // Start coroutine to stop the particle system after the delay
            StartCoroutine(StopWithDelay(destruction, index, delayTime));
        }
    }

    public void PlayLevelUp(int index)
    {
        if (IsValidIndex(levelUp, index))
        {
            levelUp[index].gameObject.SetActive(true);
            levelUp[index].Play();
            StartCoroutine(StopWithDelay(levelUp, index, delayTime));
        }
    }

    public void PlayBuildRestore(int index)
    {
        if (IsValidIndex(buildRestore, index))
        {
            buildRestore[index].gameObject.SetActive(true);
            buildRestore[index].Play();
            StartCoroutine(StopWithDelay(buildRestore, index, delayTime));
        }
    }

    // Play and Stop methods for IsBuilding Particle System by index
    public void PlayIsBuilding(int index)
    {
        if (IsValidIndex(isBuilding, index))
        {
            isBuilding[index].gameObject.SetActive(true);
        }
    }

    public void StopIsBuilding(int index)
    {
        if (IsValidIndex(isBuilding, index))
        {
            isBuilding[index].gameObject.SetActive(false);
        }
    }

    // Play and Stop methods for IsUpgrading Particle System by index
    public void PlayIsUpgrading(int index)
    {
        if (IsValidIndex(isUpgrading, index))
        {
            isUpgrading[index].gameObject.SetActive(true);
        }
    }

    public void StopIsUpgrading(int index)
    {
        if (IsValidIndex(isUpgrading, index))
        {
            isUpgrading[index].gameObject.SetActive(false);
        }
    }

    public void PlayWallUpgrade(Vector3 position, Quaternion rotation)
    {
        ParticleSystem particleSystem = Instantiate(wallParticles[1], position, rotation);
        Destroy(particleSystem, 2f);
    }

    public void PlayWallRestore(Vector3 position, Quaternion rotation)
    {
        ParticleSystem particleSystem = Instantiate(wallParticles[2], position, rotation);
        Destroy(particleSystem, 2f);
    }



    public void PlayWallDestroy(Vector3 position, Quaternion rotation)
    {
        ParticleSystem particleSystem = Instantiate(wallParticles[0], position, rotation);
        Destroy(particleSystem, 2f);
    }
    // Coroutine to stop the particle system after a delay
    private IEnumerator StopWithDelay(ParticleSystem[] particleSystems, int index, float delayTime)
    {
        yield return new WaitForSeconds(delayTime);  // Wait for the specified delay time
        if (IsValidIndex(particleSystems, index))
        {
            //particleSystems[index].Stop();  // Stop the particle effect
            particleSystems[index].gameObject.SetActive(false);
        }
    }

    // Helper function to validate index and ensure it is within the array bounds
    private bool IsValidIndex(ParticleSystem[] systems, int index)
    {
        return systems != null && index >= 0 && index < systems.Length;
    }

    public void MissilePurchased()
    {
        buildRestore[8].gameObject.SetActive(true);
        buildRestore[8].Play();
        StartCoroutine(StopWithDelay(buildRestore, 8, delayTime));
    }

}
