using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FreeLookCameraController : MonoBehaviour, IDragHandler
{
    public float rotationSpeed = 0.2f;  // Speed of camera rotation
    public Transform target;  // Target to rotate around (e.g., the drone)

    private Vector2 lastTouchPosition;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Target is not assigned in FreeLookCameraController.");
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        lastTouchPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (target == null)
            return;

        Vector2 delta = eventData.position - lastTouchPosition;

        // Rotate around the target
        transform.RotateAround(target.position, Vector3.up, -delta.x * rotationSpeed);
        Vector3 right = Vector3.Cross(Vector3.up, transform.forward);
        transform.RotateAround(target.position, right, delta.y * rotationSpeed);

        lastTouchPosition = eventData.position;
    }
}