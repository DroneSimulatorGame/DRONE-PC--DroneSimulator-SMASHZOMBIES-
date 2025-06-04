using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainTamom : MonoBehaviour
{
    public void ExitTraining()
    {
        PlayerPrefs.SetInt("training3", 1);
        PlayerPrefs.Save();


        AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");
        operation.allowSceneActivation = true;  
    }
}
