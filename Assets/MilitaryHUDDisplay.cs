using UnityEngine;
using UnityEngine.UI;

public class MilitaryHUDDisplay : MonoBehaviour
{
    // Reference to the target GameObject for tracking position
    public Transform targetObject;

    // References to the standard Text UI elements
    public Text northCoordinatesText;
    public Text southCoordinatesText;
    public Text mgsText;
    public Text twsText;
    public Text ltdText;

    private Vector3 lastPosition;

    private void Start()
    {
        // Initialize text values
        UpdateDisplay();
        lastPosition = targetObject.position;
    }

    private void Update()
    {
        // Update display only if the target object's position changes
        if (targetObject.position != lastPosition)
        {
            UpdateDisplay();
            lastPosition = targetObject.position;
        }
    }

    private void UpdateDisplay()
    {
        // Generate random coordinate values
        northCoordinatesText.text = $"N {Random.Range(0, 90)}° {Random.Range(0, 60)}' {Random.Range(0, 60)}\"";
        southCoordinatesText.text = $"S {Random.Range(0, 90)}° {Random.Range(0, 60)}' {Random.Range(0, 60)}\"";

        // Generate random values for MGS, TWS, LTD
        mgsText.text = $"MGS {Random.Range(0, 999)}";
        twsText.text = $"TWS {Random.Range(0, 999)}";
        ltdText.text = $"LTD {Random.Range(0, 999)}";
    }
}
