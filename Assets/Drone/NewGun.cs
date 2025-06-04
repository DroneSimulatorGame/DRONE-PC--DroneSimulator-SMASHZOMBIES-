using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NewGun : MonoBehaviour
{
    [SerializeField] private CheckUI uiManager;

    #region Audio Components
    [Header("Audio")]
    [SerializeField] private GameObject droneAudio;
    [SerializeField] private AudioSource endSound;
    [SerializeField] private int gunSoundIndex = 1;
    private bool isSoundPlaying = false;
    #endregion

    #region Gun Properties
    [Header("Gun Properties")]
    [SerializeField] private bool addBulletSpread = true;
    [SerializeField] private float shootDelay = 0.1f;
    public float bulletSpeed = 100f;
    [Range(0f, 1000f)]
    public int damageBullet = 10;
    #endregion

    #region Particle Systems
    [Header("Particle Systems")]
    [SerializeField] private ParticleSystem shootingSystem;
    [SerializeField] private ParticleSystem impactParticleSystem;
    [SerializeField] private ParticleSystem overHeatParticle;
    [SerializeField] private GameObject groundImpactPrefab;
    [SerializeField] private GameObject metalImpactPrefab;
    private Vector3 direction;
    #endregion

    #region References
    [Header("References")]
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private TrailRenderer bulletTrail;
    [SerializeField] private LayerMask mask;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private Slider overheatSlider;
    [SerializeField] private Button fireButton;
    [SerializeField] private Image crosshair;
    [SerializeField] private Camera mainCamera;
    #endregion

    #region Aim Assist Settings
    [Header("Aim Assist Settings")]
    [SerializeField] private float aimAssistRadius = 0f;
    [SerializeField] private float horizontalAimAssistAngle = 0f;
    [SerializeField] private float verticalAimAssistAngle = 0f;
    [SerializeField] private bool useAimAssist = false;
    #endregion

    #region Heat System Settings
    [Header("Heat System Settings")]
    public float maxHeatTime = 3f;
    [SerializeField] private float cooldownTime = 2f;
    public float cooldownSpeed = 0.5f;
    [SerializeField] private float gradualCooldownSpeed = 0.2f;
    #endregion

    #region Crosshair Settings
    [Header("Crosshair Settings")]
    [SerializeField] private Color normalCrosshairColor = Color.white;
    [SerializeField] private Color enemyTargetedColor = Color.red;
    [SerializeField] public GameObject bulletTegdi;
    [SerializeField] private float bulletTegdiShowTime = 0.3f;
    #endregion

    #region Private Fields
    private float lastShootTime;
    private bool isGunOverheated;
    private bool isFireButtonPressed;
    private bool isMouseButtonPressed;
    private bool mouseClickedOnce;
    private float currentHeatValue;
    private float mouseClickTimer;
    private bool isEnemyInCrosshair = false;

    private readonly Color greenColor = Color.green;
    private readonly Color yellowColor = Color.yellow;
    private readonly Color redColor = Color.red;
    #endregion

    #region Unity Methods
    private void Start()
    {
        if (uiManager == null)
        {
            uiManager = CheckUI.Instance;
            if (uiManager == null)
            {
                Debug.LogWarning("UIManager not found. Creating one.");
                GameObject uiManagerObject = new GameObject("UIManager");
                uiManager = uiManagerObject.AddComponent<CheckUI>();
            }
        }

        SetupFireButton();
        ResetHeatValues();
        mouseClickTimer = 0f;

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (bulletTegdi != null)
        {
            bulletTegdi.SetActive(false);
        }
    }

    private void Update()
    {
        bool isAnyPanelOpen = uiManager.IsAnyPanelOpen();

        if (isAnyPanelOpen)
        {
            StopGunSound();
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            GrenadeLauncher.Instance.LaunchGrenade();
        }

        if (Input.GetMouseButtonDown(0))
        {
            isMouseButtonPressed = true;
            mouseClickedOnce = true;
            mouseClickTimer = 0f;
            PlayGunSound();
        }

        if (Input.GetMouseButton(0) && isMouseButtonPressed)
        {
            mouseClickTimer += Time.deltaTime;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isMouseButtonPressed = false;
            mouseClickedOnce = false;
            StopGunSound();
        }

        HandleGunSystem();
        UpdateSliderColor();
        direction = GetCrosshairDirection();
        UpdateCrosshairColor();
    }

    private void OnDisable()
    {
        StopGunSound();
    }
    #endregion

    #region Audio Control
    private void PlayGunSound()
    {
        if (!isSoundPlaying && !isGunOverheated)
        {
            MainScript.instance.PressBtn(gunSoundIndex);
            isSoundPlaying = true;
            Debug.Log("Gun sound started");
        }
    }

    private void StopGunSound()
    {
        if (isSoundPlaying)
        {
            MainScript.instance.ReleaseBtn(gunSoundIndex);
            isSoundPlaying = false;
            Debug.Log("Gun sound stopped");
        }
    }
    #endregion

    #region Crosshair System
    private Vector3 GetCrosshairDirection()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        isEnemyInCrosshair = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, enemyLayer);

        if (Physics.Raycast(ray, out hit, float.MaxValue, mask))
        {
            return (hit.point - bulletSpawnPoint.position).normalized;
        }

        return ray.direction;
    }

    private void UpdateCrosshairColor()
    {
        if (crosshair != null)
        {
            crosshair.color = isEnemyInCrosshair ? enemyTargetedColor : normalCrosshairColor;
        }
    }
    #endregion

    #region Heat System
    private void HandleGunSystem()
    {
        if (isGunOverheated)
        {
            StopGunSound();
            overHeatParticle.Play();
            HandleOverheatedState();
            return;
        }

        bool shouldFire = isFireButtonPressed ||
                          (mouseClickedOnce && mouseClickTimer <= shootDelay) ||
                          (isMouseButtonPressed && mouseClickTimer > shootDelay);

        if (shouldFire)
        {
            HandleFiring();

            if (mouseClickedOnce && mouseClickTimer <= shootDelay && !isMouseButtonPressed)
            {
                mouseClickedOnce = false;
            }
        }
        else
        {
            HandleCooldown();
        }

        UpdateHeatSlider();
    }

    private void HandleOverheatedState()
    {
        float timeSinceOverheat = Time.time - lastShootTime;

        if (timeSinceOverheat >= cooldownTime)
        {
            StartGradualCooldown();
            droneAudio?.SetActive(true);
            overHeatParticle.gameObject?.SetActive(true);
            Debug.Log("Gun sound 2");
        }
    }

    private void StartGradualCooldown()
    {
        isGunOverheated = false;
        currentHeatValue = maxHeatTime;
        UpdateHeatSlider();
    }

    private void HandleFiring()
    {
        currentHeatValue += Time.deltaTime;
        currentHeatValue = Mathf.Min(currentHeatValue, maxHeatTime);

        if (currentHeatValue >= maxHeatTime)
        {
            TriggerOverheat();
        }
        else if (CanShoot())
        {
            Shoot();
            lastShootTime = Time.time;
        }
    }

    private void HandleCooldown()
    {
        if (currentHeatValue > 0)
        {
            float cooldownMultiplier = isGunOverheated ? gradualCooldownSpeed : cooldownSpeed;
            currentHeatValue -= Time.deltaTime * cooldownMultiplier;
            currentHeatValue = Mathf.Max(currentHeatValue, 0f);
        }
    }

    private void TriggerOverheat()
    {
        isGunOverheated = true;
        lastShootTime = Time.time;
        endSound?.Play();
        StopGunSound();
        droneAudio?.SetActive(false);
        overHeatParticle.gameObject?.SetActive(false);
        Debug.Log("Gun Sound ");
    }

    private bool CanShoot()
    {
        return Time.time >= lastShootTime + shootDelay && !isGunOverheated;
    }

    private void ResetHeatValues()
    {
        currentHeatValue = 0f;
        isGunOverheated = false;
        overheatSlider.value = 0f;
    }

    private void UpdateHeatSlider()
    {
        overheatSlider.value = currentHeatValue / maxHeatTime;
    }
    #endregion

    #region Shooting System
    public void Shoot()
    {
        if (isGunOverheated) return;

        shootingSystem.Play();

        // Otish yo'nalishi - prisel turgan joy
        Vector3 shootDirection = direction;

        // Agar aim assist yoqilgan bo'lsa va dushman priselda bo'lmasa
        if (useAimAssist && !isEnemyInCrosshair)
        {
            Vector3 aimAssistDirection = AimAssist();
            if (aimAssistDirection != GetDirection())
            {
                shootDirection = aimAssistDirection;
            }
        }

        // Apply bullet spread
        Vector3 finalDirection = shootDirection;
        if (addBulletSpread)
        {
            finalDirection += new Vector3(
                Random.Range(-0.005f, 0.005f),
                Random.Range(-0.005f, 0.005f),
                Random.Range(-0.005f, 0.005f)
            );
            finalDirection.Normalize();
        }

        // Use the spread direction for both visuals and damage
        if (Physics.Raycast(bulletSpawnPoint.position, finalDirection, out RaycastHit hit, float.MaxValue, mask))
        {
            TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit, hit.point, hit.normal, true));
            PlayImpactParticle(hit);

            // Check if the hit was an enemy
            if (hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hit.collider.TryGetComponent<DetectBullet>(out DetectBullet enemy))
                {
                    if (bulletTegdi != null)
                    {
                        bulletTegdi.SetActive(true);
                    }

                    Debug.Log("Dushmanga o'q tegdi urraaaaaa!!!!!");
                    enemy.TakeingDamage(damageBullet);

                    Invoke(nameof(DisableBulletTegdi), bulletTegdiShowTime);
                }
            }
        }
        else
        {
            HandleBulletMiss(finalDirection);
        }
    }

    private void DisableBulletTegdi()
    {
        if (bulletTegdi != null)
        {
            bulletTegdi.SetActive(false);
        }
    }

    private void HandleBulletMiss(Vector3 direction)
    {
        Vector3 farPoint = bulletSpawnPoint.position + direction * 1000f;
        TrailRenderer trail = Instantiate(bulletTrail, bulletSpawnPoint.position, Quaternion.identity);
        StartCoroutine(SpawnTrail(trail, new RaycastHit(), farPoint, Vector3.zero, false));
    }
    #endregion

    #region Aim Assist
    private Vector3 AimAssist()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, aimAssistRadius, enemyLayer);

        if (enemies.Length > 0)
        {
            Collider nearestEnemy = FindNearestEnemy(enemies);
            if (nearestEnemy != null)
            {
                return (nearestEnemy.bounds.center - bulletSpawnPoint.position).normalized;
            }
        }

        return GetDirection();
    }

    private Collider FindNearestEnemy(Collider[] enemies)
    {
        Collider nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (Collider enemy in enemies)
        {
            Vector3 directionToEnemy = enemy.transform.position - transform.position;
            float distanceToEnemy = directionToEnemy.magnitude;

            bool isWithinAimAssist = IsWithinAimAssistCone(directionToEnemy);
            bool isVisible = IsEnemyVisible(enemy);

            Transform targetTransform = enemy.transform.Find("target");
            if (targetTransform != null)
            {
                GameObject targetObject = targetTransform.gameObject;

                if (isWithinAimAssist && isVisible)
                {
                    targetObject.SetActive(true);
                }
                else
                {
                    targetObject.SetActive(false);
                }
            }

            if (isWithinAimAssist &&
                distanceToEnemy < nearestDistance &&
                isVisible)
            {
                nearestEnemy = enemy;
                nearestDistance = distanceToEnemy;
            }
        }

        return nearestEnemy;
    }

    private bool IsWithinAimAssistCone(Vector3 directionToEnemy)
    {
        Vector3 forward = transform.forward;

        float horizontalDot = Vector3.Dot(Vector3.ProjectOnPlane(directionToEnemy, Vector3.up).normalized, forward);
        float verticalDot = Vector3.Dot(directionToEnemy.normalized, forward);

        float horizontalAngleRad = horizontalAimAssistAngle * 0.5f * Mathf.Deg2Rad;
        float verticalAngleRad = verticalAimAssistAngle * 0.5f * Mathf.Deg2Rad;

        return Mathf.Acos(horizontalDot) <= horizontalAngleRad &&
               Mathf.Acos(verticalDot) <= verticalAngleRad;
    }

    private bool IsEnemyVisible(Collider enemy)
    {
        Vector3 directionToEnemy = enemy.bounds.center - bulletSpawnPoint.position;
        float distanceToEnemy = directionToEnemy.magnitude;

        return Physics.Raycast(bulletSpawnPoint.position, directionToEnemy, out RaycastHit hit, distanceToEnemy, enemyLayer) &&
               hit.collider == enemy;
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = transform.forward;

        if (addBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-0.015f, 0.015f),
                Random.Range(-0.015f, 0.015f),
                Random.Range(-0.015f, 0.015f)
            );
            direction.Normalize();
        }

        return direction;
    }
    #endregion

    #region UI Controls
    private void SetupFireButton()
    {
        if (fireButton == null || fireButton.gameObject == null)
            return;

        EventTrigger trigger = fireButton.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = fireButton.gameObject.AddComponent<EventTrigger>();
        }

        trigger.triggers.Clear();

        EventTrigger.Entry entryPointerDown = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerDown
        };
        entryPointerDown.callback.AddListener((data) => { OnFireButtonDown((PointerEventData)data); });
        trigger.triggers.Add(entryPointerDown);

        EventTrigger.Entry entryPointerUp = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entryPointerUp.callback.AddListener((data) => { OnFireButtonUp((PointerEventData)data); });
        trigger.triggers.Add(entryPointerUp);
    }

    private void UpdateSliderColor()
    {
        if (overheatSlider == null || overheatSlider.fillRect == null)
            return;

        Image fillImage = overheatSlider.fillRect.GetComponent<Image>();
        if (fillImage != null)
        {
            float value = overheatSlider.value;
            fillImage.color = value < 0.5f ?
                Color.Lerp(greenColor, yellowColor, value * 2f) :
                Color.Lerp(yellowColor, redColor, (value - 0.5f) * 2f);
        }
    }

    private void OnFireButtonDown(PointerEventData eventData)
    {
        if (!uiManager.IsAnyPanelOpen())
        {
            isFireButtonPressed = true;
            PlayGunSound();
        }
    }

    private void OnFireButtonUp(PointerEventData eventData)
    {
        isFireButtonPressed = false;
        StopGunSound();
    }
    #endregion

    #region Visual Effects
    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit, Vector3 hitPoint, Vector3 hitNormal, bool madeImpact)
    {
        DetectBullet enemy = hit.collider?.GetComponent<DetectBullet>();
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
        Destroy(trail.gameObject, trail.time);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, aimAssistRadius);

        DrawAimAssistCone(transform.position, transform.forward, horizontalAimAssistAngle, verticalAimAssistAngle, Color.magenta);

        Gizmos.color = Color.blue;
        Gizmos.DrawRay(bulletSpawnPoint.position, direction * 100f);

        if (mainCamera != null)
        {
            Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            Gizmos.color = Color.red;
            Gizmos.DrawRay(ray.origin, ray.direction * 100f);
        }
    }

    private void DrawAimAssistCone(Vector3 position, Vector3 forward, float horizontalAngle, float verticalAngle, Color color)
    {
        float horizontalRadius = Mathf.Tan(horizontalAngle * 0.5f * Mathf.Deg2Rad) * aimAssistRadius;
        float verticalRadius = Mathf.Tan(verticalAngle * 0.5f * Mathf.Deg2Rad) * aimAssistRadius;

        Vector3 topRight = position + (Quaternion.AngleAxis(horizontalAngle, Vector3.up) * Quaternion.AngleAxis(verticalAngle, Vector3.right) * forward * aimAssistRadius);
        Vector3 topLeft = position + (Quaternion.AngleAxis(-horizontalAngle, Vector3.up) * Quaternion.AngleAxis(verticalAngle, Vector3.right) * forward * aimAssistRadius);
        Vector3 bottomRight = position + (Quaternion.AngleAxis(horizontalAngle, Vector3.up) * Quaternion.AngleAxis(-verticalAngle, Vector3.right) * forward * aimAssistRadius);
        Vector3 bottomLeft = position + (Quaternion.AngleAxis(-horizontalAngle, Vector3.up) * Quaternion.AngleAxis(-verticalAngle, Vector3.right) * forward * aimAssistRadius);

        Gizmos.color = color;

        Gizmos.DrawLine(position, topRight);
        Gizmos.DrawLine(position, topLeft);
        Gizmos.DrawLine(position, bottomRight);
        Gizmos.DrawLine(position, bottomLeft);

        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }

    private void PlayImpactParticle(RaycastHit hit)
    {
        if (hit.collider == null) return;

        GameObject impactPrefab = null;

        if (hit.collider.CompareTag("Ground"))
        {
            impactPrefab = groundImpactPrefab;
        }
        else if (hit.collider.CompareTag("Building") || hit.collider.CompareTag("Target"))
        {
            impactPrefab = metalImpactPrefab;
        }

        if (impactPrefab != null)
        {
            GameObject impactInstance = Instantiate(impactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            ParticleSystem impactParticle = impactInstance.GetComponent<ParticleSystem>();
            if (impactParticle != null)
            {
                impactParticle.Play();
                Destroy(impactInstance, impactParticle.main.duration);
            }
            else
            {
                Destroy(impactInstance, 0.3f);
            }
        }
    }
    #endregion
}