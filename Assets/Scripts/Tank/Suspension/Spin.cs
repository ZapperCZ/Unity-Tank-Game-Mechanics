using UnityEngine;

public class Spin : MonoBehaviour
{
    [SerializeField] bool applyTorque = false;          //Whether the torque should be applied or not
    [SerializeField] float Torque = 0f;                 //The amount of torque applied
    [SerializeField] bool FlipSpinDirection = false;    //Whether the spinning direction should be flipped or not
    [SerializeField] float MaxAngularVelocity = 20;     //The rotational limit, by default set to 7 by Unity

    int Direction = 1;
    //TODO: Add variable that allows to change the axis of the spin

    private void OnValidate()
    {
        Direction = FlipSpinDirection ? -1 : 1;                 //If enabled, direction is flipped (-1), else it is unchanged (1)
        MaxAngularVelocity = MaxAngularVelocity < 0 ? 0 : MaxAngularVelocity;   //If less than 0, change to 0, otherwise leave as is
    }
    private void FixedUpdate()
    {
        if(applyTorque)
        {
            this.GetComponent<Rigidbody>().AddRelativeTorque(new Vector3(Torque * Direction, 0, 0));  //Apply the torque
        }
        this.GetComponent<Rigidbody>().maxAngularVelocity = MaxAngularVelocity;                 //Apply the rotational velocity limit
    }
}
