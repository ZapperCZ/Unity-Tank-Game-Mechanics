using UnityEngine;

public class Spin : MonoBehaviour
{
    public bool applyTorque = false;          //Whether the torque should be applied or not
    public float Torque = 0f;                 //The amount of torque applied
    public bool FlipSpinDirection = false;    //Whether the spinning direction should be flipped or not
    [SerializeField] Vector3 SpinAxis = new Vector3(0, 1, 0);   
    [SerializeField] float MaxAngularVelocity = 20;     //The rotational limit, by default set to 7 by Unity
    [SerializeField] float delay = 0;

    [SerializeField] float internalTimer = 0;
    int Direction = 1;

    private void Awake()
    {
        this.GetComponent<Rigidbody>().maxAngularVelocity = MaxAngularVelocity;     //Apply the rotational velocity limit
    }
    private void OnValidate()
    {
        MaxAngularVelocity = MaxAngularVelocity < 0 ? 0 : MaxAngularVelocity;       //If less than 0, change to 0, otherwise leave as is
        this.GetComponent<Rigidbody>().maxAngularVelocity = MaxAngularVelocity;     //Apply the rotational velocity limit
    }
    private void Update()
    {
        Direction = FlipSpinDirection ? -1 : 1;                 //If enabled, direction is flipped (-1), else it is unchanged (1)
        if(delay != 0)
        {
            if (internalTimer < delay)
            {
                internalTimer += Time.deltaTime;
                applyTorque = false;
            }
            else
            {
                applyTorque = true;
            }
        }
    }
    private void FixedUpdate()
    {
        if(applyTorque)
            this.GetComponent<Rigidbody>().AddRelativeTorque(SpinAxis * Torque * Direction);  //Apply the torque
    }
}
