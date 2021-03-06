using UnityEngine;

public class TankController : MonoBehaviour
{
    public GameObject LeftSprocket;
    public GameObject RightSprocket;

    [SerializeField] Speedometer speedometer;
    [SerializeField] TurretController TurretControllerScript;
    [SerializeField] ShootingSystem ShootingSystemScript;
    public Transform CameraFocusPoint;

    [SerializeField] float responsiveness = 0.05f;
    [SerializeField] float maxTorque = 600f;
    [SerializeField] float idleForce = 1000f;               //The force stopping the sprocket hinges when no input is being received
    float currentLeftTorque = 0f;
    float currentRightTorque = 0f;
    bool leftReverse = false;
    bool rightReverse = false;
    public Vector3 leftExitPos;
    public Vector3 rightExitPos;

    Spin LeftSprocketSpin;
    Spin RightSprocketSpin;
    HingeJoint LeftSprocketHinge;
    HingeJoint RightSprocketHinge;
    bool isSpaceOnLeft = true;
    bool isSpaceOnRight = true;

    public float CameraDistance = 4;

    [SerializeField] Transform Player;

    private void OnEnable()
    {
        LeftSprocketSpin.applyTorque = true;
        RightSprocketSpin.applyTorque = true;

        TurretControllerScript.enabled = true;
        ShootingSystemScript.enabled = true;
        transform.tag = "Player Controlled";
        AddTagToChildren(transform, "Player Controlled");

        if(Speedometer.Instance != null)
        {
            Speedometer.Instance.Vehicle = transform;
            Speedometer.Instance.enabled = true;
        }
    }
    private void OnDisable()
    {
        LeftSprocketSpin.applyTorque = false;
        RightSprocketSpin.applyTorque = false;

        TurretControllerScript.enabled = false;
        ShootingSystemScript.enabled = false;
        transform.tag = "Untagged";
        AddTagToChildren(transform, "Untagged");

        if (Speedometer.Instance != null)
        {
            Speedometer.Instance.enabled = false;
        }
    }
    void Awake()
    {
        LeftSprocketSpin = LeftSprocket.GetComponent<Spin>();
        RightSprocketSpin = RightSprocket.GetComponent<Spin>();
        LeftSprocketHinge = LeftSprocket.GetComponent<HingeJoint>();
        RightSprocketHinge = RightSprocket.GetComponent<HingeJoint>();
    }
    void Update()
    {
        GetInput();
        if (Input.GetButtonDown("Use Key"))
        {
            Player.GetComponent<PlayerControllerManager>().SwitchController(true, this.isSpaceOnLeft, this.isSpaceOnRight);
        }
        ApplyTorque();
    }
    void ApplyTorque()
    {
        LeftSprocketSpin.FlipSpinDirection = leftReverse;
        RightSprocketSpin.FlipSpinDirection = rightReverse;
        LeftSprocketSpin.Torque = currentLeftTorque;
        RightSprocketSpin.Torque = currentRightTorque;
    }
    void GetInput()
    {
        float vertical = Input.GetAxisRaw("Vertical");
        float horizontal = Input.GetAxisRaw("Horizontal");
        if (vertical != 0)          //W or S is being pressed
        {
            LeftSprocketHinge.useMotor = false;
            RightSprocketHinge.useMotor = false;
            if(vertical > 0)        //W is being pressed - forward
            {
                if(currentLeftTorque < maxTorque)
                    currentLeftTorque += responsiveness * Mathf.Abs(vertical) * Time.deltaTime;
                if (currentRightTorque < maxTorque)
                    currentRightTorque += responsiveness * Mathf.Abs(vertical) * Time.deltaTime;

                leftReverse = false;
                rightReverse = false;
            }
            if (vertical < 0)        //S is being pressed - backwards
            {
                if (currentLeftTorque < maxTorque)
                    currentLeftTorque += responsiveness * Time.deltaTime;
                if (currentRightTorque < maxTorque)
                    currentRightTorque += responsiveness * Time.deltaTime;

                leftReverse = true;
                rightReverse = true;
            }
        }
        else
        {
            LeftSprocketHinge.useMotor = true;
            RightSprocketHinge.useMotor = true;

            if (currentLeftTorque > 0)
                if (currentLeftTorque > responsiveness * Time.deltaTime)
                    currentLeftTorque -= responsiveness * Time.deltaTime;
                else
                    currentLeftTorque = 0;
            if (currentRightTorque > 0)
                if (currentRightTorque > responsiveness * Time.deltaTime)
                    currentRightTorque -= responsiveness * Time.deltaTime;
                else
                    currentRightTorque = 0;
        }
        if (horizontal != 0)        //A or D is being pressed
        {
            if (horizontal > 0)        //D is being pressed - right
            {
                leftReverse = false;
                rightReverse = true;
            }
            if (horizontal < 0)        //A is being pressed - left
            {
                leftReverse = true;
                rightReverse = false;
            }
        }
    }
    public void EvaluateTrigger(double localX, bool isExiting)
    {
        if (localX < 0)  //Left
        {
            isSpaceOnLeft = isExiting;
        }
        else            //Right
        {
            isSpaceOnRight = isExiting;
        }
    }
    void AddTagToChildren(Transform Parent, string tag)
    {
        foreach (Transform child in Parent)
        {
            child.tag = tag;
            if (child.childCount >= 0)
            {
                AddTagToChildren(child, tag);
            }
        }
    }
}
