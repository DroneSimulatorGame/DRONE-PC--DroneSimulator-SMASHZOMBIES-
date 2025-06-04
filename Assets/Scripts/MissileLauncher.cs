using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MissileLauncher : MonoBehaviour
{
    public float maxTurnSpeed = 90f;
    public float maxDetectionRadius = 50f;
    public float minDetectionRadius = 5f;
    public Vector3 rotationOffset;
    public LayerMask enemyLayerMask;
    public GameObject missilePrefab;
    public GameObject explosionPrefab; // Reference to the explosion prefab
    public List<Transform> spawners;
    public float fireCooldown = 2f;
    public float initialMissileSpeed = 50f;
    public float finalMissileSpeed = 75f;
    public Vector3 missileScale = new Vector3(1f, 1f, 1f);
    public float initialStraightDistance = 10f;
    public float acceptanceRadius = 1f;
    public int maxMissileCapacity = 10;
    private int currentMissileCapacity;
    private int sana = 0;
    public Transform intermediatePoint;

    public float impactRadius = 10f;
    public int maxDamage = 100;

    private Transform target;
    private bool aimed;
    private float cooldownTimer = 0f;

    // Store the last missile impact position for visualization
    private Vector3 lastImpactPosition;
    private bool impactHappened = false;

    // Audio components
    public AudioClip rotationSound; // Rotation sound clip
    public AudioSource audioSource; // Audio source component

    // Public variable for the duration of the THRUSTER VFX after the missile hit
    public float thrusterVFXDuration = 2f; // Set a default value

    // Public variable for the duration of the EXPLOSION prefab after instantiation
    public float explosionDuration = 2f; // Set a default value for explosion duration

    private void Start()
    {
        currentMissileCapacity = maxMissileCapacity;
        audioSource = gameObject.AddComponent<AudioSource>(); // Add AudioSource component
        audioSource.clip = rotationSound; // Assign the rotation sound clip
        audioSource.volume = 0.05f;
        audioSource.loop = true; // Set the audio to loop
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;
        FindClosestTarget();

        //Debug.Log($"Current Target: {target?.name}, Cooldown Timer: {cooldownTimer}, Missile Capacity: {currentMissileCapacity}");

        if (target != null)
        {
            AimAtTarget();

            if (aimed && cooldownTimer >= fireCooldown && currentMissileCapacity > 0)
            {
                LaunchMissile();
                cooldownTimer = 0f;
                currentMissileCapacity--;
            }
        }
        else
        {
            // Stop sound when there is no target
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }

    //----------------------------
    public void ResetMissileCapacity()
    {
        currentMissileCapacity = maxMissileCapacity;
        //Debug.Log("Missile capacity reset to maximum.");
    }









    // Updated FindClosestTarget method
    private void FindClosestTarget()
    {
        // Find all enemies within the max detection radius
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, maxDetectionRadius, enemyLayerMask);
        List<Transform> detectedEnemies = new List<Transform>();

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy"))
            {
                // Calculate the distance from the launcher to the enemy
                float distanceToEnemy = Vector3.Distance(transform.position, hitCollider.transform.position);

                // Add enemies only if they are outside the min detection radius
                if (distanceToEnemy >= minDetectionRadius)
                {
                    detectedEnemies.Add(hitCollider.transform);
                }
            }
        }

        // If there are detected enemies, target the closest one
        if (detectedEnemies.Count > 0)
        {
            // Sort enemies by distance to find the closest one
            detectedEnemies.Sort((a, b) => Vector3.Distance(transform.position, a.position)
                .CompareTo(Vector3.Distance(transform.position, b.position)));

            target = detectedEnemies[0]; // Target the closest enemy
        }
        else
        {
            target = null; // No targets found
        }
    }

    private void AimAtTarget()
    {
        if (target == null) return;

        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(direction, transform.up);
        targetRotation *= Quaternion.Euler(rotationOffset);

        aimed = false;
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
        {
            aimed = true;
        }

        // Rotate the launcher and play sound while rotating
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxTurnSpeed * Time.deltaTime);

        // Check if we are rotating and play sound
        if (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f && !audioSource.isPlaying)
        {
            audioSource.Play(); // Start playing the rotation sound
        }
        else if (Quaternion.Angle(transform.rotation, targetRotation) <= 0.1f && audioSource.isPlaying)
        {
            audioSource.Stop(); // Stop playing the rotation sound
        }
    }

    private void LaunchMissile()
    {
        if (intermediatePoint == null)
        {
            //Debug.LogWarning("No intermediate point assigned!");
            return;
        }

        if (spawners.Count == 0)
        {
            //Debug.LogWarning("No missile spawners assigned!");
            return;
        }

        Transform randomSpawner = spawners[Random.Range(0, spawners.Count)];
        GameObject missileInstance = Instantiate(missilePrefab, randomSpawner.position, randomSpawner.rotation);
        missileInstance.transform.localScale = missileScale;

        StartCoroutine(SendHoming(missileInstance));
    }

    private IEnumerator SendHoming(GameObject missile)
    {
        Vector3 startPosition = missile.transform.position;
        Vector3 straightDirection = missile.transform.forward;
        float currentSpeed = initialMissileSpeed;

        while (Vector3.Distance(startPosition, missile.transform.position) < initialStraightDistance)
        {
            missile.transform.position += straightDirection * currentSpeed * Time.deltaTime;
            missile.transform.rotation = Quaternion.Slerp(missile.transform.rotation, Quaternion.LookRotation(straightDirection), 0.1f);
            yield return null;
        }

        while (Vector3.Distance(intermediatePoint.position, missile.transform.position) > acceptanceRadius)
        {
            Vector3 directionToIntermediate = (intermediatePoint.position - missile.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToIntermediate);
            missile.transform.rotation = Quaternion.Slerp(missile.transform.rotation, targetRotation, 0.1f);
            missile.transform.position += missile.transform.forward * currentSpeed * Time.deltaTime;
            yield return null;
        }

        currentSpeed = finalMissileSpeed;

        while (target != null && Vector3.Distance(target.position, missile.transform.position) > acceptanceRadius)
        {
            Vector3 directionToTarget = (target.position - missile.transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            missile.transform.rotation = Quaternion.Slerp(missile.transform.rotation, targetRotation, 0.1f);
            missile.transform.position += missile.transform.forward * currentSpeed * Time.deltaTime;
            yield return null;
        }

        ApplyExplosionDamage(missile.transform.position);

        // Instantiate explosion prefab at the missile's final position
        GameObject explosionInstance = Instantiate(explosionPrefab, missile.transform.position, Quaternion.identity);
        StartCoroutine(HandleExplosionVFX(explosionInstance)); // Start coroutine for explosion duration

        // Detach the Thruster VFX from the missile before destruction
        Transform thrusterVFX = missile.transform.Find("ThrusterVFX"); // Replace with your actual VFX child name
        if (thrusterVFX != null)
        {
            thrusterVFX.SetParent(null); // Detach from the missile
            StartCoroutine(HandleThrusterVFX(thrusterVFX.gameObject)); // Start coroutine for VFX duration
        }

        Destroy(missile); // Now destroy the missile
    }

    private IEnumerator HandleThrusterVFX(GameObject thrusterVFX)
    {
        // Wait for the specified duration before destroying the VFX
        yield return new WaitForSeconds(thrusterVFXDuration);
        Destroy(thrusterVFX); // Destroy the VFX
    }

    private IEnumerator HandleExplosionVFX(GameObject explosion)
    {
        // Wait for the specified duration before destroying the explosion
        yield return new WaitForSeconds(explosionDuration);
        Destroy(explosion); // Destroy the explosion
    }

    private void ApplyExplosionDamage(Vector3 explosionPosition)
    {
        Collider[] hitColliders = Physics.OverlapSphere(explosionPosition, impactRadius, enemyLayerMask);
        HashSet<GameObject> damagedEnemies = new HashSet<GameObject>(); // Track already damaged enemies

        foreach (var hitCollider in hitColliders)
        {
            GameObject enemy = hitCollider.gameObject;

            // Ensure we are referencing the "root" GameObject (in case the collider is on a child object)
            GameObject rootObject = enemy.transform.root.gameObject;

            if (damagedEnemies.Contains(rootObject))
                continue; // Skip if this enemy was already damaged

            float distanceToTarget = Vector3.Distance(explosionPosition, rootObject.transform.position);
            DetectBullet detectBullet = rootObject.GetComponent<DetectBullet>();

            if (detectBullet != null)
            {
                float damageMultiplier = 1 - (distanceToTarget / impactRadius);
                int damageToApply = Mathf.RoundToInt(maxDamage * damageMultiplier);
                detectBullet.TakeingDamage(damageToApply);
                Debug.Log($"Damage applied to {rootObject.name}: {damageToApply}");
            }

            damagedEnemies.Add(rootObject); // Mark this enemy as damaged
        }

        lastImpactPosition = explosionPosition;
        impactHappened = true;
    }




    //private void ApplyExplosionDamage(Vector3 explosionPosition)
    //{
    //    Collider[] hitColliders = Physics.OverlapSphere(explosionPosition, impactRadius, enemyLayerMask);

    //    foreach (var hitCollider in hitColliders)
    //    {
    //        float distanceToTarget = Vector3.Distance(explosionPosition, hitCollider.transform.position);

    //        DetectBullet detectBullet = hitCollider.GetComponent<DetectBullet>();

    //        if (detectBullet != null)
    //        {
    //            float damageMultiplier = 1 - (distanceToTarget / impactRadius);
    //            int damageToApply = Mathf.RoundToInt(maxDamage * damageMultiplier);

    //            sana += 1;
    //            if (sana == 2)
    //            {
    //                detectBullet.TakeingDamage(damageToApply);
    //                Debug.Log("Damage applied: " + damageToApply);
    //                sana = 0;
    //            }

    //        }
    //    }

    //    // Store the explosion position for impact radius visualization
    //    lastImpactPosition = explosionPosition;
    //    impactHappened = true;
    //}

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, maxDetectionRadius);

        // Draw the min detection radius in blue
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minDetectionRadius);

        if (impactHappened)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(lastImpactPosition, impactRadius);
        }
    }
}
