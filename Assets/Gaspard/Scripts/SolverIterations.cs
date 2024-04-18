using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolverIterations : MonoBehaviour
{
    public int solver;
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        solver = rb.solverIterations;
        solver = 30;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
