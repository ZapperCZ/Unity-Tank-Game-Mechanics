using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    public float Torque = 0f;
    public bool FlipSpinDirection = false;
    public float MaxAngularVelocity = 7;
    int Direction = 1;

    private void OnValidate()
    {
        Direction = FlipSpinDirection ? -1 : 1;                 //If enabled, direction is flipped (-1), else it is unchanged (1)
        MaxAngularVelocity = MaxAngularVelocity < 0 ? 0 : MaxAngularVelocity;   //If less than 0, change to 0, otherwise leave as is
    }
    private void FixedUpdate()
    {
        this.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(0,Torque * Direction,0));
        this.GetComponent<Rigidbody>().maxAngularVelocity = MaxAngularVelocity;
    }
}
