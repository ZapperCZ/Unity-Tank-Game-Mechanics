using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float Torque = 100f;
    private void FixedUpdate()
    {
        this.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0,Torque,0));
    }
}
