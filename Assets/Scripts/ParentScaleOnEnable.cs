using UnityEngine;

public class ParentScaleOnEnable : MonoBehaviour
{
    [Header("Scale Values")]
    public Vector3 startScale = Vector3.one;
    public Vector3 midScale = Vector3.one * 1.5f;
    public Vector3 endScale = Vector3.one * 2f;

    [Header("Time Settings")]
    public float timeToMid = 1f;
    public float timeToEnd = 1f;

    private float timer = 0f;
    private bool isScaling = false;
    private Transform parentTransform;

    private void OnEnable()
    {
        if (transform.parent != null)
        {
            parentTransform = transform.parent;
            parentTransform.localScale = startScale;
            timer = 0f;
            isScaling = true;
        }
        else
        {
            Debug.LogWarning("ParentScaleOnEnable: This object has no parent. Scaling cannot be applied.");
        }
    }

    private void Update()
    {
        if (!isScaling || parentTransform == null) return;

        // Scale to the mid value over the first interval
        if (timer < timeToMid)
        {
            parentTransform.localScale = Vector3.Lerp(startScale, midScale, timer / timeToMid);
        }
        // Scale to the end value over the second interval
        else if (timer < timeToMid + timeToEnd)
        {
            float progress = (timer - timeToMid) / timeToEnd;
            parentTransform.localScale = Vector3.Lerp(midScale, endScale, progress);
        }
        else
        {
            // Scaling is complete
            parentTransform.localScale = endScale;
            isScaling = false;
        }

        // Increase the timer by the delta time for smooth scaling
        timer += Time.deltaTime;
    }
}
