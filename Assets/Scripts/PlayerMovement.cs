using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody rb;
    CharacterController charController;
    public float moveSpeed = 5.0f;
    public float sprintMultiplier = 1.5f;
    public float crouchMultiplier = 0.5f;
    public float jumpHeight = 1.0f;
    public float gravity = 9.81f;

    private bool isGrounded;
    private float groundedTimer;
    private float verticalVelocity;



    void Start()
    {
        rb = GetComponent<Rigidbody>();
        charController = GetComponent<CharacterController>();
    }



    void Update()
    {
        Move();
    }



    void Move()
    {
        isGrounded = charController.isGrounded;
        if (isGrounded)
        {
            groundedTimer = 0.2f;
        }

        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
        }

        //https://youtu.be/7kGCrq1cJew
        //https://forum.unity.com/threads/how-to-correctly-setup-3d-character-movement-in-unity.981939/#post-6379746


        //X AND Z MOVEMENT
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeInput = forward * Input.GetAxis("Vertical");
        Vector3 rightRelativeInput = right * Input.GetAxis("Horizontal");

        Vector3 move = (forwardRelativeInput + rightRelativeInput) * moveSpeed;

        if (Input.GetButton("Crouch"))
        {
            move *= crouchMultiplier;
        }
        
        else if (Input.GetButton("Sprint"))
        {
            move *= sprintMultiplier;
        }


        //Y MOVEMENT
        //prevent bouncing
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }

        verticalVelocity -= gravity * Time.deltaTime;


        if (Input.GetButtonDown("Jump") && groundedTimer > 0)
        {
            groundedTimer = 0;

            verticalVelocity += Mathf.Sqrt(jumpHeight * 2 * gravity);
        }

        move.y = verticalVelocity;
        charController.Move(move * Time.deltaTime);
    }
}
