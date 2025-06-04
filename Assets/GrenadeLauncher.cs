using UnityEngine;

public class GrenadeLauncher : MonoBehaviour
{
    public static GrenadeLauncher Instance;
    [SerializeField] private GameObject nadePrefab; // Prefab for the grenade
    [SerializeField] private Transform[] spawnPoints; // Array of spawn points
    [SerializeField] public int magazineSize = 12; // Number of grenades in magazine
    [SerializeField] private float launchForce = 30f; // Force applied to the grenade
    [SerializeField] private AudioClip launchSound; // Audio clip for launching
    [SerializeField] private AudioClip errorSound; // Audio clip for out of nades

    private int remainingGrenades; // Track remaining grenades
    private AudioSource audioSource; // Audio source component

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>(); // Reference to AudioSource
        ResetMagazine(); // Fill the magazine at the start
    }

    // Method to be called when the player presses the UI button
    public void LaunchGrenade()
    {
        if (remainingGrenades <= 0)
        {
            PlayErrorSound(); // Play error sound if no grenades are left
            return; // Exit the method if there are no grenades
        }

        // Select a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the grenade prefab at the spawn point
        GameObject nade = Instantiate(nadePrefab, spawnPoint.position, spawnPoint.rotation);

        // Apply force to the grenade
        Rigidbody rb = nade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * launchForce, ForceMode.Impulse);
        }

        // Play launch sound
        if (audioSource != null && launchSound != null)
        {
            audioSource.PlayOneShot(launchSound);
        }

        remainingGrenades--; // Decrease remaining grenades
    }

    // Method to play the error sound when out of grenades
    private void PlayErrorSound()
    {
        if (audioSource != null && errorSound != null)
        {
            audioSource.PlayOneShot(errorSound);
        }
    }

    // Method to refill the magazine (can be called from another script or event)
    public void RefillMagazine()
    {
        ResetMagazine(); // Reset the magazine count
    }

    // Helper method to reset the magazine
    private void ResetMagazine()
    {
        remainingGrenades = magazineSize;
    }
}
