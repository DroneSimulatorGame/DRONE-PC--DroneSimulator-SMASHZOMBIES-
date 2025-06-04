using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewCameraSwitch : MonoBehaviour
{
    public GameObject droneCam;
    public GameObject mainCam;
    public GameObject Drone;
    //public AudioSource mainCamAudioSource; // Reference to the AudioSource for the main camera

    public int Manager;

    public void Start()
    {
        // Ensure only one camera is active at the start
        Cam_1(); // Assuming you start with the main camera active
    }

    public void ChangeCamera()
    {
        GetComponent<Animator>().SetTrigger("Switch");
    }

    public void ManageCameras()
    {
        if (Manager == 0)
        {
            Cam_2();
            Manager = 1;
        }
        else
        {
            Cam_1();
            Manager = 0;
        }
    }

    void Cam_1()
    {
        // Activate the main camera and disable the drone camera
        mainCam.GetComponent<Camera>().enabled = true;
        droneCam.GetComponent<Camera>().enabled = false;

        // Manage AudioListeners
        AudioListener droneAudio = droneCam.GetComponent<AudioListener>();
        if (droneAudio != null) droneAudio.enabled = false;

        AudioListener mainAudio = mainCam.GetComponent<AudioListener>();
        if (mainAudio != null) mainAudio.enabled = true;

        // Play the AudioSource for the main camera
        //if (mainCamAudioSource != null)
        //{
        //    mainCamAudioSource.mute = false;
        //    if (!mainCamAudioSource.isPlaying)
        //    {
        //        mainCamAudioSource.Play();
        //    }
        //}

        // You can activate or deactivate the drone object as needed
        // Drone.SetActive(false);
    }

    void Cam_2()
    {
        // Activate the drone camera and disable the main camera
        mainCam.GetComponent<Camera>().enabled = false;
        droneCam.GetComponent<Camera>().enabled = true;

        // Manage AudioListeners
        AudioListener mainAudio = mainCam.GetComponent<AudioListener>();
        if (mainAudio != null) mainAudio.enabled = false;

        AudioListener droneAudio = droneCam.GetComponent<AudioListener>();
        if (droneAudio != null) droneAudio.enabled = true;

        //// Mute the AudioSource for the main camera
        //if (mainCamAudioSource != null)
        //{
        //    mainCamAudioSource.mute = true;
        //}

        // Drone.SetActive(true); // You can activate the drone object as needed
    }

    // Method to toggle the AudioSource ON (Unmute and play)
    //public void TurnOnAudio()
    //{
    //    if (mainCamAudioSource != null)
    //    {
    //        mainCamAudioSource.mute = false;
    //        if (!mainCamAudioSource.isPlaying)
    //        {
    //            mainCamAudioSource.Play();
    //        }
    //    }
    //}

    //// Method to toggle the AudioSource OFF (Mute)
    //public void TurnOffAudio()
    //{
    //    if (mainCamAudioSource != null)
    //    {
    //        mainCamAudioSource.mute = true;
    //    }
    //}
}
