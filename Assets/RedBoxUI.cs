using UnityEngine;

public class RedBoxUI : MonoBehaviour
{
    // Reference to the GameObject you want to control
    public GameObject targetObject;

    // Public bool to control whether the GameObject is active or inactive
    public bool isActive;

    // Update is called once per frame
    void Update()
    {
        // Set the target GameObject active/inactive based on the isActive bool
        if (targetObject != null)
        {
            targetObject.SetActive(isActive);
        }
    }
}
