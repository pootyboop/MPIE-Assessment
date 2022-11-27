using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    CharacterController charController;
    public Camera cam;
    public GameObject gliderMesh;
    public GameObject clothCollision;
    private CapsuleCollider clothCapsule;

    public const float halfHeight = 1.0f;

    public bool useInput = true;
    public float moveSpeed = 5.0f;
    public const float sprintMultiplier = 1.5f;
    public const float crouchMultiplier = 0.5f;
    public float jumpHeight = 1.0f;
    public float baseGravity = 9.81f;
    public float glideGravity = 3.0f;
    public float glideMaxVerticalVelocity = 1.5f;

    private bool isCrouching = false;
    private bool isGliding = false;
    private bool isGrounded;
    private float groundedTimer;
    private float verticalVelocity;
    private float gravity;



    void Start()
    {
        charController = GetComponent<CharacterController>();
        clothCapsule = clothCollision.GetComponent<CapsuleCollider>();
        gravity = baseGravity;
    }



    void Update()
    {
        UpdateGrounded();

        if (useInput)
        {
            UpdateGliding();
            UpdateCrouch();
            Move();
        }
    }



    void UpdateGrounded()
    {
        isGrounded = charController.isGrounded;
        if (isGrounded)
        {
            groundedTimer = 0.2f;

            TryGlideStop();
        }

        /*else if (isCrouching)
        {
            CrouchStop();
        }
        */

        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
        }
    }



    void UpdateGliding()
    {
        if (Input.GetButtonDown("Jump") && !isGliding && groundedTimer == 0)
        {
            GlideStart();
        }

        else if (Input.GetButtonUp("Jump") && isGliding)
        {
            GlideStop();
        }
    }



    void GlideStart()
    {
        gravity = glideGravity;
        isGliding = true;
        gliderMesh.SetActive(true);
    }



    public void TryGlideStop()
    {
        if (isGliding)
        {
            gravity = baseGravity;
            isGliding = false;
            gliderMesh.SetActive(false);
        }
    }



    void GlideStop()
    {
        gravity = baseGravity;
        isGliding = false;
        gliderMesh.SetActive(false);
    }



    void UpdateCrouch()
    {
        if (Input.GetButtonDown("Crouch") && !isCrouching && isGrounded)
        {
            CrouchStart();
        }

        else if (Input.GetButtonUp("Crouch") && isCrouching)
        {
            CrouchStop();
        }
    }



    void CrouchStart()
    {
        isCrouching = true;
        //cam.transform.position = new Vector3(transform.position.x, transform.position.y - halfHeight / 2, transform.position.z);
        transform.position = new Vector3(transform.position.x, transform.position.y - halfHeight, transform.position.z);
        charController.height = halfHeight;
        clothCapsule.height = halfHeight - 0.1f;
    }



    void CrouchStop()
    {
        isCrouching = false;
        //cam.transform.position = new Vector3(transform.position.x, transform.position.y + halfHeight / 2, transform.position.z);
        transform.position = new Vector3(transform.position.x, transform.position.y + halfHeight, transform.position.z);
        charController.height = halfHeight * 2;
        clothCapsule.height = halfHeight * 2 - 0.1f;
    }



    void Move()
    {
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

        if (isCrouching)
        {
            move *= crouchMultiplier;
        }
        
        else if (Input.GetButton("Sprint"))
        {
            move *= sprintMultiplier;
        }

        //Y MOVEMENT
        //prevent bouncing
        move.y = GetJumpHeight();
        charController.Move(move * Time.deltaTime);
    }



    float GetJumpHeight()
    {
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = 0f;
        }

        verticalVelocity -= gravity * Time.deltaTime;


        if (Input.GetButtonDown("Jump") && groundedTimer > 0)
        {
            CrouchStop();

            groundedTimer = 0;
            verticalVelocity += Mathf.Sqrt(jumpHeight * 2 * gravity);
        }

        if (isGliding)
        {
            verticalVelocity = Mathf.Clamp(verticalVelocity, -glideMaxVerticalVelocity, glideMaxVerticalVelocity);
        }

        return verticalVelocity;
    }
}
