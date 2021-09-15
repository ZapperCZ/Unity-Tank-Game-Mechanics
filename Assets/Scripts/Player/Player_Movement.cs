using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] CharacterController Controller;
    [SerializeField] Transform GroundCheckParent;
    [SerializeField] Transform CeilingChechParent;

    [SerializeField] LayerMask GroundMask;

    [Header("Ground Check")]
    [Range(6, 50)]
    [SerializeField] int groundCheckSides = 10;
    [Range(0.1f, 10)]
    [SerializeField] float groundCheckSideWidth = 10;
    [SerializeField] bool sideWidthLocked = true;
    [Range(0.1f, 5)]
    [SerializeField] float groundCheckDiameter = 10;
    [SerializeField] bool diameterLocked = true;
    [SerializeField] float groundCheckThickness = 0.1f;
    [Header("Movement")]
    [SerializeField] float defaultSpeed = 12f;
    [SerializeField] float sprintMultiplier = 1.6f;
    [SerializeField] float crouchMultiplier = 0.4f;
    [SerializeField] float airMultiplier = 0.4f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float crouchHeight = 1.4f;
    [SerializeField] float crouchDurationMultiplier = 0.01f;

    Vector3 velocity;
    float currentSpeed;

    float resultHeight;
    float crouchInterpolationValue;
    float defaultStepOffset;
    float defaultHeight;
    bool prevSideWidthLocked;
    bool prevDiameterLocked;
    //float prevSides;
    float prevSideWidth;
    float prevDiameter;
    bool regenerate = false;
    bool isGrounded;

    void Start()
    {
        //prevSides = groundCheckSides;
        crouchInterpolationValue = 0;
        defaultHeight = Controller.height;
        defaultStepOffset = Controller.stepOffset;
        prevSideWidth = groundCheckSideWidth;
        prevDiameter = groundCheckDiameter;
        prevSideWidthLocked = sideWidthLocked;
        prevDiameterLocked = diameterLocked;
        CreateGroundCheckColliders();
        currentSpeed = defaultSpeed;
        resultHeight = defaultHeight;
        
        Debug.Log("Player Movement - Initialized");
    }
    private void OnValidate()
    {
        /*
         * TODO: Only regenerate when number of sides is changed, otherwise change the transform of existing colliders 
         * as that is less resource intensive
        */

        //Allows only even numbers
        if (groundCheckSides % 2 == 1)
        {
            groundCheckSides -= 1;
        }

        //Locks the currently changing variable
        //Has to be an else if otherwise they would affect each other
        if (prevSideWidth != groundCheckSideWidth)
        {
            prevSideWidth = groundCheckSideWidth;
            sideWidthLocked = true;
        }
        else if (prevDiameter != groundCheckDiameter)
        {
            prevDiameter = groundCheckDiameter;
            diameterLocked = true;
        }

        //Only 1 can be checked at the same time, but both can be unchecked
        if (sideWidthLocked && diameterLocked)
        {
            if (sideWidthLocked != prevSideWidthLocked)
            {
                prevSideWidthLocked = sideWidthLocked;
                diameterLocked = prevDiameterLocked = !sideWidthLocked;
            }
            if (diameterLocked != prevDiameterLocked)
            {
                prevDiameterLocked = diameterLocked;
                sideWidthLocked = prevSideWidthLocked = !diameterLocked;
            }
        }

        //TODO: Maybe switch the complex equation for my approximation once the n-gon gets complex

        if (sideWidthLocked)
        {
            //prevDiameter = groundCheckDiameter = groundCheckSides * groundCheckSideWidth / Mathf.PI;
            prevDiameter = groundCheckDiameter = groundCheckSideWidth / Mathf.Tan(Mathf.PI / groundCheckSides);
        }
        if (diameterLocked)
        {
            //prevSideWidth = groundCheckSideWidth = Mathf.PI * groundCheckDiameter / groundCheckSides;
            prevSideWidth = groundCheckSideWidth = groundCheckDiameter * Mathf.Tan(Mathf.PI / groundCheckSides);
        }
        regenerate = true;
    }
    void Update()
    {
        isGrounded = false;
        if (regenerate)
        {
            regenerate = false;
            CreateGroundCheckColliders();
        }

        foreach (Transform groundCheckPart in GroundCheckParent.GetComponentInChildren<Transform>())
        {
            if (groundCheckPart.GetComponent<GroundCheckPiece>().Grounded)
            {
                isGrounded = true;
                break;
            }
        }

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (Input.GetButton("Sprint") && isGrounded)
        {
            //TODO: Maybe increase camera FOV slightly when sprinting
            currentSpeed = defaultSpeed * sprintMultiplier;
        }
        else
        {
            currentSpeed = defaultSpeed;
        }

        //TODO: Move the player up when getting up from the crouch as currently the controller moves down the stairs when standing up
        if(Input.GetButton("Crouch"))
        {
            if(crouchInterpolationValue < 1)
            {
                crouchInterpolationValue += crouchDurationMultiplier * Time.deltaTime;
            }
        }
        else
        {
            if (crouchInterpolationValue > 0)
            {
                crouchInterpolationValue -= crouchDurationMultiplier * Time.deltaTime;
            }
            //Controller.Move(new Vector3(0, resultHeight - defaultHeight, 0));
        }

        resultHeight = Mathf.SmoothStep(defaultHeight, crouchHeight, crouchInterpolationValue);
        Debug.Log(resultHeight + " - " + crouchInterpolationValue);
        Controller.height = resultHeight;



        Vector3 Direction = transform.right * x + transform.forward * z;
        Controller.Move(Direction * currentSpeed * Time.deltaTime);

        //TODO: Slow down when not grounded
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //Takes care of the player getting glitched into the ground when trying to jump onto a ledge
        if (isGrounded)
        {
            Controller.stepOffset = defaultStepOffset;
        }
        else
        {
            Controller.stepOffset = 0f;
        }
        
        velocity.y += gravity * Time.deltaTime;

        Controller.Move(velocity * Time.deltaTime);
    }
    #region GroundCheck Colliders
    void DestroyGroundCheckColliders()
    {
        foreach (Transform prevCollider in GroundCheckParent.GetComponentInChildren<Transform>())
        {
            Destroy(prevCollider.gameObject);
        }
    }
    void CreateGroundCheckColliders()
    {
        DestroyGroundCheckColliders();
        Debug.Log("Creating Ground Check");
        for (int i = 0; i < groundCheckSides / 2; i++)
        {
            GameObject collider = new GameObject();
            collider.layer = 7;
            collider.name = "GroundCheckPart";
            collider.AddComponent<BoxCollider>().isTrigger = true;
            collider.AddComponent<Rigidbody>().useGravity = false;
            collider.AddComponent<GroundCheckPiece>();
            collider.transform.SetParent(GroundCheckParent);
            collider.transform.localPosition = new Vector3(0, 0, 0);
            float rotationY = (float)i * 180 / (float)groundCheckSides * 2;

            collider.transform.localRotation = Quaternion.Euler(0, rotationY, 0);
            collider.transform.localScale = new Vector3(groundCheckSideWidth, groundCheckThickness, groundCheckDiameter);
        }
    }
        #endregion
}