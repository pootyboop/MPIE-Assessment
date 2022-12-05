using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //references
    CharacterController charController;
    public Camera cam;
    public GameObject gliderMesh;
    public GameObject clothCollision;   //collides with cloths (e.g. cloths on doors) since CharacterController doesn't have a referenceable capsule collider
    private CapsuleCollider clothCapsule;

    //half height of the player collision capsule
    //cloth collision should always be halfHeight * 2 - 0.1 to prevent collision issues
    public const float halfHeight = 1.0f;

    //public movement
    public bool useInput = true;
    public float moveSpeed = 5.0f;
    public bool useSprint = false;  //enable/disable sprinting entirely
    public float sprintSpeedMultiplier = 1.5f;
    public float crouchSpeedMultiplier = 0.5f;
    public float waterSpeedMultiplier = 0.8f;
    public float jumpHeight = 1.0f;
    public float baseGravity = 9.81f;
    public float glideGravity = 3.0f;
    public float glideMaxVerticalVelocity = 1.5f;   //the cap of how fast the player can go up/down while gliding
    public float airCannonBoostStrength = 10.0f;    //strength of air cannon boosts
    public float airCannonMaxTime = 0.5f;           //how long air cannon blasts last after leaving the air cannon's trigger collider
    public float airCannonVerticalVelocityFalloffTime = 0.4f;   //how much vertical velocity returns at the end of a air cannon blast

    //private movement state
    private bool isCrouching = false;
    private bool tryingToStand = false;
    private bool isGliding = false;
    private bool isGrounded;
    private float groundedTimer;
    private float verticalVelocity;
    private float gravity;  //currently active gravity
    private Vector3 airCannonMove;

    //private consts
    private const float gliderTilt = 22.5f;

    //water
    private bool isInWater;
    private List<GameObject> overlappedWaterPlanes; //all water planes currently overlapped by the player - used to ensure isInWater stays true when moving across water planes

    //air cannon
    private float airCannonTimer;
    private GameObject overlappedAirCannon;


    void Start()
    {
        charController = GetComponent<CharacterController>();
        clothCapsule = clothCollision.GetComponent<CapsuleCollider>();
        gravity = baseGravity;

        overlappedWaterPlanes = new List<GameObject>();
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



    private void FixedUpdate()
    {
        //UpdateGliderTilt();
    }



    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Water":
                overlappedWaterPlanes.Add(other.gameObject);
                isInWater = true;

                TryCrouchStop();

                break;

            case "Air Cannon":
                airCannonTimer = airCannonMaxTime;
                overlappedAirCannon = other.gameObject;
                airCannonMove = AirCannonBoost(overlappedAirCannon);
                break;
        }
    }



    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            case "Water":
                overlappedWaterPlanes.Remove(other.gameObject);
                if (overlappedWaterPlanes.Count == 0)
                {
                    isInWater = false;
                }

                break;

            case "Air Cannon":
                overlappedAirCannon = null;
                break;
        }
    }



    Vector3 AirCannonBoost(GameObject airCannon)
    {
        Vector3 finalBoost = airCannon.GetComponent<AirCannon>().Boost() * airCannonBoostStrength;
        //print(finalBoost);
        //rb.AddForce(finalBoost);
        return (finalBoost);
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
        if (Input.GetButtonDown("Jump"))
        {
            //toggle
            if (!isGliding)
            {
                TryGlideStart();
            }

            else
            {
                TryGlideStop();
            }
        }

        //hold
        /*
        else if (Input.GetButtonUp("Jump"))
        {
            TryGlideStop();
        }
        */
    }



    void TryGlideStart()
    {
        if (!isGliding && groundedTimer <= 0)
        {
            gravity = glideGravity;
            isGliding = true;
            gliderMesh.SetActive(true);
        }
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



    void UpdateCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            tryingToStand = false;
            TryCrouchStart();
        }

        else if (Input.GetButtonUp("Crouch") || tryingToStand)
        {
            tryingToStand = true;
            TryCrouchStop();
        }
    }



    void TryCrouchStart()
    {
        if (!isCrouching && isGrounded && !isInWater)
        {
            isCrouching = true;
            charController.height = halfHeight;
            clothCapsule.height = halfHeight - 0.1f;
            //cam.transform.position = new Vector3(transform.position.x, transform.position.y - halfHeight / 2, transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y - halfHeight, transform.position.z);
        }
    }



    public void TryCrouchStop()
    {
        bool canStand = CanStand();
        tryingToStand = !canStand;

        if (isCrouching && canStand)
        {
            isCrouching = false;
            //cam.transform.position = new Vector3(transform.position.x, transform.position.y + halfHeight / 2, transform.position.z);
            transform.position = new Vector3(transform.position.x, transform.position.y + halfHeight, transform.position.z);
            charController.height = halfHeight * 2;
            clothCapsule.height = halfHeight * 2 - 0.1f;
        }
    }



    bool CanStand()
    {
        
        if (Physics.Raycast(transform.position, charController.transform.up, halfHeight * 1.5f) == true)
        {
            return false;
        }
        
        return true;
    }



    void Move()
    {
        //credit for GetDesiredMvmt() and GetJumpHeight():
        //https://youtu.be/7kGCrq1cJew
        //https://forum.unity.com/threads/how-to-correctly-setup-3d-character-movement-in-unity.981939/#post-6379746

        //X AND Z MOVEMENT
        Vector3 move = GetDesiredMvmt();

        //UpdateGliderTilt();

        move = ApplySpeedModifiers(move);

        //Y MOVEMENT
        move.y = GetJumpHeight();



        if (overlappedAirCannon == null && airCannonTimer > 0)
        {
            airCannonTimer -= Time.deltaTime;
        }

        float airCannonTimerAlpha = airCannonTimer / airCannonMaxTime;
        Vector3 finalMove = Vector3.Lerp(move, airCannonMove, airCannonTimerAlpha);

        charController.Move(finalMove * Time.deltaTime);
    }



    Vector3 GetDesiredMvmt()
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;
        forward.y = 0;
        right.y = 0;
        forward = forward.normalized;
        right = right.normalized;

        Vector3 forwardRelativeInput = forward * Input.GetAxis("Vertical");
        Vector3 rightRelativeInput = right * Input.GetAxis("Horizontal");

        return (forwardRelativeInput + rightRelativeInput) * moveSpeed;
    }



    Vector3 ApplySpeedModifiers(Vector3 move)
    {
        if (isCrouching)
        {
            move *= crouchSpeedMultiplier;
        }

        //useSprint is disabled by default
        else if (Input.GetButton("Sprint") && useSprint)
        {
            move *= sprintSpeedMultiplier;
        }

        if (isInWater)
        {
            move *= waterSpeedMultiplier;
        }

        return move;
    }



    void UpdateGliderTilt()
    {
        if (isGliding)
        {
            float gliderY = gliderMesh.transform.rotation.y;
            Quaternion newRot = Quaternion.RotateTowards(gliderMesh.transform.rotation, Quaternion.LookRotation(charController.velocity.normalized), 360 * Time.fixedDeltaTime);
            gliderMesh.transform.rotation = Quaternion.Euler(newRot.x, gliderY, newRot.z);
            /*
            float horizontalInputAlpha = (Input.GetAxis("Horizontal") / 2) + 0.5f;
            float gliderY = Mathf.Lerp(-gliderTilt, gliderTilt, horizontalInputAlpha);

            gliderMesh.transform.localRotation = Quaternion.Euler(gliderMesh.transform.localRotation.x, gliderY, gliderMesh.transform.localRotation.z);
            */
        }
    }



    float GetJumpHeight()
    {
        //prevent the player from immediately falling supa dupa fast when air cannon boost ends
        if (airCannonTimer / airCannonMaxTime > airCannonVerticalVelocityFalloffTime)
        {
            verticalVelocity = 0.0f;
        }

        else
        {

            //prevent bouncing
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = 0f;
            }

            verticalVelocity -= gravity * Time.deltaTime;


            if (Input.GetButtonDown("Jump") && groundedTimer > 0)
            {
                TryCrouchStop();

                groundedTimer = 0;
                verticalVelocity += Mathf.Sqrt(jumpHeight * 2 * gravity);
            }

            if (isGliding)
            {
                verticalVelocity = Mathf.Clamp(verticalVelocity, -glideMaxVerticalVelocity, glideMaxVerticalVelocity);
            }
        }

        return verticalVelocity;
    }
}
