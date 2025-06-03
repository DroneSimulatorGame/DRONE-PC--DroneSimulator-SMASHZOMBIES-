using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILookAt : MonoBehaviour
{
    [Header("Tweaks")]
    [SerializeField] public Transform LookAt;
    [SerializeField] public Vector3 Offset;

    [Header("Logic")]
    public Camera Cam;


    private void Start()
    {
        
    }


    private void LateUpdate()
    {
        Vector3 pos = Cam.WorldToScreenPoint(LookAt.position + Offset);

        if (transform.position != pos)
        {
            transform.position = pos;
        }


    }



}
