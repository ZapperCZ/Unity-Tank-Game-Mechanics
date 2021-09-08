using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] CharacterController Controller;
    [SerializeField] Transform GroundCheck;

    [SerializeField] LayerMask GroundMask;

    [Range(1, 100)]
    [SerializeField] int groundCheckPrecision = 10;
    [SerializeField] float groundDistance = 0.3f;
    [SerializeField] float defaultSpeed = 12f;
    [SerializeField] float sprintMultiplier = 1.6f;
    [SerializeField] float crouchMultiplier = 0.4f;
    [SerializeField] float airMultiplier = 0.4f;
    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpHeight = 3f;

    Vector3 velocity;
    float currentSpeed;
    bool isGrounded;

    void Start()
    {
        CreateGroundCheckColliders();
        currentSpeed = defaultSpeed;
        Debug.Log("Player Movement - Initialized");
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(GroundCheck.position, groundDistance, GroundMask);

        if(isGrounded && velocity.y < 0)    //TODO: Improve this by recreating the ground check from LP
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if(Input.GetButton("Sprint") && isGrounded)
        {
            //TODO: Maybe increase camera FOV slightly when sprinting
            currentSpeed = defaultSpeed * sprintMultiplier;
        }
        else
        {
            currentSpeed = defaultSpeed;
        }

        Vector3 Direction = transform.right*x + transform.forward*z;
        Controller.Move(Direction * currentSpeed * Time.deltaTime);

        //TODO: Slow down when not grounded
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            //currentSpeed = defaultSpeed;  Doesn't feel good and doesn't make much sense either
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        Controller.Move(velocity * Time.deltaTime);
    }
    void CreateGroundCheckColliders()
    {
        for(int i = 0; i < groundCheckPrecision; i++)
        {
            GameObject collider = new GameObject();
            collider.layer = 7; //Non-Static layer
            collider.name = "GroundCheckPart";
            collider.AddComponent<BoxCollider>().isTrigger = true;
            collider.transform.SetParent(GroundCheck);
            collider.transform.localPosition = new Vector3(0,0,0);
        }
    }
}
