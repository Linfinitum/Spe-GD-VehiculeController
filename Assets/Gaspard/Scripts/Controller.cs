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

    private bool handbrake = false;
    private bool respawn = false;
    private Vector2 MoveSide = Vector2.zero;
    private float Rear;
    private float Forward;
    private float RearForward = 0f;

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
        animateWheels();
        VehicleMovement();
        SteerVehicle();
        Brake();
        Limit();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MoveSide = context.ReadValue<Vector2>();
    }
    public void OnRear(InputAction.CallbackContext context)
    {
        Rear = context.ReadValue<float>();
    }
    public void OnForward(InputAction.CallbackContext context)
    {
        Forward = context.ReadValue<float>();
    }
    public void OnHandbrake(InputAction.CallbackContext context)
    {
        handbrake = context.action.triggered;
    }

    public void OnRespawn(InputAction.CallbackContext context)
    {
        respawn = context.action.triggered;
    }

    private void Update()
    {
        Reset_pos();
        Wheel_Smoke();

        RearForward = (Rear * -1) + Forward;
    }



    private void VehicleMovement()
    {

        if (_drive == driveType.frontWheelDrive)
        {
            for (int i = 0; i < wheels.Length - 2; i++)
            {
                // Fait avancer les 2 roues avant
                wheels[i].motorTorque = RearForward * (_motorTorque / 2) * accel;
                //_wheelTorqueText.text = $"Wheel Torque : {wheels[i].motorTorque}";
            }
        }

        if (_drive == driveType.rearWheelDrive)
        {
            for (int i = 2; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = RearForward * (_motorTorque / 2) * accel;
            }
        }

        if (_drive == driveType.allWheelDrive)
        {
            // Fait avancer les 4 roues
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].motorTorque = RearForward * (_motorTorque / 4) * accel;
            }
        }

        KPH = rigidbody.velocity.magnitude * 3.6f;

    }

    private void SteerVehicle()
    {
        //acerman steering formula
        if (handbrake == false)
        {
            print(MoveSide.x);
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * MoveSide.x;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * MoveSide.x;
        }
        else
        {
            transform.Rotate(Vector3.up * MoveSide.x * ForceHandBrake * steeringMax * Time.deltaTime);
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
        //InputManager = GetComponent<InputManager>();
        rigidbody = GetComponent<Rigidbody>();
    }


    private void Brake()
    {

        print("collision");
        // Appliquer une force inverse à la direction actuelle de la voiture
        Vector3 brakeDirection = -rigidbody.velocity.normalized;

        if (handbrake)
        {
            //Debug.Log(brakeDirection);

            // Appliquer la force de freinage
            rigidbody.AddForce(brakeDirection * brakeForce, ForceMode.Force);
        }

    }

    private void Limit()
    {
        // Appliquer une force inverse à la direction actuelle de la voiture
        Vector3 LimitDirection = -rigidbody.velocity.normalized;

        if (KPH > 100)
        {
            //Debug.Log(LimitDirection);

            // Appliquer la force de freinage
            rigidbody.AddForce(LimitDirection * LimitForce, ForceMode.Force);
        }
    }

    private void Reset_pos()
    {
        if (respawn == true)
        {
            // Récupérer la rotation actuelle
            Quaternion currentRotation = transform.rotation;

            // Réinitialiser la rotation sur l'axe X uniquement
            currentRotation.x = 0f;
            currentRotation.z = 0f;

            // Réappliquer la rotation
            transform.rotation = currentRotation;

            transform.position += Vector3.up * 2f;
            //transform.rotation = Quaternion.identity;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ;
            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX;
            rigidbody.constraints = RigidbodyConstraints.None;

            respawn = false;
        }
    }

    private void Awake()
    {
        lastKPH = 0f;
    }

    private void Wheel_Smoke()
    {
        if (Forward >= 0.8f || Rear >= 0.8f || handbrake == true)
        {
            if (KPH < 1 && KPH > 0)
            {
                //a l arret
                SmokeTire1.startLifetime = Mathf.Lerp(0, 1, 0);
                SmokeTire2.startLifetime = Mathf.Lerp(0, 1, 0);
            }

            else if (lastKPH > KPH)
            {
                //décelere
                SmokeTire1.startLifetime = Mathf.Lerp(0, 1, 0.4f);
                SmokeTire2.startLifetime = Mathf.Lerp(0, 1, 0.4f);
            }
            else if (lastKPH < KPH)
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





