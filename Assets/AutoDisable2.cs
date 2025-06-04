using System.Collections;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    [Tooltip("Time in seconds before the object is disabled")]
    [SerializeField]
    private float disableDelay = 1f;

    private void OnEnable()
    {
        // Start a coroutine to wait and disable the object
        StartCoroutine(WaitAndDisable());
    }

    private IEnumerator WaitAndDisable()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(disableDelay);

        // Disable the object
        gameObject.SetActive(false);
    }
}
