using UnityEngine;
using System.Collections;

public class InternetConnectionChecker : MonoBehaviour
{
    [SerializeField] private GameObject internetErrorUI; // The UI to show the error message
    [SerializeField] private Camera targetCamera; // The specific Camera component to monitor
    [SerializeField] private float checkInterval = 60f; // Interval to check the connection (in seconds)

    private void Start()
    {
        StartCoroutine(CheckInternetConnectionRoutine());
    }

    private IEnumerator CheckInternetConnectionRoutine()
    {
        while (true)
        {
            if (targetCamera != null && targetCamera.enabled)
            {
                // Check internet connection status
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    ShowInternetError();
                }
                else
                {
                    HideInternetError();
                }
            }
            else
            {
                HideInternetError(); // Hide if the target Camera component is disabled
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private void ShowInternetError()
    {
        if (!internetErrorUI.activeSelf)
        {
            internetErrorUI.SetActive(true);
        }
    }

    private void HideInternetError()
    {
        if (internetErrorUI.activeSelf)
        {
            internetErrorUI.SetActive(false);
        }
    }

    public void DismissErrorUI()
    {
        HideInternetError();
    }
}
