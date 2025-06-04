using System.Collections;
using UnityEngine;

public class DefenseTower : MonoBehaviour
{
    [Header("Attributes")]
    public float range = 10f;
    public float minRange = 3f;
    public float fireRate = 1f;
    private float fireCountdown = 0f;
    public int damage = 10;

    [Header("Unity Setup Fields")]
    public LayerMask enemyLayer; // Use LayerMask to define enemy layers
    public Transform partToRotate;
    public float turnSpeed = 10f;
    public Transform firePoint;
    public TrailRenderer bulletTrailPrefab;
    public float bulletSpeed = 100f;

    private GameObject target;
    private Vector3 raycastEndPoint;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        InvokeRepeating("UpdateTarget", 0f, 0.5f);
    }

    void UpdateTarget()
    {
        Collider[] enemiesInRange = Physics.OverlapSphere(transform.position, range, enemyLayer);
        float shortestDistance = Mathf.Infinity;
        GameObject nearestEnemy = null;

        foreach (Collider enemyCollider in enemiesInRange)
        {
            GameObject enemy = enemyCollider.gameObject;
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            // Ignore enemies within minRange
            if (distanceToEnemy < shortestDistance && distanceToEnemy > minRange)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        target = (nearestEnemy != null && shortestDistance <= range) ? nearestEnemy : null;
    }

    void Update()
    {
        if (target == null)
        {
            audioSource.Stop();
            return;
        }

        RotateTowardsTarget();

        if (fireCountdown <= 0f)
        {
            Shoot();
            fireCountdown = 1f / fireRate;
        }

        fireCountdown -= Time.deltaTime;
    }

    void RotateTowardsTarget()
    {
        Collider targetCollider = target.GetComponent<Collider>();
        Vector3 targetCenter = targetCollider.bounds.center;

        Vector3 direction = targetCenter - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
        partToRotate.rotation = Quaternion.Euler(90f, rotation.y, 0f);
    }


    void Shoot()
    {
        audioSource.Play();

        // Use the collider center instead of an offset
        Collider targetCollider = target.GetComponent<Collider>();
        Vector3 targetCenter = targetCollider.bounds.center;  // Get the center of the target collider

        Vector3 direction = targetCenter - firePoint.position;

        // Perform raycast towards the target center
        if (Physics.Raycast(firePoint.position, direction, out RaycastHit hit, range, enemyLayer))
        {
            HandleImpact(hit);
            LaunchTrail(hit, hit.point, hit.normal);
        }
        else
        {
            // If the raycast doesn't hit the target, assume it missed
            Vector3 missPoint = firePoint.position + direction.normalized * range;
            LaunchTrail(hit, missPoint, Vector3.zero);
        }
    }


    private void HandleImpact(RaycastHit hit)
    {
        DetectBullet enemy = hit.collider.GetComponent<DetectBullet>();
        if (enemy != null)
        {
            enemy.TakeingDamage(damage);
        }
    }

    private void LaunchTrail(RaycastHit hit, Vector3 hitPoint, Vector3 hitNormal)
    {
        TrailRenderer trail = Instantiate(bulletTrailPrefab);
        trail.transform.position = firePoint.position;
        StartCoroutine(SpawnTrail(trail, hit, hitPoint, hitNormal));
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit, Vector3 hitPoint, Vector3 hitNormal)
    {
        DetectBullet enemy = hit.collider.GetComponent<DetectBullet>();
        Vector3 startPosition = trail.transform.position;
        float distance = Vector3.Distance(startPosition, hitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hitPoint, 1 - (remainingDistance / distance));
            remainingDistance -= bulletSpeed * Time.deltaTime;
            yield return null;
        }

        trail.transform.position = hitPoint;
        if (enemy != null)
        {
            enemy.OnHit(hit.point, hit.normal);
        }
        Destroy(trail);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, minRange);

        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(firePoint.position, raycastEndPoint);
        }
    }
}
