using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankController : MonoBehaviour
{
    public GameObject LeftSprocket;
    public GameObject RightSprocket;

    public float responsiveness = 0.05f;
    [SerializeField] float maxTorque = 800f;
    float currentLeftTorque = 0f;
    float currentRightTorque = 0f;
    bool leftReverse = false;
    bool rightReverse = false;
    public Vector3 leftExitPos;
    public Vector3 rightExitPos;

    Spin LeftSprocketSpin;
    Spin RightSprocketSpin;
    float torqueStep = 0f;
    bool isSpaceOnLeft = true;
    bool isSpaceOnRight = true;

    public Vector3 CameraOffset;

    [SerializeField] Transform Player;

    private void OnEnable()
    {
        LeftSprocketSpin.applyTorque = true;
        RightSprocketSpin.applyTorque = true;
    }
    private void OnDisable()
    {
        LeftSprocketSpin.applyTorque = false;
        RightSprocketSpin.applyTorque = false;
    }
    void Awake()
    {
        torqueStep = maxTorque / 100;
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
            if(vertical > 0)        //W is being pressed
            {
                if(currentLeftTorque < maxTorque)
                    currentLeftTorque += torqueStep;
                if (currentRightTorque < maxTorque)
                    currentRightTorque += torqueStep;

                leftReverse = false;
                rightReverse = false;
            }
            if (vertical < 0)        //S is being pressed
            {
                if (currentLeftTorque < maxTorque)
                    currentLeftTorque += torqueStep;
                if (currentRightTorque < maxTorque)
                    currentRightTorque += torqueStep;

                leftReverse = true;
                rightReverse = true;
            }
        }
        else
        {
            if (currentLeftTorque > 0)
                currentLeftTorque -= torqueStep;
            if (currentRightTorque > 0)
                currentRightTorque -= torqueStep;
        }
        if (horizontal != 0)        //A or D is being pressed
        {
            if (horizontal > 0)        //D is being pressed
            {
                leftReverse = false;
                rightReverse = true;
            }
            if (horizontal < 0)        //A is being pressed
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

}
