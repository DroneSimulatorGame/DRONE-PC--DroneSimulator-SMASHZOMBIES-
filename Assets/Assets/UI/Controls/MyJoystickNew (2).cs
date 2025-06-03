using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MyJoystickNew : MonoBehaviour
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



    public Joystick joystickLeft;
    public Joystick joystickRight;

    private float joystickVertical;
    private float joystickHorizontal;


    public float horizontalInput;
    public float verticalInput;


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
            Stick();
            MoveUpDown();
            MovementForward();
            Rotation();
            ClampingSpeed();
            Swerv();
            DroneSound();






            ourDrone.AddRelativeForce(Vector3.up * upForce);
            ourDrone.rotation = Quaternion.Euler(new Vector3(tiltAmountForward, currentYRotation, tiltAmountSideways));
        }
    }


    public void Stick()
    {
        joystickVertical = -joystickLeft.Vertical; // Invert vertical input
        joystickHorizontal = -joystickLeft.Horizontal; // Invert horizontal input

        // Invert the joystick inputs for right joystick as well if necessary
        horizontalInput = -joystickRight.Horizontal;
        verticalInput = -joystickRight.Vertical;

        // The rest of the logic remains the same
        Vector3 movement = new Vector3(horizontalInput, 0, verticalInput);

        if (verticalInput > sensitivity)
        {
            Debug.Log("Moving forward");
        }
        else if (verticalInput < 0)
        {
            Debug.Log("Moving backward");
        }
    }



    public void MoveUpDown()
    {
        if (Mathf.Abs(joystickVertical) > sensitivity || Mathf.Abs(joystickHorizontal) > sensitivity)
        {
            if (verticalInput > 0 || verticalInput < 0)
            {
                ourDrone.velocity = ourDrone.velocity;
            }
            else if (verticalInput ==0  && horizontalInput ==0)
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 300;// 281;
            }
            else if (verticalInput == 0 && (horizontalInput > 0 || horizontalInput < 0))
            {
                ourDrone.velocity = new Vector3(ourDrone.velocity.x, Mathf.Lerp(ourDrone.velocity.y, 0, Time.deltaTime * 5), ourDrone.velocity.z);
                upForce = 299;
            }
            else if (horizontalInput > 0 || horizontalInput < 0)
            {
                upForce = 410;
            }
        }

        if (Mathf.Abs(joystickVertical) < sensitivity && Mathf.Abs(joystickHorizontal) > sensitivity)
        {
            upForce = 150;// 135;
        }

        if (verticalInput > 0 )
        {
            upForce = Up;
            if (Mathf.Abs(joystickHorizontal) > sensitivity)
            {
                upForce = 500;
            }
        }

        if (verticalInput < 0 )
        {
            upForce = -Down;
        }
        if (verticalInput == 0 && (Mathf.Abs(joystickVertical) < sensitivity && Mathf.Abs(joystickHorizontal) < sensitivity))
        {
            upForce = 98.1f;
        }
    }
    //changed to public
    public void MovementForward()
    {
        if (joystickVertical != 0)
        {
            //upForce = 400;
            ourDrone.AddRelativeForce(Vector3.forward * joystickVertical * moveForwardSpeed);
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 20 * joystickVertical, ref tiltVelocityForward, 0.05f);
        }
        else
        {
            tiltAmountForward = Mathf.SmoothDamp(tiltAmountForward, 1 * joystickVertical, ref tiltVelocityForward, 0.05f);
        }
    }
    //changed to public
    public void Rotation()
    {
        // O'ng joystick gorizontal harakatini aylanish uchun ishlatamiz
        if (Mathf.Abs(horizontalInput) > 0.3f)
        {
            wantedYRotation += rotateAmountByKeys * horizontalInput;
        }

        // Vertikal harakatni yuqoriga va pastga harakat uchun ishlatamiz
        if (Mathf.Abs(verticalInput) > 0.1f)
        {
            // Vertikal inputni [-0.4, 0.4] oralig'ida cheklaymiz
            float clampedVerticalInput = Mathf.Clamp(verticalInput, -2f, 2f);

            // Yuqoriga va pastga harakatni boshqaramiz
            upForce = clampedVerticalInput * (clampedVerticalInput > 0 ? Up : Down);
        }
        else
        {
            // Agar input bo'lmasa, dronni havoda ushlab turamiz
            upForce = 98.1f;
        }

        currentYRotation = Mathf.SmoothDamp(currentYRotation, wantedYRotation, ref rotataYVelocity, 0.25f);
    }

    public void ClampingSpeed()
    {
        if (Mathf.Abs(joystickVertical) > sensitivity && Mathf.Abs(joystickHorizontal) > sensitivity)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
        else if (Mathf.Abs(joystickVertical) > sensitivity && Mathf.Abs(joystickHorizontal) < sensitivity)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
        else if (Mathf.Abs(joystickVertical) < sensitivity && Mathf.Abs(joystickHorizontal) > sensitivity)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
        else if (Mathf.Abs(joystickVertical) < sensitivity && Mathf.Abs(joystickHorizontal) < sensitivity)
        {
            ourDrone.velocity = Vector3.SmoothDamp(ourDrone.velocity, Vector3.zero, ref velocityToSmoothDampToZero, 0.2f);
        }
    }
    //changed to public
    public void Swerv()
    {
        if (Mathf.Abs(joystickHorizontal) > sensitivity)
        {
            ourDrone.AddRelativeForce(Vector3.right * joystickHorizontal * sideMoveAmount);
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, -20 * joystickHorizontal, ref tiltAmountVelocity, 0.05f);
        }
        else
        {
            tiltAmountSideways = Mathf.SmoothDamp(tiltAmountSideways, 0, ref tiltAmountVelocity, 0.1f);
        }
    }
    //changed to public
    //changed to public
    public void DroneSound()
    {
        droneSound.pitch = 1 + (ourDrone.velocity.magnitude / 90);
    }


}