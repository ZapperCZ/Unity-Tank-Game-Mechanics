using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] CharacterController Controller;
    [SerializeField] Transform GroundCheckParent;
    [SerializeField] Transform CeilingChechParent;

    [SerializeField] LayerMask GroundMask;

    [Header("Movement")]
    [SerializeField] float defaultSpeed = 12f;
    [SerializeField] float sprintMultiplier = 1.6f;                 //Multiplier of default speed used when sprinting
    [SerializeField] float crouchMultiplier = 0.4f;                 //Multiplier of default speed used when crouching
    [SerializeField] float airMultiplier = 0.4f;                    //Multiplier of default speed used when in air
    [SerializeField] float airDecreaseSpeedMultiplier = 0.2f;       //How fast the speed bleeds of when in air
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float crouchHeight = 1.4f;
    [SerializeField] float crouchDurationMultiplier = 0.01f;

    Vector3 velocity;
    float currentSpeed;

    float resultHeight;
    float crouchInterpolationValue;
    float airDecreaseInterpolationValue;
    float defaultStepOffset;
    float defaultHeight;
    bool isGrounded;

    void Start()
    {
        crouchInterpolationValue = 0;
        airDecreaseInterpolationValue = 0;
        defaultHeight = Controller.height;
        defaultStepOffset = Controller.stepOffset;
        currentSpeed = defaultSpeed;
        resultHeight = defaultHeight;

        float difference = defaultHeight - crouchHeight;
        CeilingChechParent.localPosition = new Vector3(0,(defaultHeight-((difference)/2))/2,0);
        CeilingChechParent.GetComponent<CylinderCollider>().cylinderHeight = difference;

        Debug.Log("Player Movement - Initialized");
    }
    void Update()
    {
        isGrounded = GroundCheckParent.GetComponent<TriggerChildManager>().isTriggered;

        if (isGrounded)
        {
            airDecreaseInterpolationValue = 0f;
            //Takes care of the player getting glitched into the ground when trying to jump onto a ledge
            Controller.stepOffset = defaultStepOffset;

            if (Controller.height == defaultHeight)    //not crouching
            {
                if (Input.GetButtonDown("Jump"))
                {
                    velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
                if (Input.GetButton("Sprint") && Controller.height == defaultHeight)
                {
                    //TODO: Maybe increase camera FOV slightly when sprinting
                    currentSpeed = defaultSpeed * sprintMultiplier;
                }
                else
                {
                    currentSpeed = defaultSpeed;
                }
            }
            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }
        }
        else
        {
            if(airDecreaseInterpolationValue < 0)
            {
                airDecreaseInterpolationValue += airDecreaseSpeedMultiplier * Time.deltaTime;
            }
            currentSpeed = Mathf.SmoothStep(defaultSpeed, defaultSpeed * airMultiplier, airDecreaseInterpolationValue);
            Controller.stepOffset = 0f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        //TODO: Move the player up when getting up from the crouch as currently the controller moves down a slope when getting up
        if(Input.GetButton("Crouch"))
        {
            if(crouchInterpolationValue < 1)
            {
                crouchInterpolationValue += crouchDurationMultiplier * Time.deltaTime;
            }
            currentSpeed = defaultSpeed * crouchMultiplier;     //Do this using interpolation
        }
        else
        {
            if (!CeilingChechParent.GetComponent<TriggerChildManager>().isTriggered)
            {
                if (crouchInterpolationValue > 0)
                {
                    crouchInterpolationValue -= crouchDurationMultiplier * Time.deltaTime;
                }
            }
            //Controller.Move(new Vector3(0, resultHeight - defaultHeight, 0));
        }

        resultHeight = Mathf.SmoothStep(defaultHeight, crouchHeight, crouchInterpolationValue);
        Controller.height = resultHeight;

        Vector3 Direction = transform.right * x + transform.forward * z;

        if (Direction.magnitude > 1)
            Direction /= Direction.magnitude;           //Solves the diagonal movement being faster

        //Debug.Log(Direction);

        Controller.Move(Direction * currentSpeed * Time.deltaTime);
        
        velocity.y += gravity * Time.deltaTime;         //Simulates gravity

        Controller.Move(velocity * Time.deltaTime);
    }
}