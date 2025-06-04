using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher: MonoBehaviour 
{
    
    public GameObject Drone;
    public GameObject Menue;
    public AudioListener DroneAudio;
    public AudioListener MenueAudio;
    



    public void Start()
    {
        Drone.SetActive(false);
        Menue.SetActive(true);
        DroneAudio.enabled = false;
        MenueAudio.enabled = true;
    }



    private void Update()
    {
        if (Input.GetKey(KeyCode.C))
        {
            Drone.SetActive(true);
            DroneAudio.enabled=true;
            Menue.SetActive(false);
            MenueAudio.enabled=false;
            
        }
        if (Input.GetKey(KeyCode.V)) 
        {
            Drone.SetActive(false);
            DroneAudio.enabled = false;
            
            Menue.SetActive(true);
            MenueAudio.enabled = true;
        }
    }

}
