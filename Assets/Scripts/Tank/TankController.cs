using UnityEngine;

public class TankController : MonoBehaviour
{
    public GameObject LeftSprocket;
    public GameObject RightSprocket;

    [SerializeField] TurretController TurretControllerScript;
    [SerializeField] ShootingSystem ShootingSystemScript;
    public Transform CameraFocusPoint;

    public float responsiveness = 0.05f;
    [SerializeField] float maxTorque = 600f;
    float currentLeftTorque = 0f;
    float currentRightTorque = 0f;
    bool leftReverse = false;
    bool rightReverse = false;
    public Vector3 leftExitPos;
    public Vector3 rightExitPos;

    Spin LeftSprocketSpin;
    Spin RightSprocketSpin;
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
    }
    private void OnDisable()
    {
        LeftSprocketSpin.applyTorque = false;
        RightSprocketSpin.applyTorque = false;

        TurretControllerScript.enabled = false;
        ShootingSystemScript.enabled = false;
        transform.tag = "Untagged";
        AddTagToChildren(transform, "Untagged");
    }
    void Awake()
    {
        LeftSprocketSpin = LeftSprocket.GetComponent<Spin>();
        RightSprocketSpin = RightSprocket.GetComponent<Spin>();
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
            if(vertical > 0)        //W is being pressed - forward
            {
                if(currentLeftTorque < maxTorque)
                    currentLeftTorque += responsiveness * Time.deltaTime;
                if (currentRightTorque < maxTorque)
                    currentRightTorque += responsiveness * Time.deltaTime;

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
