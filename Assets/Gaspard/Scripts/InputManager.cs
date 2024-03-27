using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public float vertical;
    public float horizontal;
    public bool handbrake;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        vertical = Input.GetAxis("Vertical");
        horizontal = Input.GetAxis("Horizontal");

        //Jump psk cela retourne une valeur entre 0 et 1
        handbrake = (Input.GetAxis("Jump") != 0)? true : false;

        

        //if (handbrake = Input.GetAxis("Jump") != 0)
        //{
        //    handbrake = true;
        //}
        //else
        //{
        //    handbrake = false;
        //}


    }
}
