using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class RainController : MonoBehaviour
{
    [SerializeField] private List<ParticleSystem> rainParticleSystems; // List of rain particle systems
    [SerializeField] private Light directionalLight; // Directional Light for lightning effect
    [SerializeField] private AudioSource lightningAudioSource; // AudioSource for lightning sound
    [SerializeField] private AudioClip lightningSound; // AudioClip for lightning sound
    [SerializeField] private Volume globalVolume; // Global volume for post-processing

    // Emission rates for each phase (customize these in Inspector)
    [System.Serializable]
    private struct EmissionRates
    {
        public float startPhaseRate;
        public float midPhaseRate;
        public float finalPhaseRate;
    }

    [SerializeField] private List<EmissionRates> particleEmissionRates; // Emission rates for each particle system

    private const float RAIN_CHANCE = 0.25f; // 25% chance for rain
    private const float TOTAL_RAIN_DURATION = 300f; // 5 minutes in seconds
    private const float START_PHASE_DURATION = 30f; // 30 seconds
    private const float MID_PHASE_DURATION = 210f; // From 30s to 4min (240s - 30s = 210s)
    private const float FINAL_PHASE_DURATION = 60f; // From 4min to 5min (300s - 240s = 60s)
    private const int MIN_LIGHTNING_STRIKES = 2;
    private const int MAX_LIGHTNING_STRIKES = 3;
    private const float FPS = 60f; // Assumed framerate for timing
    private const float AUDIO_DELAY = 1f; // Delay before playing lightning sound

    // Saturation keyframes (frame, saturation value)
    private readonly (int frame, float saturation)[] saturationKeyframes = new[]
    {
        (0, 0f),   // Frame 0: 0 saturation
        (3, -40f), // Frame 3: -40 saturation
        (6, -10f), // Frame 6: -10 saturation
        (9, -25f), // Frame 9: -25 saturation
        (12, -5f), // Frame 12: -5 saturation
        (15, -80f),// Frame 15: -80 saturation
        (21, -50f),// Frame 21: -50 saturation
        (30, 25f), // Frame 30: 25 saturation
        (42, -5f), // Frame 42: -5 saturation
        (50, 0f)   // Frame 50: 0 saturation
    };

    // Intensity keyframes (frame, intensity value)
    private readonly (int frame, float intensity)[] intensityKeyframes = new[]
    {
        (0, 0f),   // Frame 0: 0 (dark)
        (3, 2.4f), // Frame 3: 2.4 (first flick)
        (6, 0.6f), // Frame 6: 0.6 (quick drop)
        (9, 1.6f), // Frame 9: 1.6 (second flick)
        (12, 0.2f),// Frame 12: 0.2 (drop)
        (15, 5.0f),// Frame 15: 5.0 (brightest flash)
        (21, 3.0f),// Frame 21: 3.0 (begin decay)
        (30, 1.6f),// Frame 30: 1.6 (further drop)
        (42, 0.4f),// Frame 42: 0.4 (final flicker)
        (54, 0f)   // Frame 54: 0 (back to dark)
    };

    // Public method to start the rain, called from another script
    public void StartRain()
    {
        // Ensure particle systems are disabled initially
        SetParticleSystemsActive(false);

        // Roll the dice for rain chance
        if (Random.value > RAIN_CHANCE)
        {
            Debug.Log("No rain this time.");
            return; // 75% chance to skip rain
        }

        Debug.Log("Rain starting!");
        StartCoroutine(RainSequence());
    }

    private IEnumerator RainSequence()
    {
        // Enable and play particle systems
        SetParticleSystemsActive(true);
        foreach (var particleSystem in rainParticleSystems)
        {
            if (particleSystem != null)
            {
                particleSystem.Play();
            }
        }

        // Start lightning coroutine concurrently
        StartCoroutine(HandleLightning());

        // Start Phase (0s - 30s)
        SetEmissionRatesForPhase("start");
        yield return new WaitForSeconds(START_PHASE_DURATION);

        // Mid Phase (30s - 4min)
        SetEmissionRatesForPhase("mid");
        yield return new WaitForSeconds(MID_PHASE_DURATION);

        // Final Phase (4min - 5min)
        SetEmissionRatesForPhase("final");
        yield return new WaitForSeconds(FINAL_PHASE_DURATION);

        // Stop and disable particle systems after rain duration
        foreach (var particleSystem in rainParticleSystems)
        {
            if (particleSystem != null)
            {
                particleSystem.Stop();
            }
        }
        SetParticleSystemsActive(false);

        Debug.Log("Rain stopped.");
    }

    private void SetEmissionRatesForPhase(string phase)
    {
        for (int i = 0; i < rainParticleSystems.Count; i++)
        {
            if (i >= particleEmissionRates.Count || rainParticleSystems[i] == null)
                continue;

            var emission = rainParticleSystems[i].emission;
            float rate = 0f;

            switch (phase.ToLower())
            {
                case "start":
                    rate = particleEmissionRates[i].startPhaseRate;
                    break;
                case "mid":
                    rate = particleEmissionRates[i].midPhaseRate;
                    break;
                case "final":
                    rate = particleEmissionRates[i].finalPhaseRate;
                    break;
            }

            emission.rateOverTime = rate;
        }
    }

    private void SetParticleSystemsActive(bool active)
    {
        foreach (var particleSystem in rainParticleSystems)
        {
            if (particleSystem != null)
            {
                particleSystem.gameObject.SetActive(active);
            }
        }
    }

    private IEnumerator HandleLightning()
    {
        if (directionalLight == null)
        {
            Debug.LogWarning("Directional Light not set for lightning effect.");
            yield break;
        }

        if (lightningAudioSource == null || lightningSound == null)
        {
            Debug.LogWarning("Lightning AudioSource or AudioClip not set.");
        }

        // Random number of lightning strikes (2 or 3)
        int lightningCount = Random.Range(MIN_LIGHTNING_STRIKES, MAX_LIGHTNING_STRIKES + 1);

        // Generate random times for lightning strikes
        List<float> strikeTimes = new List<float>();
        for (int i = 0; i < lightningCount; i++)
        {
            strikeTimes.Add(Random.Range(0f, TOTAL_RAIN_DURATION));
        }
        strikeTimes.Sort(); // Sort to play in chronological order

        float elapsedTime = 0f;
        int currentStrikeIndex = 0;

        while (elapsedTime < TOTAL_RAIN_DURATION && currentStrikeIndex < strikeTimes.Count)
        {
            if (elapsedTime >= strikeTimes[currentStrikeIndex])
            {
                StartCoroutine(AdjustLightIntensity()); // Animate light intensity
                StartCoroutine(AdjustSaturation()); // Adjust saturation
                StartCoroutine(PlayLightningSound()); // Play sound after 1 second
                currentStrikeIndex++;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator AdjustLightIntensity()
    {
        if (directionalLight == null)
            yield break;

        float startTime = Time.time;
        int currentKeyframe = 0;

        while (currentKeyframe < intensityKeyframes.Length)
        {
            float targetTime = startTime + (intensityKeyframes[currentKeyframe].frame / FPS);
            if (Time.time >= targetTime)
            {
                directionalLight.intensity = intensityKeyframes[currentKeyframe].intensity;
                currentKeyframe++;
            }
            yield return null;
        }

        // Ensure final intensity is reset to 0
        directionalLight.intensity = 0f;
    }

    private IEnumerator AdjustSaturation()
    {
        if (globalVolume == null || !globalVolume.profile.TryGet<ColorAdjustments>(out var colorAdjustments))
        {
            Debug.LogWarning("Global Volume or ColorAdjustments not found.");
            yield break;
        }

        float startTime = Time.time;
        int currentKeyframe = 0;

        while (currentKeyframe < saturationKeyframes.Length)
        {
            float targetTime = startTime + (saturationKeyframes[currentKeyframe].frame / FPS);
            if (Time.time >= targetTime)
            {
                colorAdjustments.saturation.value = saturationKeyframes[currentKeyframe].saturation;
                currentKeyframe++;
            }
            yield return null;
        }

        // Ensure final saturation is reset to 0
        colorAdjustments.saturation.value = 0f;
    }

    private IEnumerator PlayLightningSound()
    {
        if (lightningAudioSource == null || lightningSound == null)
            yield break;

        yield return new WaitForSeconds(AUDIO_DELAY);
        lightningAudioSource.PlayOneShot(lightningSound);
    }

    // Optional: Validate setup in Inspector
    private void OnValidate()
    {
        if (rainParticleSystems.Count > particleEmissionRates.Count)
        {
            Debug.LogWarning("Not enough emission rates defined for all particle systems.");
        }
        if (globalVolume != null && !globalVolume.profile.Has<ColorAdjustments>())
        {
            Debug.LogWarning("Global Volume profile does not contain ColorAdjustments override.");
        }
    }
}