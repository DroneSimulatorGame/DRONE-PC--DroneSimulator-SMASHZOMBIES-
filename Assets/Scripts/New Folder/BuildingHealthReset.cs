
using UnityEngine;

public class BuildingHealthManager : MonoBehaviour
{
    // Method to reset health for all buildings on the "Building" layer
    public void ResetAllBuildingHealth()
    {
        // Find all GameObjects in the scene
        GameObject[] allObjects = FindObjectsOfType<GameObject>();

        // Loop through all the objects and check if they are in the "Building" layer
        foreach (var obj in allObjects)
        {
            if (obj.layer == LayerMask.NameToLayer("Building"))
            {
                // Try to get the Building component (assuming all objects have the "Building" script)
                Building1 buildingComponent = obj.GetComponent<Building1>();

                // If the object has the Building component, call its ResetHealth method
                if (buildingComponent != null)
                {
                    buildingComponent.ResetHealth();
                }
            }
        }
    }
}
