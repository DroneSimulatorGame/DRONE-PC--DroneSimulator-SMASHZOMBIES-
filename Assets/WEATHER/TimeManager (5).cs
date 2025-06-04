using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    public float realTimeInMinutes = 60f; // 1 real hour = 24 in-game hours
    [Range(0, 24)] public float sunriseStart = 5.5f;
    [Range(0, 24)] public float sunriseEnd = 7f;
    [Range(0, 24)] public float sunsetStart = 18f;
    [Range(0, 24)] public float sunsetEnd = 19.5f;

    public float timeOfDay; // 0 to 24
    public string currentTime; // Shunchaki Inspector’da ko‘rinadigan format

    [Header("Light Settings")]
    public Light directionalLight;
    public float lightIntensityNight = 0f;
    public float lightIntensityDay = 1f;
    public Gradient lightColorGradient;

    [Header("Light Rotation Settings")]
    [Tooltip("Quyosh Y o‘qi bo‘yicha boshlang‘ich rotatsiyasi")]
    public float startYRotation = 90f; // masalan: quyosh sharqdan chiqadi

    [Tooltip("Quyosh aylanish oraliği (0-360), masalan 180° bo‘lsa, yarim osmon")]
    public float rotationRange = 180f; // odatda 180°



    [Header("Skybox Settings")]
    public Material skyboxMaterial;
    public float skyboxExposureNight = 0.5f;
    public float skyboxExposureDay = 1.3f;

    [Header("Fog Settings")]
    public float fogEndDistanceNight = 30f;
    public float fogEndDistanceDay = 300f;
    public Color fogColorDay = new Color(0.75f, 0.85f, 1f);     // och moviy, kunduzgi
    public Color fogColorNight = new Color(0.05f, 0.05f, 0.1f); // qora-siyohrang, kechasi

    [Header("Environment Settings")]
    public Color dayAmbientColor = new Color(0.419f, 0.419f, 0.419f);
    public Color nightAmbientColor = Color.black;

    private float timeSpeed;

    [Header("Event Triggers")]
    public UnityEvent OnNightStarted;
    public UnityEvent OnNightEnded;
    public UnityEvent OnDayStarted;
    public UnityEvent OnDayEnded;


    [Header("Custom Events")]
    public List<CustomTimeEvent> customTimeEvents = new List<CustomTimeEvent>();

    private bool hasTriggeredNightStart = false;
    private bool hasTriggeredNightEnd = false;
    private bool hasTriggeredDayStart = false;
    private bool hasTriggeredDayEnd = false;
    private bool hasTriggeredCustom = false;

    private void Start()
    {
        timeSpeed = 24f / (realTimeInMinutes * 60f);
        if (skyboxMaterial != null)
            RenderSettings.skybox = skyboxMaterial;
    }

    private void Update()
    {
        // Time progression
        timeOfDay += timeSpeed * Time.deltaTime;
        if (timeOfDay > 24f) timeOfDay -= 24f;

        float normalizedTime = timeOfDay / 24f;

        float transitionFactor = GetTransitionFactor(timeOfDay);

        UpdateLighting(normalizedTime, transitionFactor);
        UpdateSkybox(transitionFactor);
        UpdateFog(transitionFactor);
        UpdateAmbientColor(transitionFactor);

        UpdateCurrentTimeString();
        CheckTimeEvents();
    }

    float GetTransitionFactor(float hour)
    {
        // Sunrise
        if (hour >= sunriseStart && hour <= sunriseEnd)
        {
            return Mathf.InverseLerp(sunriseStart, sunriseEnd, hour);
        }
        // Day
        else if (hour > sunriseEnd && hour < sunsetStart)
        {
            return 1f;
        }
        // Sunset
        else if (hour >= sunsetStart && hour <= sunsetEnd)
        {
            return Mathf.InverseLerp(sunsetEnd, sunsetStart, hour); // Reverse lerp
        }
        // Night
        return 0f;
    }

    void UpdateLighting(float normalizedTime, float t)
    {
        if (directionalLight != null)
        {
            // Y faqatgina vaqtga qarab o‘zgaradi
            float currentY = startYRotation + (normalizedTime * rotationRange);

            // X va Z rotation o‘zgarmasin
            Vector3 fixedEuler = directionalLight.transform.eulerAngles;
            directionalLight.transform.rotation = Quaternion.Euler(fixedEuler.x, currentY, fixedEuler.z);

            directionalLight.intensity = Mathf.Lerp(lightIntensityNight, lightIntensityDay, t);
            directionalLight.color = lightColorGradient.Evaluate(normalizedTime);
        }
    }


    void UpdateSkybox(float t)
    {
        if (skyboxMaterial != null)
        {
            float exposure = Mathf.Lerp(skyboxExposureNight, skyboxExposureDay, t);
            skyboxMaterial.SetFloat("_Exposure", exposure);
        }
    }

    void UpdateFog(float t)
    {
        RenderSettings.fogEndDistance = Mathf.Lerp(fogEndDistanceNight, fogEndDistanceDay, t);
        RenderSettings.fogColor = Color.Lerp(fogColorNight, fogColorDay, t);
    }


    void UpdateAmbientColor(float t)
    {
        RenderSettings.ambientLight = Color.Lerp(nightAmbientColor, dayAmbientColor, t);
    }

    void UpdateCurrentTimeString()
    {
        int hours = Mathf.FloorToInt(timeOfDay);
        int minutes = Mathf.FloorToInt((timeOfDay - hours) * 60);
        currentTime = $"{hours:D2}:{minutes:D2}";
    }

    void CheckTimeEvents()
    {
        // Night Start (after sunsetEnd)
        if (!hasTriggeredNightStart && timeOfDay >= sunsetEnd)
        {
            OnNightStarted?.Invoke();
            hasTriggeredNightStart = true;
            hasTriggeredNightEnd = false; // prepare for next cycle

            Debug.Log(" Nigt start  <<<<<<<<<>>>>>>>>>  sunset end ");
        }

        // Night End (sunriseStart)
        if (!hasTriggeredNightEnd && timeOfDay >= sunriseStart && timeOfDay < sunriseEnd)
        {
            OnNightEnded?.Invoke();
            hasTriggeredNightEnd = true;
            hasTriggeredDayStart = false; // prepare for next cycle
            Debug.Log(" Sunrise start  <<<<<<<<<>>>>>>>>>  Night end ");
        }

        // Day Start (sunriseEnd)
        if (!hasTriggeredDayStart && timeOfDay >= sunriseEnd && timeOfDay < sunsetStart)
        {
            OnDayStarted?.Invoke();
            hasTriggeredDayStart = true;
            hasTriggeredDayEnd = false;
            Debug.Log(" Day start  <<<<<<<<<>>>>>>>>>  Sunrise end ");
        }

        // Day End (sunsetStart)
        if (!hasTriggeredDayEnd && timeOfDay >= sunsetStart && timeOfDay < sunsetEnd)
        {
            OnDayEnded?.Invoke();
            hasTriggeredDayEnd = true;
            hasTriggeredNightStart = false;
            Debug.Log(" Sunrise start  <<<<<<<<<>>>>>>>>>  Day end ");
        }

        foreach (var customEvent in customTimeEvents)
        {
            float target = customEvent.triggerTime;
            float buffer = timeSpeed * Time.deltaTime * 2f;

            if (!customEvent.hasTriggered && timeOfDay >= target && timeOfDay < target + buffer)
            {
                customEvent.onTimeReached?.Invoke();
                customEvent.hasTriggered = true;
            }

            // Reset qilish uchun (time loop qilganda)
            if (timeOfDay < target)
            {
                customEvent.hasTriggered = false;
            }
        }

    }

}

[System.Serializable]
public class CustomTimeEvent
{
    [Range(0f, 24f)]
    public float triggerTime; // Masalan: 12.5 = 12:30
    public UnityEvent onTimeReached;

    [HideInInspector] public bool hasTriggered = false;
}

