using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JoystickControllerFreeLook : MonoBehaviour
{
    
    public Joystick leftJoystick;  // Reference to the left joystick
    public Joystick rightJoystick;  // Reference to the right joystick

    // Check if any joystick is being used
    public bool IsAnyJoystickActive()
    {
        if (leftJoystick != null && (leftJoystick.Horizontal > 0 || leftJoystick.Vertical > 0))
        {
            return true;
        }

        if (rightJoystick != null && (rightJoystick.Horizontal > 0 || rightJoystick.Vertical > 0))
        {
            return true;
        }

        return false;
    }
}

