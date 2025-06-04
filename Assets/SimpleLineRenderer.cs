using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class SimpleLineRenderer : MonoBehaviour
{
    public Transform startPoint; // Assign the first point in the Inspector
    public Transform endPoint;   // Assign the second point in the Inspector

    private LineRenderer lineRenderer;

    void Start()
    {
        // Get the LineRenderer component
        lineRenderer = GetComponent<LineRenderer>();

        // Set the number of positions for the LineRenderer
        lineRenderer.positionCount = 2;
    }

    void Update()
    {
        // Update the positions of the LineRenderer
        if (startPoint != null && endPoint != null)
        {
            lineRenderer.SetPosition(0, startPoint.position); // Start point
            lineRenderer.SetPosition(1, endPoint.position);   // End point
        }
    }
}
