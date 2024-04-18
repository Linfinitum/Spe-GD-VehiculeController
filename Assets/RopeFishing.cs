using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeFishing : MonoBehaviour
{

    [SerializeField] private LockFirstJoint _joint;
    public void Init(GameObject anchore)
    {
        if(_joint == null)
        {
            print("sphere joint not set");
            return;
        }

        _joint.Init(anchore);
    }
}
