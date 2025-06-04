using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isPaused = false;

    // Method to pause the game and mute all sounds
    public void PauseGame()
    {
        // Stop time to pause the game
        Time.timeScale = 0;

        // Mute all sounds
        AudioListener.pause = true;

        // Optionally, you can set a UI element active to indicate the game is paused
        Debug.Log("Game Paused");
    }

    // Method to resume the game and unmute all sounds
    public void PlayGame()
    {
        // Resume time to continue the game
        Time.timeScale = 1;

        // Unmute all sounds
        AudioListener.pause = false;

        // Optionally, you can set the paused UI element inactive
        Debug.Log("Game Resumed");
    }

    // Method to toggle pause and sound
    public void TogglePause()
    {
        if (isPaused)
        {
            PlayGame();
        }
        else
        {
            PauseGame();
        }

        isPaused = !isPaused;
    }
}
