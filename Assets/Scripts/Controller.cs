using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    internal enum driveType
    {
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }

    [SerializeField] driveType drive;

    [SerializeField] InputManager IM;
    [SerializeField] WheelCollider[] wheels = new WheelCollider[4];
    [SerializeField] GameObject[] wheelMesh = new GameObject[4];
    [SerializeField] float motorTorque = 200;
    [SerializeField] float steeringMax = 4;
    [SerializeField] float radius = 6;
    
    // Start is called before the first frame update
    void Start()
    {
        getObjects();
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        animateWheels();
        VehicleMovement();
        SteerVehicle();
    }

    private void VehicleMovement()
    {
        //Si j'appuie sur Z les roues prennent la puissance du torque
        //if (Input.GetKey(KeyCode.W))
        //{
        //    for (int i = 0; i < wheels.Length; i++)
        //    {
        //        wheels[i].motorTorque = torque;
        //    }
        //}


        //else
        //{
        //    for (int i = 0; i < wheels.Length; i++)
        //    {
        //        wheels[i].motorTorque = 0;
        //    }
        //}

        //
        //if (Input.GetAxis("Horizontal") != 0)
        //{
        //    for (int i = 0; i < wheels.Length - 2; i++)
        //    {
        //        wheels[i].steerAngle = Input.GetAxis("Horizontal") * steeringMax;
        //    }
        //}
        //else
        //{

        //    for (int i = 0; i < wheels.Length - 2; i++)
        //    {
        //        wheels[i].steerAngle = 0;
        //    }

        //}

        if (drive == driveType.allWheelDrive)
        {
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque/4);
            }
        }

        if (drive == driveType.frontWheelDrive)
        {
            for (int i = 0; i < wheels.Length -2; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque / 2);
            }
        }

        if (drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = IM.vertical * (motorTorque / 2);
            }
        }

    }

    private void SteerVehicle()
    {
        //acerman steering formula
        //steerAngle = Mathf. Rad2Deg * Mathf.Atan(2.55f/ (radius + (1.5f/ 2)))* horizontalInput;
        if (IM.horizontal > 0)
        {
            //rear tracks size is set to 1.5f wheel base has been set to 2.55f (1.5f / 2))) horizontalInput;
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
        }
        else if (IM.horizontal < 0)
        {
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * IM.horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * IM.horizontal;
            //transform.Rotate(Vector3.up * steerHelping);
        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }


        //for (int i = 0; i < wheels.Length -2; i++)
        //{
        //    wheels[i].steerAngle = IM.horizontal * steeringMax;
        //}

    }

    void animateWheels()
    {
        Vector3 wheelPosition = Vector3.zero;
        Quaternion wheelRotation = Quaternion.identity;
        for (int i = 0; i < 4; i++)
        {
            wheels[i].GetWorldPose(out wheelPosition, out wheelRotation);
            wheelMesh[i].transform.position = wheelPosition;
            wheelMesh[i].transform.rotation = wheelRotation;
        }
    }

    //Permet au reste du script d'interagir avec l'InputManager
    private void getObjects()
    {
        IM = GetComponent<InputManager>();
    }


}
