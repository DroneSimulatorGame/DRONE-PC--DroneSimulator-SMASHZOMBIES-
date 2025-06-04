using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform drone;
    public Vector3 offset = new Vector3(0, 5, -10);

    public float followSpeed = 5f;
    public float rotateSpeed = 5f;
    public float heightAboveDrone = 2f;

    public RectTransform joystickRect;

    private Vector3 currentVelocity;
    private Quaternion defaultRotation;
    private Rect joystickArea;
    private bool isJoystickActive = false;
    private int joystickTouchId = -1;

    void Start()
    {
        defaultRotation = transform.rotation;
        UpdateJoystickArea();
    }

    void Update()
    {
        // No manual rotation handling
    }

    void FixedUpdate()
    {
        if (drone == null)
            return;

        // Smoothly update the camera's position
        Vector3 desiredPosition = drone.position + drone.rotation * offset;
        desiredPosition.y += heightAboveDrone;
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref currentVelocity, followSpeed * Time.deltaTime);

        // Automatically rotate the camera to look at the drone
        Vector3 lookAtPosition = drone.position;
        lookAtPosition.y += 1f;
        Quaternion desiredRotation = Quaternion.LookRotation(lookAtPosition - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, rotateSpeed * Time.deltaTime);
    }

    // Joystick area handling remains unchanged
    bool IsTouchInJoystickArea(Vector2 touchPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(joystickRect.parent as RectTransform, touchPosition, null, out Vector2 localPoint);
        return joystickArea.Contains(localPoint);
    }

    void UpdateJoystickArea()
    {
        if (joystickRect != null)
        {
            Vector3[] corners = new Vector3[4];
            joystickRect.GetWorldCorners(corners);

            for (int i = 0; i < 4; i++)
            {
                corners[i] = joystickRect.parent.InverseTransformPoint(corners[i]);
            }

            joystickArea = new Rect(corners[0], corners[2] - corners[0]);
        }
    }

    public void OnJoystickChanged()
    {
        UpdateJoystickArea();
    }
}
