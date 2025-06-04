using System.Collections;
using UnityEngine;

public class StartFinish : MonoBehaviour
{
    [Header("Rotor Objects (parent or root)")]
    public GameObject Rotor1;
    public GameObject Rotor2;
    public AudioSource dronjala;

    [Header("References")]
    public DroneUpEndDownAnimator DroneUpEndDownAnimator;
    public MyJoystickNew2 MyJoystickNew2;

    // Cached animator references
    private Animator rotor1Animator;
    private Animator rotor2Animator;

    void Start()
    {

        dronjala.enabled = false;
        // Try to get Animator from self or child
        rotor1Animator = Rotor1.GetComponent<Animator>();
        if (rotor1Animator == null)
            rotor1Animator = Rotor1.GetComponentInChildren<Animator>();

        rotor2Animator = Rotor2.GetComponent<Animator>();
        if (rotor2Animator == null)
            rotor2Animator = Rotor2.GetComponentInChildren<Animator>();
    }

    public void startRotors()
    {
        Debug.Log("Starting rotors...");
        dronjala.enabled = true;

        if (rotor1Animator != null) rotor1Animator.enabled = true;
        else Debug.LogWarning("Rotor1 Animator not found!");

        if (rotor2Animator != null) rotor2Animator.enabled = true;
        else Debug.LogWarning("Rotor2 Animator not found!");
    }

    public void stopRotors()
    {
        Debug.Log("Stopping rotors...");

        dronjala.enabled = false;
        if (rotor1Animator != null) rotor1Animator.enabled = false;
        else Debug.LogWarning("Rotor1 Animator not found!");

        if (rotor2Animator != null) rotor2Animator.enabled = false;
        else Debug.LogWarning("Rotor2 Animator not found!");
    }

    public void stopMvement()
    {
        // Placeholder for movement stop logic (if needed)
        // MyJoystickNew2.isActive = false;
    }
}
