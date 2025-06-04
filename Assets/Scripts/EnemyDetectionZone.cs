using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EnemyDetectionZone : MonoBehaviour
{
    public AudioSource alarmSound; // Reference to the AudioSource component
    public LayerMask enemyLayer; // LayerMask to specify the Enemy layer
    public Vector3 zoneSize = new Vector3(5f, 5f, 5f); // Size of the detection zone

    public Volume globalVolume; // Reference to the Global Volume
    private ColorAdjustments colorAdjustments; // To access the Color Adjustments in Volume

    public float colorChangeSpeed = 1f; // Speed of the color transition
    public float colorHoldDuration = 1f; // How long the color stays in each state (normal and warning)

    // Expose the normal and warning colors to the inspector
    public Color normalColor = new Color(1f, 1f, 1f); // Normal color (default white)
    public Color warningColor = new Color(1f, 0.8f, 0.8f); // Warning color (default slightly red)

    private bool isEnemyDetected = false;
    private bool isColorIncreasing = true;

    private float lerpValue = 0f;
    private float colorHoldTimer = 0f; // Timer to hold the color after fully transitioned

    private void Start()
    {
        // Get the ColorAdjustments from the Global Volume
        if (globalVolume != null && globalVolume.profile.TryGet(out colorAdjustments))
        {
            // Set the initial color filter to normal
            colorAdjustments.colorFilter.value = normalColor;
        }
        else
        {
            //Debug.LogWarning("Color Adjustments not found in Global Volume.");
        }
    }

    private void Update()
    {
        // Check if any enemy objects are inside the detection box
        Collider[] hitColliders = Physics.OverlapBox(transform.position, zoneSize / 2, Quaternion.identity, enemyLayer);

        if (hitColliders.Length > 0)
        {
            if (!isEnemyDetected)
            {
                // Play sound if enemies are detected
                alarmSound.Play();
                isEnemyDetected = true;
            }
            UpdateColorFilter();
        }
        else
        {
            if (isEnemyDetected)
            {
                // Stop sound if no enemies are detected
                alarmSound.Stop();
                isEnemyDetected = false;

                // Reset color to normal when no enemies are present
                colorAdjustments.colorFilter.value = normalColor;
            }
        }
    }

    private void UpdateColorFilter()
    {
        if (colorAdjustments != null && isEnemyDetected)
        {
            // Control the timer to hold the color after it reaches fully normal or warning
            if (colorHoldTimer > 0f)
            {
                colorHoldTimer -= Time.deltaTime;
                return;
            }

            // Gradually transition between normal and warning colors
            if (isColorIncreasing)
            {
                lerpValue += Time.deltaTime * colorChangeSpeed;
                if (lerpValue >= 1f)
                {
                    lerpValue = 1f;
                    isColorIncreasing = false; // Reverse the direction
                    colorHoldTimer = colorHoldDuration; // Start holding the color
                }
            }
            else
            {
                lerpValue -= Time.deltaTime * colorChangeSpeed;
                if (lerpValue <= 0f)
                {
                    lerpValue = 0f;
                    isColorIncreasing = true; // Reverse the direction
                    colorHoldTimer = colorHoldDuration; // Start holding the color
                }
            }

            // Lerp between normal and warning colors based on the lerp value
            colorAdjustments.colorFilter.value = Color.Lerp(normalColor, warningColor, lerpValue);
        }
    }


    


    private void OnDrawGizmos()
    {
        // Draw a box in the scene view to visualize the detection zone
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, zoneSize);
    }
}
