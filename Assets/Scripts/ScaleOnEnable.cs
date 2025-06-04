using UnityEngine;

public class ScaleOnEnable : MonoBehaviour
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

    private void OnEnable()
    {
        transform.localScale = startScale;
        timer = 0f;
        isScaling = true;
    }

    private void Update()
    {
        if (!isScaling) return;

        // Scale to the mid value over the first interval
        if (timer < timeToMid)
        {
            transform.localScale = Vector3.Lerp(startScale, midScale, timer / timeToMid);
        }
        // Scale to the end value over the second interval
        else if (timer < timeToMid + timeToEnd)
        {
            float progress = (timer - timeToMid) / timeToEnd;
            transform.localScale = Vector3.Lerp(midScale, endScale, progress);
        }
        else
        {
            // Scaling is done
            transform.localScale = endScale;
            isScaling = false;
        }

        // Increase the timer by the delta time for smooth scaling
        timer += Time.deltaTime;
    }
}
