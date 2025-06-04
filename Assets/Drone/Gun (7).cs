using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    public GameObject droneaudio;
    public AudioSource end;


    public  MainScript audioscript;

    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private ParticleSystem ImpactParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private float ShootDelay = 0.1f;
    [SerializeField]
    private LayerMask Mask;
    [SerializeField]
    private LayerMask enemyLayer;

    [SerializeField]
    public float BulletSpeed = 100;

    [Range(0f, 1000f)]
    public int DamageBullet = 10;

    [SerializeField]
    public float aimAssistRadius = 200f;
    [SerializeField]
    public float horizontalAimAssistAngle = 45f; // Horizontal angle in degrees
    [SerializeField]
    public float verticalAimAssistAngle = 20f; // Vertical angle in degrees

    private float LastShootTime;
    private bool isGunOverheated = false;
    //private float spaceKeyHoldTime = 0f;
    private float cooldownTime = 2f;
    public float maxHoldTime = 3f;

    [SerializeField]
    private Slider overheatSlider;


    private Color greenColor = Color.green;
    private Color yellowColor = Color.yellow;
    private Color redColor = Color.red;

    [SerializeField]
    private Button fireButton;

    private bool isFireButtonPressed = false;
    private float fireButtonHoldTime = 0f;
    //public AudioSource gunsound;

    private void Start()
    {
        // Fire tugmasiga tinglovchilar qo'shish
        EventTrigger trigger = fireButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry();
        entryPointerDown.eventID = EventTriggerType.PointerDown;
        entryPointerDown.callback.AddListener((data) => { OnFireButtonDown((PointerEventData)data); });
        trigger.triggers.Add(entryPointerDown);

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry();
        entryPointerUp.eventID = EventTriggerType.PointerUp;
        entryPointerUp.callback.AddListener((data) => { OnFireButtonUp((PointerEventData)data); });
        trigger.triggers.Add(entryPointerUp);
    }

    private void OnFireButtonDown(PointerEventData eventData)
    {
        isFireButtonPressed = true;
        fireButtonHoldTime = 0f;
    }

    private void OnFireButtonUp(PointerEventData eventData)
    {
        isFireButtonPressed = false;
        fireButtonHoldTime = 0f;
    }

    private void Update()
    {
        if (isGunOverheated)
        {
            if (Time.time >= LastShootTime + cooldownTime)
            {
                isGunOverheated = false;
                droneaudio.SetActive(true);

                
                    overheatSlider.value = 0f;
                
    
            }
            else
            {
                return;
            }
        }

        if (isFireButtonPressed)
        {
            fireButtonHoldTime += Time.deltaTime;
            overheatSlider.value = Mathf.Clamp01(fireButtonHoldTime / maxHoldTime);

            if (fireButtonHoldTime >= maxHoldTime)
            {
                isGunOverheated = true;
                LastShootTime = Time.time;
                Debug.Log("Gun is overheated! Cooling down...");
                end.Play();
                droneaudio.SetActive(false);



            }
            else if (Time.time >= LastShootTime + ShootDelay)
            {
                Shoot();
                LastShootTime = Time.time;
            }
        }
        else
        {
            fireButtonHoldTime = Mathf.Max(fireButtonHoldTime - Time.deltaTime, 0f);
            overheatSlider.value = Mathf.Clamp01(fireButtonHoldTime / maxHoldTime);
        }

        UpdateSliderColor();
    }

    private Vector3 AimAssist()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, aimAssistRadius, enemyLayer);

        if (enemies.Length > 0)
        {
            Collider nearestEnemy = null;
            float nearestDistance = float.MaxValue;

            foreach (Collider enemy in enemies)
            {
                Vector3 directionToEnemy = enemy.transform.position - transform.position;
                float distanceToEnemy = directionToEnemy.magnitude;

                if (IsWithinAimAssistCone(directionToEnemy, horizontalAimAssistAngle, verticalAimAssistAngle))
                {
                    if (distanceToEnemy < nearestDistance && IsEnemyVisible(enemy))
                    {
                        nearestEnemy = enemy;
                        nearestDistance = distanceToEnemy;
                    }
                }
            }

            if (nearestEnemy != null)
            {
                Vector3 directionToEnemyCenter = (nearestEnemy.bounds.center - BulletSpawnPoint.position).normalized;
                return directionToEnemyCenter;
            }
        }

        return transform.forward;
    }

    private bool IsWithinAimAssistCone(Vector3 directionToEnemy, float horizontalAngle, float verticalAngle)
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        float horizontalDot = Vector3.Dot(Vector3.ProjectOnPlane(directionToEnemy, Vector3.up).normalized, forward);
        float verticalDot = Vector3.Dot(directionToEnemy.normalized, forward);

        float horizontalAngleRad = horizontalAngle * 0.5f * Mathf.Deg2Rad;
        float verticalAngleRad = verticalAngle * 0.5f * Mathf.Deg2Rad;

        return Mathf.Acos(horizontalDot) <= horizontalAngleRad && Mathf.Acos(verticalDot) <= verticalAngleRad;
    }

    private bool IsEnemyVisible(Collider enemy)
    {
        Vector3 directionToEnemy = enemy.bounds.center - BulletSpawnPoint.position;
        float distanceToEnemy = directionToEnemy.magnitude;

        if (Physics.Raycast(BulletSpawnPoint.position, directionToEnemy, out RaycastHit hit, distanceToEnemy, enemyLayer))
        {
            return hit.collider == enemy;
        }

        return false;
    }

    private void UpdateSliderColor()
    {
        if (overheatSlider.fillRect != null)
        {
            Image fillImage = overheatSlider.fillRect.GetComponent<Image>();
            if (fillImage != null)
            {
                float value = overheatSlider.value;

                if (value < 0.5f)
                {
                    fillImage.color = Color.Lerp(greenColor, yellowColor, value * 2f);
                }
                else
                {
                    fillImage.color = Color.Lerp(yellowColor, redColor, (value - 0.5f) * 2f);
                }
            }
        }
    }

    public void Shoot()
    {
        if (isGunOverheated)
        {
            return;
        }

        if (Time.time >= LastShootTime + ShootDelay)
        {
            //gunsound.Play();
            ShootingSystem.Play();

            Vector3 direction = AimAssist();
            RaycastHit hit;

            if (Physics.Raycast(BulletSpawnPoint.position, direction, out hit, float.MaxValue, Mask))
            {
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, hit.point, hit.normal, true));

                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    DetectBullet enemy = hit.collider.GetComponent<DetectBullet>();
                    if (enemy != null)
                    {
                        enemy.TakeingDamage(DamageBullet);
                    }
                }
                else
                {
                    // The bullet hit something that's not an enemy (could be a building or other obstacle)
                    Debug.Log($"Bullet hit a {hit.collider.gameObject.layer} at {hit.point}");
                }
            }
            else
            {
                // This part remains unchanged for shots that don't hit anything
                Vector3 farPoint = BulletSpawnPoint.position + direction * 1000f;
                TrailRenderer trail = Instantiate(BulletTrail, BulletSpawnPoint.position, Quaternion.identity);
                StartCoroutine(SpawnTrail(trail, farPoint, Vector3.zero, false));
            }

            LastShootTime = Time.time;
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = AimAssist();

        if (AddBulletSpread)
        {
            direction += new Vector3(
                                    Random.Range(-0.01f, 0.01f),
                                    Random.Range(-0.01f, 0.01f),
                                    Random.Range(-0.01f, 0.01f)
            );
            direction.Normalize();
        }

        return direction;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, Vector3 HitPoint, Vector3 HitNormal, bool MadeImpact)
    {
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, HitPoint);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, HitPoint, 1 - (remainingDistance / distance));

            remainingDistance -= BulletSpeed * Time.deltaTime;

            yield return null;
        }

        Trail.transform.position = HitPoint;
        if (MadeImpact)
        {
            Instantiate(ImpactParticleSystem, HitPoint, Quaternion.LookRotation(HitNormal));
        }

        Destroy(Trail.gameObject, Trail.time);
    }

    private void OnDrawGizmosSelected()
    {
        // Gizmo rangini yashil qilib belgilaymiz
        Gizmos.color = Color.green;

        // Aim assist radiusini sfera shaklida chizamiz
        Gizmos.DrawWireSphere(transform.position, aimAssistRadius);

        // Horizontal aim assist konusini chizamiz
        DrawAimAssistCone(transform.position, transform.forward, horizontalAimAssistAngle, verticalAimAssistAngle, Color.magenta);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(BulletSpawnPoint.position, AimAssist() * 100f);
    }

    private void DrawAimAssistCone(Vector3 position, Vector3 forward, float horizontalAngle, float verticalAngle, Color color)
    {
        // Konusning vertikal va horizontal radiusini hisoblash
        float horizontalRadius = Mathf.Tan(horizontalAngle * 0.5f * Mathf.Deg2Rad) * aimAssistRadius;
        float verticalRadius = Mathf.Tan(verticalAngle * 0.5f * Mathf.Deg2Rad) * aimAssistRadius;

        // Konusning yuqori chekkalarini aniqlash
        Vector3 topRight = position + (Quaternion.AngleAxis(horizontalAngle, Vector3.up) * Quaternion.AngleAxis(verticalAngle, Vector3.right) * forward * aimAssistRadius);
        Vector3 topLeft = position + (Quaternion.AngleAxis(-horizontalAngle, Vector3.up) * Quaternion.AngleAxis(verticalAngle, Vector3.right) * forward * aimAssistRadius);
        Vector3 bottomRight = position + (Quaternion.AngleAxis(horizontalAngle, Vector3.up) * Quaternion.AngleAxis(-verticalAngle, Vector3.right) * forward * aimAssistRadius);
        Vector3 bottomLeft = position + (Quaternion.AngleAxis(-horizontalAngle, Vector3.up) * Quaternion.AngleAxis(-verticalAngle, Vector3.right) * forward * aimAssistRadius);

        // Gizmos rangini sozlash
        Gizmos.color = color;

        // Konusning to'g'ri chiziqlarini chizish
        Gizmos.DrawLine(position, topRight);
        Gizmos.DrawLine(position, topLeft);
        Gizmos.DrawLine(position, bottomRight);
        Gizmos.DrawLine(position, bottomLeft);

        // Konusning chekkalarini bir-biriga ulash
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}

