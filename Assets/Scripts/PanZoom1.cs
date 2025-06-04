using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanZoom : MonoBehaviour
{
    private Vector3 touchStart;
    public float zoomOutMin = 20f;
    public float zoomOutMax = 60f;
    public float zoomSpeed = 10f;

    // Area boundaries relative to the camera's initial position
    public float areaWidth = 20f;
    public float areaHeight = 10f;
    public float areaDepth = 20f;

    private Vector3 initialPosition;
    private float boundaryLeft;
    private float boundaryRight;
    private float boundaryTop;
    private float boundaryBottom;
    private float boundaryForward;
    private float boundaryBackward;

    public GameObject StartPosition;

    // Damping parameter for smooth panning
    public float panDamping = 0.1f;
    private Vector3 targetPosition;

    // Rotation parameters
    public float rotationSpeed = 0.2f;
    private float rotationX;
    private float rotationY;

    void Start()
    {
        initialPosition = StartPosition.transform.position;
        targetPosition = initialPosition;
        CalculateCameraBoundaries();
    }

    void Update()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                touchStart = GetWorldPoint(touch.position);
            }
            else if (touch.phase == TouchPhase.Moved)
            {
                Vector3 direction = touchStart - GetWorldPoint(touch.position);
                targetPosition += direction;
                ClampTargetPosition();
            }
        }
        else if (Input.touchCount == 2)
        {
            Touch touch0 = Input.GetTouch(0);
            Touch touch1 = Input.GetTouch(1);

            Vector2 touch0PrevPos = touch0.position - touch0.deltaPosition;
            Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;

            float prevMagnitude = (touch0PrevPos - touch1PrevPos).magnitude;
            float currentMagnitude = (touch0.position - touch1.position).magnitude;
            float difference = currentMagnitude - prevMagnitude;

            if (Mathf.Abs(difference) > 0.01f) // Pinching to zoom
            {
                Zoom(difference * 0.01f);
            }
            else // Rotating the camera
            {
                Vector2 touchDelta = touch0.deltaPosition - touch1.deltaPosition;
                rotationX += touchDelta.x * rotationSpeed;
                rotationY -= touchDelta.y * rotationSpeed;

                Camera.main.transform.localRotation = Quaternion.Euler(rotationY, rotationX, 0f);
            }
        }
        Zoom(Input.GetAxis("Mouse ScrollWheel")); // For mouse zooming

        // Smoothly move the camera towards the target position
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, panDamping);
    }

    void Zoom(float increment)
    {
        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView - increment * zoomSpeed, zoomOutMin, zoomOutMax);
        ClampTargetPosition();
    }

    void CalculateCameraBoundaries()
    {
        float halfWidth = areaWidth / 2f;
        float halfHeight = areaHeight / 2f;
        float halfDepth = areaDepth / 2f;

        boundaryLeft = initialPosition.x - halfWidth;
        boundaryRight = initialPosition.x + halfWidth;
        boundaryTop = initialPosition.y + halfHeight;
        boundaryBottom = initialPosition.y - halfHeight;
        boundaryForward = initialPosition.z + halfDepth;
        boundaryBackward = initialPosition.z - halfDepth;
    }

    void ClampTargetPosition()
    {
        targetPosition.x = Mathf.Clamp(targetPosition.x, boundaryLeft, boundaryRight);
        targetPosition.y = Mathf.Clamp(targetPosition.y, boundaryBottom, boundaryTop);
        targetPosition.z = Mathf.Clamp(targetPosition.z, boundaryBackward, boundaryForward);
    }

    Vector3 GetWorldPoint(Vector3 screenPoint)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPoint);
        Plane plane = new Plane(Vector3.up, Vector3.zero); // Plane with normal facing up at origin
        float distance;
        if (plane.Raycast(ray, out distance))
        {
            return ray.GetPoint(distance);
        }
        return Vector3.zero;
    }

    void OnDrawGizmos()
    {
        if (Camera.main == null)
            return;

        Gizmos.color = Color.red;

        // Draw front rectangle
        Gizmos.DrawLine(new Vector3(boundaryLeft, boundaryTop, boundaryForward), new Vector3(boundaryRight, boundaryTop, boundaryForward));
        Gizmos.DrawLine(new Vector3(boundaryRight, boundaryTop, boundaryForward), new Vector3(boundaryRight, boundaryBottom, boundaryForward));
        Gizmos.DrawLine(new Vector3(boundaryRight, boundaryBottom, boundaryForward), new Vector3(boundaryLeft, boundaryBottom, boundaryForward));
        Gizmos.DrawLine(new Vector3(boundaryLeft, boundaryBottom, boundaryForward), new Vector3(boundaryLeft, boundaryTop, boundaryForward));

        // Draw back rectangle
        Gizmos.DrawLine(new Vector3(boundaryLeft, boundaryTop, boundaryBackward), new Vector3(boundaryRight, boundaryTop, boundaryBackward));
        Gizmos.DrawLine(new Vector3(boundaryRight, boundaryTop, boundaryBackward), new Vector3(boundaryRight, boundaryBottom, boundaryBackward));
        Gizmos.DrawLine(new Vector3(boundaryRight, boundaryBottom, boundaryBackward), new Vector3(boundaryLeft, boundaryBottom, boundaryBackward));
        Gizmos.DrawLine(new Vector3(boundaryLeft, boundaryBottom, boundaryBackward), new Vector3(boundaryLeft, boundaryTop, boundaryBackward));

        // Draw lines connecting front and back rectangles
        Gizmos.DrawLine(new Vector3(boundaryLeft, boundaryTop, boundaryForward), new Vector3(boundaryLeft, boundaryTop, boundaryBackward));
        Gizmos.DrawLine(new Vector3(boundaryRight, boundaryTop, boundaryForward), new Vector3(boundaryRight, boundaryTop, boundaryBackward));
        Gizmos.DrawLine(new Vector3(boundaryRight, boundaryBottom, boundaryForward), new Vector3(boundaryRight, boundaryBottom, boundaryBackward));
        Gizmos.DrawLine(new Vector3(boundaryLeft, boundaryBottom, boundaryForward), new Vector3(boundaryLeft, boundaryBottom, boundaryBackward));
    }
}