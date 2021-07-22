using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    public CharacterController Controller;
    public Transform GroundCheck;

    public LayerMask GroundMask;

    public float groundDistance = 0.3f;
    public float defaultSpeed = 12f;
    public float sprintMultiplier = 2f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;

    Vector3 velocity;
    float currentSpeed;
    bool isGrounded;

    void Start()
    {
        currentSpeed = defaultSpeed;
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

        Vector3 Direction = transform.right*x + transform.forward*z;
        Controller.Move(Direction * currentSpeed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        Controller.Move(velocity * Time.deltaTime);
    }
}
