using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockFirstJoint : MonoBehaviour
{
    private GameObject _anchore;
   
    public void Init(GameObject anchore)
    {
        _anchore = anchore;
    }

    // Update is called once per frame
    void Update()
    {
        if(_anchore == null)
        {
            print("anchore not set");
            return;
        }

        transform.position = _anchore.transform.position;
    }
}
