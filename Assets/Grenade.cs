using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    [Header("Explosion Settings")]
    public GameObject explosionPrefab;       // The explosion effect prefab
    public float explosionRadius = 5f;       // Radius in which enemies will take damage
    public int maxDamagePercentage = 100;    // Maximum damage at the center (percentage)
    public int minDamagePercentage = 10;     // Minimum damage at the edges (percentage)
    public string[] validTags = { "Building", "Enemy", "Ground" }; // Array of valid tags for explosion

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a valid tag
        foreach (string tag in validTags)
        {
            if (collision.gameObject.CompareTag(tag))
            {
                Explode(); // Trigger explosion if a valid tag is hit
                break; // Exit the loop once an explosion is triggered
            }
        }
    }

    private void Explode()
    {
        // Instantiate the explosion effect at the grenade's position
        GameObject bombEffect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(bombEffect, 2f); // Destroy explosion effect after 2 seconds

        // Find objects within the explosion radius
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        HashSet<GameObject> damagedEnemies = new HashSet<GameObject>();

        foreach (Collider nearbyObject in colliders)
        {
            // Apply damage only to objects with the specified target tag
            if (nearbyObject.CompareTag("Enemy"))
            {
                GameObject enemy = nearbyObject.gameObject;
                GameObject rootObject = enemy.transform.root.gameObject;
                if (damagedEnemies.Contains(rootObject))
                    continue;
                // Calculate distance to the enemy
                int distanceToEnemy = Mathf.RoundToInt(Vector3.Distance(transform.position, nearbyObject.transform.position));

                // Calculate damage percentage based on distance from explosion center
                int damagePercentage = Mathf.Clamp(
                    maxDamagePercentage - ((distanceToEnemy * maxDamagePercentage) / Mathf.RoundToInt(explosionRadius)),
                    minDamagePercentage,
                    maxDamagePercentage
                );

                // Apply damage if the object has a DetectBullet component
                DetectBullet detectBullet = rootObject.GetComponent<DetectBullet>();
                if (detectBullet != null)
                {
                    detectBullet.TakeingDamage(damagePercentage);
                    Debug.Log($"Enemy at distance {distanceToEnemy} - Damage: {damagePercentage}%");
                }
                damagedEnemies.Add(rootObject);
            }
        }

        // Destroy the grenade object after explosion
        Destroy(gameObject);
    }
}
