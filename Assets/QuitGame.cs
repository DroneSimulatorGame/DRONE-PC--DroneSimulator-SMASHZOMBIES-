using UnityEngine;

public class QuitGame : MonoBehaviour
{
    // This method can be hooked up to a UI Button OnClick event
    public void Quit()
    {
        Debug.Log("Quit button pressed.");

#if UNITY_EDITOR
        // Stop play mode if running in the editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // Quit the application if running in a build
        Application.Quit();
#endif
    }
}
