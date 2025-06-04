using System.Collections.Generic;
using UnityEngine;

public class FrustumCulling : MonoBehaviour
{
    private Camera cam;
    private Plane[] frustumPlanes;
    private List<GameObject> enemies = new List<GameObject>();

    void Start()
    {
        // Get the camera component attached to this object
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        // Refresh the list of enemies each frame to include any new instances
        enemies.Clear();
        enemies.AddRange(GameObject.FindGameObjectsWithTag("Enemy"));

        // Get the frustum planes from this camera's perspective
        frustumPlanes = GeometryUtility.CalculateFrustumPlanes(cam);

        // Loop through each enemy and enable/disable renderer based on frustum check
        foreach (GameObject enemy in enemies)
        {
            Collider collider = enemy.GetComponent<Collider>();
            if (collider != null)
            {
                SkinnedMeshRenderer skinnedRenderer = enemy.GetComponentInChildren<SkinnedMeshRenderer>();
                if (skinnedRenderer != null)
                {
                    // Check if the enemy's bounds are within the frustum
                    bool isVisible = GeometryUtility.TestPlanesAABB(frustumPlanes, collider.bounds);

                    // Enable or disable the SkinnedMeshRenderer based on visibility
                    skinnedRenderer.enabled = isVisible;

                    // Log the visibility status and bounds
                    Debug.Log($"Enemy {enemy.name} visibility: {(isVisible ? "Visible" : "Culled")}, Bounds: {collider.bounds}");
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        if (cam != null)
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = cam.transform.localToWorldMatrix;
            Gizmos.DrawFrustum(Vector3.zero, cam.fieldOfView, cam.farClipPlane, cam.nearClipPlane, cam.aspect);
        }
    }
}
