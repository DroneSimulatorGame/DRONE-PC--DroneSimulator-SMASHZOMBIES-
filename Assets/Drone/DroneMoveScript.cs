using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMoveScript : MonoBehaviour
{
    #region


    //changed to public
    public Rigidbody ourDrone;

    [Range(-1, 1)]
    public float sensitivity = 0.2f;

    [Range(0, 3000)]
    public float moveForwardSpeed = 500.0f;

    [Range(0, 1500)]
    public float sideMoveAmount = 300.0f;

    [Range(0, 1500)]
    public float Up = 450.0f;

    [Range(0, 1000)]
    public float Down = 200.0f;

    [Range(0, 20)]
    public float rotateAmountByKeys = 2.5f;

    [Range(-1000, 3000)]
    public float upForce;

    //changed to public
    public float tiltAmountForward = 0;
    public float tiltVelocityForward;
    public float tiltAmountSideways;
    public float tiltAmountVelocity;
    public float wantedYRotation;
    [HideInInspector] public float currentYRotation;
    public float rotataYVelocity;
    public Vector3 velocityToSmoothDampToZero;
    public AudioSource droneSound;

    //changed to public
    public float I;
    public float J;
    public float K;
    public float L;


    public bool isActive = true;

    #endregion

    //changed to public
    public void Awake()
    {
        ourDrone = GetComponent<Rigidbody>();
        droneSound = gameObject.transform.Find("drone_sound").GetComponent<AudioSource>();
        //isActive = false;

    }



    //changed to public
    public void FixedUpdate()
    {
        if (isActive)
        {
            //MouseMov();
            MoveUpDown();
            MovementForward();
            Rotation();
            ClampingSpeed();
            Swerv();
            DroneSound();
            Turn180And360();






            ourDrone.AddRelativeForce(Vector3.up * upForce);
            ourDrone.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways));
        }
    }


    //public void TakeOff()
    //{
      //  isActive = true;
    //}
    //changed to public
    public void MoveUpDown()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity || Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
        {
            if (Input.GetKey(KeyCode.I) || Input.GetKey(KeyCode.K))
            {
                ourDrone.velocity = ourDrone.velocity;
            }
            else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 300;// 281;
            }
            else if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)))
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 299;
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                upForce = 410;
            }
        }

        if (Mathf.Abs(Input.GetAxis("Vertical")) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
        {
            upForce = 150;// 135;
        }

        if (Input.GetKey(KeyCode.I))
        {
            upForce = Up;
            if (Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
            {
                upForce = 500;
            }
        }
        if (Input.GetKey(KeyCode.K))
        {
            upForce = -Down;
        }
        if (!Input.GetKey(KeyCode.I) && !Input.GetKey(KeyCode.K) && (Mathf.Abs(Input.GetAxis("Vertical")) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) < sensitivity) )
        {
            upForce = 98.1f;
        }
    }
    //changed to public
    public void MovementForward()
    {
        if (Input.GetAxis("Vertical") != 0)
        {
            //upForce = 400;
            ourDrone.AddRelativeForce(Vector3.forward * Input.GetAxis("Vertical") * moveForwardSpeed);
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * Input.GetAxis("Vertical"), ref tiltVelocityForward, 0.05f);
        }
    }
    //changed to public
    public void Rotation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            wantedYRotation -= rotateAmountByKeys;
        }
        if (Input.GetKey(KeyCode.D))
        {
            wantedYRotation += rotateAmountByKeys;
        }


        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotataYVelocity, 0.25f);
    }
    //changed to public
    public void ClampingSpeed()
    {
        if (Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
        else if (Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) < sensitivity)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
        else if (Mathf.Abs(Input.GetAxis("Vertical")) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
        else if (Mathf.Abs(Input.GetAxis("Vertical")) < sensitivity && Mathf.Abs(Input.GetAxis("Horizontal")) < sensitivity)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
    }
    //changed to public
    public void Swerv()
    {
        if (Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
        {
            ourDrone.AddRelativeForce(Vector3.right * Input.GetAxis("Horizontal") * sideMoveAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * Input.GetAxis("Horizontal"), ref tiltAmountVelocity, 0.05f);
        }
        else
        {
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
        }
    }
    //changed to public
    public void Turn180And360()
    {
        if (Input.GetKey(KeyCode.Q) && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
        {
            ourDrone.AddRelativeForce(Vector3.right * Mathf.Abs(Input.GetAxis("Horizontal")) * sideMoveAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 180 * Mathf.Abs(Input.GetAxis("Horizontal")), ref tiltAmountVelocity, 0.1f);
        }
        if (Input.GetKey(KeyCode.Q) && Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity)
        {
            ourDrone.AddRelativeForce(Vector3.right * Mathf.Abs(Input.GetAxis("Vertical")) * sideMoveAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 180 * Mathf.Abs(Input.GetAxis("Vertical")), ref tiltAmountVelocity, 0.1f);
        }
        if (Input.GetKey(KeyCode.E) && Mathf.Abs(Input.GetAxis("Horizontal")) > sensitivity)
        {
            ourDrone.AddRelativeForce(Vector3.right * -Mathf.Abs(Input.GetAxis("Horizontal")) * sideMoveAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 180 * -Mathf.Abs(Input.GetAxis("Horizontal")), ref tiltAmountVelocity, 0.1f);
        }
        if (Input.GetKey(KeyCode.E) && Mathf.Abs(Input.GetAxis("Vertical")) > sensitivity)
        {
            ourDrone.AddRelativeForce(Vector3.right * -Mathf.Abs(Input.GetAxis("Vertical")) * sideMoveAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 180 * -Mathf.Abs(Input.GetAxis("Vertical")), ref tiltAmountVelocity, 0.1f);
        }


    }
    //changed to public
    public void DroneSound()
    {
        droneSound.pitch = 1 + (ourDrone.velocity.magnitude / 90);
    }


}