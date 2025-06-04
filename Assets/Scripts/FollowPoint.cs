using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPoint : MonoBehaviour
{
    [SerializeField] public Transform LookAt;
    [SerializeField] public Vector3 Offset;

    private Camera cam;


    private void Start()
    {
        cam = Camera.main;

    }


    private void Update()
    {
        Vector3 pos  = cam.WorldToScreenPoint(LookAt.position + Offset);

        if (transform.position != pos)
            transform.position = pos;
        


    }


}
