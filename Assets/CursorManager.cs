using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    // Public fields to assign cursor textures in the Inspector
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D interactableCursor;

    // Hotspot for the cursor (default is top-left corner; adjust if needed)
    private Vector2 cursorHotspot = Vector2.zero;

    // Track if the cursor is over an interactable
    private bool isOverInteractable = false;

    private void Start()
    {
        // Set the default cursor when the game starts
        SetDefaultCursor();
    }

    private void Update()
    {
        // Check for pointer over UI elements
        UpdateCursorState();
    }

    private void UpdateCursorState()
    {
        // Check if the pointer is over a UI element
        if (IsPointerOverInteractable())
        {
            if (!isOverInteractable)
            {
                // Switch to interactable cursor
                SetInteractableCursor();
                isOverInteractable = true;
            }
        }
        else
        {
            if (isOverInteractable)
            {
                // Revert to default cursor
                SetDefaultCursor();
                isOverInteractable = false;
            }
        }
    }

    private bool IsPointerOverInteractable()
    {
        // Check if the EventSystem exists and the pointer is over a UI element
        if (EventSystem.current != null)
        {
            // Create a pointer event data
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            // Raycast to detect UI elements under the pointer
            var raycastResults = new System.Collections.Generic.List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, raycastResults);

            foreach (var result in raycastResults)
            {
                // Check if the hit object has a Button component or is marked as interactable
                if (result.gameObject.GetComponent<Button>() != null)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void SetDefaultCursor()
    {
        if (defaultCursor != null)
        {
            Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogWarning("Default cursor texture is not assigned!");
        }
    }

    private void SetInteractableCursor()
    {
        if (interactableCursor != null)
        {
            Cursor.SetCursor(interactableCursor, cursorHotspot, CursorMode.Auto);
        }
        else
        {
            Debug.LogWarning("Interactable cursor texture is not assigned!");
        }
    }
}