using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject Player;
    public GameObject Child;
    public float speed;
    private controller RR;

    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        Child = Player.transform.Find("camera constraint").gameObject;
        RR = Player.GetComponent<controller>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        follow();
        //speed = (RR.KPH >= 50) ? 20 : RR.KPH / 4;
    }

    private void follow()
    {
        gameObject.transform.position = Vector3.Lerp(transform.position, Child.transform.position, Time.deltaTime * speed);
        gameObject.transform.LookAt(Player.gameObject.transform.position);
    }
}
