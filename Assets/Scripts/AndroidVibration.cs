using UnityEngine;
using UnityEngine.UI;

public class AndroidVibration : MonoBehaviour
{
    private AndroidJavaObject vibrator;
    public bool isPressed = false;

    void Start()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            try
            {
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
                vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            }
            catch (System.Exception e)
            {
                Debug.LogError("Error getting Vibrator service: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("Vibration only works on Android.");
        }
    }

    public void IsPressed()
    {
        if (!isPressed)
        {
            isPressed = true;
        }
        else
        {
            isPressed = !isPressed;

        }
    }
    public void StartVibration()
    {
        if (isPressed)
        {
            if (vibrator != null)
            {
                long[] vibrationPattern = { 50, 120 };
                int repeat = -1;
                vibrator.Call("vibrate", vibrationPattern, repeat);
                Debug.Log("Vibration started");
            }
            else
            {
                Debug.LogError("Failed to connect to Vibrator service.");
            }
        }
    }

    private void StopVibration()
    {
        if (vibrator != null)
        {
            vibrator.Call("cancel");
            Debug.Log("Vibration stopped");
        }
    }

    void OnDestroy()
    {
        StopVibration();
    }
}