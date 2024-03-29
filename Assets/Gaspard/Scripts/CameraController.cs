using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject LookAt;
    public GameObject Constraint;
    public float speed;

    private void FixedUpdate()
    {
        follow();
    }

    private void follow()
    {
        gameObject.transform.position = Vector3.Lerp(transform.position, Constraint.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(LookAt.gameObject.transform.position);
    }
}

//