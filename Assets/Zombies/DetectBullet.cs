using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectBullet : MonoBehaviour
{
    [Range(0, 1000)]
    public int health = 100;
    public int restoreHealth;
    private Vector3 newPosition;
    [SerializeField] private GameObject bloodEffectPrefab;
    [SerializeField] private GameObject deadEffect;
    [SerializeField] private string bulletTag = "Bullet";
    public GameObject healthBar;

    // Reference to the zombie pool to release the zombie
    private ZombiePool zombiePool;

    private void Start()
    {
        healthBar.GetComponent<HealthScript>().SetMaxHealth(health);
        restoreHealth = health;

        // Find the ZombiePool in the scene (you can assign it directly if it's known)
        zombiePool = FindObjectOfType<ZombiePool>();
    }

    private void Update()
    {
        healthBar.GetComponent<HealthScript>().SetHealth(health);
    }

    public void TakeingDamage(int damage)
    {
        healthBar.SetActive(true);
        health -= damage;
        //Debug.Log("Hit! Damage: " + damage);

        if (health <= 0)
        {
            HandleDeath();
        }
    }

    private void HandleDeath()
    {
        // Set the position for the dead effect
        newPosition = transform.position;
        newPosition.y += 2;

        //Debug.Log("Enemy destroyed");
        Instantiate(deadEffect, newPosition, transform.rotation);

        // Release the zombie back to the pool
        if (zombiePool != null)
        {
            zombiePool.ReleaseZombie(gameObject);
        }
        else
        {
            //Debug.LogError("ZombiePool reference not set!");
        }
    }

    public void OnHit(Vector3 hitPoint, Vector3 hitNormal)
    {
        // Instantiate the blood effect at the hit point
        Quaternion rotation = Quaternion.LookRotation(hitNormal);
        Instantiate(bloodEffectPrefab, hitPoint, rotation);
    }

    public void ResetHealth()
    {
        health = restoreHealth;
    }
}
