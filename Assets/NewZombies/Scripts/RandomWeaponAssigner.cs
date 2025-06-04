using UnityEngine;

public class RandomWeaponActivator : MonoBehaviour
{
    // Array to hold references to the weapon models attached to the player, set inactive by default
    public GameObject[] weaponModels;

    void Start()
    {
        // Ensure there are weapon models to choose from
        if (weaponModels.Length == 0)
        {
            Debug.LogWarning("No weapon models assigned to the RandomWeaponActivator.");
            return;
        }

        // Select a random weapon model from the array
        int randomIndex = Random.Range(0, weaponModels.Length);

        // Activate the randomly selected weapon and keep others inactive
        for (int i = 0; i < weaponModels.Length; i++)
        {
            weaponModels[i].SetActive(i == randomIndex);
        }
    }
}
