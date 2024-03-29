using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    internal enum driveType
    {
        frontWheelDrive = 0,
        rearWheelDrive = 1,
        allWheelDrive = 2
    }

    [SerializeField] driveType _drive;

    public InputManager InputManager;
    [SerializeField] WheelCollider[] wheels = new WheelCollider[4];
    [SerializeField] GameObject[] wheelMesh = new GameObject[4];
    [SerializeField] float _motorTorque;
    [SerializeField] float steeringMax = 4;
    [SerializeField] float radius = 6;
    private Rigidbody rigidbody;
    [SerializeField] float KPH;
    [SerializeField] Rigidbody car;
    //[SerializeField] Rigidbody RearRightWheel;
    [SerializeField] float ForceHandBrake;
    [SerializeField] float accel;
    [SerializeField] float brakeForce;
    [SerializeField] float LimitForce;
    [SerializeField] Skidmarks SkidmarkController;
    [SerializeField] GameObject Smoke_Wheel;
    [SerializeField] ParticleSystem SmokeTire1;
    [SerializeField] ParticleSystem SmokeTire2;

    [Header("------- Texts Debug")]
    [SerializeField] TMP_Text _wheelTorqueText;

    private float lastKPH;

    // Start is called before the first frame update
    void Start()
    {
        getObjects();

        SkidmarkController = FindObjectOfType<Skidmarks>();
        foreach (WheelCollider collider in wheels)
        {
            collider.gameObject.GetComponent<WheelSkid>().skidmarksController = SkidmarkController;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //handbrake();
        animateWheels();
        VehicleMovement();
        SteerVehicle();
        Brake();
        Limit();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        Debug.Log("ssssssssssssssssssss");
    }

    private void Update()
    {
        Reset_pos();
        Wheel_Smoke();


    }



    private void VehicleMovement()
    {

        if (_drive == driveType.frontWheelDrive)
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                // Fait avancer les 2 roues avant
                wheels[i].motorTorque = InputManager.vertical * (_motorTorque / 2) * accel;
                //_wheelTorqueText.text = $"Wheel Torque : {wheels[i].motorTorque}";
            }
        }

        if (_drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = InputManager.vertical * (_motorTorque / 2) * accel;
            }
        }

        if (_drive == driveType.allWheelDrive)
        {
            // Fait avancer les 4 roues
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = InputManager.vertical * (_motorTorque / 4) * accel;
            }
        }

        KPH = rigidbody.velocity.magnitude * 3.6f;

    }

    private void SteerVehicle()
    {
        //acerman steering formula
        //steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;
        if (Input.GetAxis("Jump") == 0)
        {
            if (InputManager.horizontal > 0)
            {
                //rear tracks size is set to 1.5f wheel base has been set to 2.55f (1.5f / 2))) horizontalInput;
                wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * InputManager.horizontal;
                wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * InputManager.horizontal;
            }
            else if (InputManager.horizontal < 0)
            {
                wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * InputManager.horizontal;
                wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * InputManager.horizontal;

            }
            else
            {
                wheels[0].steerAngle = 0;
                wheels[1].steerAngle = 0;
            }

        }
        else
        {
            float steerInput = Input.GetAxis("Horizontal"); transform.Rotate(Vector3.up * steerInput * ForceHandBrake * steeringMax * Time.deltaTime);
        }
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
        InputManager = GetComponent<InputManager>();
        rigidbody = GetComponent<Rigidbody>();
    }

    private void handbrake()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            wheels[2].motorTorque = -1000;
            wheels[3].motorTorque = -1000;
            wheels[1].motorTorque = -1000;
            wheels[0].motorTorque = -1000;
        }
    }

    private void Brake()
    {
        // Appliquer une force inverse � la direction actuelle de la voiture
        Vector3 brakeDirection = -rigidbody.velocity.normalized;

        if (Input.GetKey(KeyCode.Space))
        {
            Debug.Log(brakeDirection);

            // Appliquer la force de freinage
            rigidbody.AddForce(brakeDirection * brakeForce, ForceMode.Force);
        }
    }

    private void Limit()
    {
        // Appliquer une force inverse � la direction actuelle de la voiture
        Vector3 LimitDirection = -rigidbody.velocity.normalized;

        if (KPH > 100)
        {
            Debug.Log(LimitDirection);

            // Appliquer la force de freinage
            rigidbody.AddForce(LimitDirection * LimitForce, ForceMode.Force);
        }
    }

    private void Reset_pos()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position += Vector3.up * 10f;
            transform.rotation = Quaternion.identity;
        }
    }

    private void Awake()
    {
        lastKPH = 0f;
    }

    private void Wheel_Smoke()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Space))
        {
            if (KPH < 1 && KPH > 0)
            { 
                //a l arret
                SmokeTire1.startLifetime = Mathf.Lerp(0, 1, 0);
                SmokeTire2.startLifetime = Mathf.Lerp(0, 1, 0);
            }

            else if (lastKPH > KPH)
            {
                //d�celere
                SmokeTire1.startLifetime = Mathf.Lerp(0, 1, 0.4f);
                SmokeTire2.startLifetime = Mathf.Lerp(0, 1, 0.4f);
            }
            else if(lastKPH < KPH)
            {
                //accelere
                SmokeTire1.startLifetime = Mathf.Lerp(0, 1, 0.7f);
                SmokeTire2.startLifetime = Mathf.Lerp(0, 1, 0.7f);
            }

        }

        else
        {
            SmokeTire1.startLifetime = Mathf.Lerp(0, 1, 0);
            SmokeTire2.startLifetime = Mathf.Lerp(0, 1, 0);
        }


        lastKPH = KPH;
    }
    
    //if (KPH > 15)
    //{
    //    SmokeTire1.startLifetime = Mathf.Lerp(0, 1, 0.4f);
    //    SmokeTire2.startLifetime = Mathf.Lerp(0, 1, 0.4f);
    //}

    //else
    //{
    //    SmokeTire1.startLifetime = Mathf.Lerp(0, 1, 0);
    //    SmokeTire2.startLifetime = Mathf.Lerp(0, 1, 0);
    //}
}





