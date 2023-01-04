using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //==================================================================VARIABLES==================================================================\\

    //references
    CharacterController charController;
    public Camera cam;
    public UIManager canvas;
    public DialogueBox dialogueGameObject;
    public GameObject gliderMesh;
    public GameObject gliderCloths;
    private Animator gliderAnim;
    public GameObject speedLines;
    private SpeedLines speedLinesScript;
    public GameObject clothCollision;   //collides with cloths (e.g. cloths on doors) since CharacterController doesn't have a referenceable capsule collider
                                        //probably could've done this with collision channels but whatever
    private CapsuleCollider clothCapsule;
    public GameObject landingParticles;
    private AudioSettings audioSettings;
    public FadeSFX windSound;

    //half height of the player collision capsule
    //cloth collision's height should always be halfHeight * 2 - 0.1 to prevent collision issues
    public const float halfHeight = 1.0f;

    //public movement
    public bool useInput = true;    //whether or not to accept player input
    public bool useGravity = true;  //whether or not to use gravity
    public bool glideToggle = true;     //whether glide controls are hold or toggle
    public bool crouchToggle = false;   //whether crouch controls are hold or toggle
    public float moveSpeed = 5.0f;
    public bool useSprint = false;  //enable/disable sprinting entirely (disabled by default)
    public float sprintSpeedMultiplier = 1.5f;
    public float crouchSpeedMultiplier = 0.5f;
    public float waterSpeedMultiplier = 0.8f;   //movement speed multiplier when semi-deep in water
    public float jumpHeight = 1.0f;
    public float baseGravity = 9.81f;   //default movement gravity
    public float glideGravity = 3.0f;   //gravity when gliding
    public float glideMaxVerticalVelocity = 1.5f;   //the cap of how fast the player can go up/down while gliding
    public float airCannonBoostStrength = 10.0f;    //strength of air cannon boosts
    public float airCannonMaxTime = 0.5f;           //how long air cannon blasts last after leaving the air cannon's trigger collision
    public float airCannonVerticalVelocityFalloffTime = 0.4f;   //how much vertical velocity returns at the end of a air cannon blast

    //private movement
    private bool isCrouching = false;
    private bool justStartedCrouching = false;
    private bool tryingToStand = false;
    private bool isGliding = false;
    private bool isGrounded;
    private float groundedTimer;
    private float verticalVelocity;
    private float previousVerticalVelocity;
    private float gravity;  //currently active gravity
    private Vector3 airCannonMove;  //the movement vector from air cannon
    private const float gliderTilt = 22.5f; //how much the glider mesh can tilt
    private float respawnY = -20.0f;    //the Y position past which the player gets respawned at respawnPosition
    private Vector3 respawnPosition;    //the player's last grounded position

    //water
    private bool isInWater;
    private List<GameObject> overlappedWaterPlanes; //all water planes currently overlapped by the player - used to ensure isInWater stays true when moving across water planes

    //air cannon
    private float airCannonTimer;   //the timer for lerping between player and air cannon movement after leaving the air cannon's trigger collision
    private GameObject overlappedAirCannon; //the air cannon the player is getting boosted by

    //characters
    private GameObject nearCharacter;   //the character the player is currently able to interact with
    private Character nearCharacterScript;   //character script
    public float lookDistance = 3.0f;   //how far away the player can interact with characters from
    private int booksLeft; //how many books the player has left to deliver

    //dialogue
    public Color dialogueColor;

    //=============================================================================================================================================\\



    void Start()
    {
        //init variables/references
        gliderAnim = gliderMesh.GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        speedLinesScript = speedLines.GetComponent<SpeedLines>();
        clothCapsule = clothCollision.GetComponent<CapsuleCollider>();
        gravity = baseGravity;

        audioSettings = FindObjectOfType<AudioSettings>();

        overlappedWaterPlanes = new List<GameObject>();

        booksLeft = GameObject.FindGameObjectsWithTag("Character").Length;
        canvas.SetBooksRemaining(booksLeft);

        dialogueColor = new Color(0.7f, 0.3f, 0.5f);

        DialogueBox dialogueBox = Instantiate(dialogueGameObject, canvas.gameObject.transform);
        dialogueBox.SetContent("Finally... my life's work is complete! Time to go deliver copies to everyone! They'll be so excited! I can't wait to see the looks on their faces!", dialogueColor);
    }



    void Update()
    {
        //GetLookedAtCharacter();
        UpdateInteractions();

        UpdateGrounded();

        if (useInput)
        {
            UpdateGliding();
            UpdateCrouch();
            Move();
        }


        //check if player fell to respawnY
        CheckIfFallen();

        previousVerticalVelocity = verticalVelocity;
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

                speedLines.SetActive(true);
                speedLinesScript.SetOpacity(1.0f);
                break;

            case "Character":
                nearCharacter = other.gameObject;
                nearCharacterScript = nearCharacter.GetComponent<Character>();
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

            case "Character":
                nearCharacter = null;
                nearCharacterScript = null;
                break;
        }
    }



    Vector3 AirCannonBoost(GameObject airCannon)
    {
        Vector3 finalBoost = airCannon.GetComponent<AirCannon>().Boost() * airCannonBoostStrength;
        //print(finalBoost);
        return (finalBoost);
    }



    void UpdateInteractions()
    {
        //player trying to interact?
        if (Input.GetButtonDown("Interact"))
        {
            if (nearCharacter != null)
            {
                if (nearCharacterScript.TryGiveBook())
                {
                    booksLeft--;
                    canvas.SetBooksRemaining(booksLeft);
                }
            }
        }
    }



    //deprecated
    void GetLookedAtCharacter()
    {
        RaycastHit hitInfo;
        bool raycastHit = Physics.Raycast(cam.transform.position, cam.transform.forward, out hitInfo, lookDistance);
        //Debug.DrawRay(cam.transform.position, cam.transform.forward * lookDistance, Color.white, 1.0f);

        if (raycastHit)
        {
            print("Looking at " + hitInfo.transform.gameObject);
            //if (hitInfo)
        }

        else
        {
            print("Looking at nothing");
        }
    }



    void UpdateGrounded()
    {
        if (!charController.isGrounded && isGrounded)
        {
            respawnPosition = transform.position;
        }

        else if (charController.isGrounded && !isGrounded && previousVerticalVelocity <= -glideMaxVerticalVelocity)
        {
            LandingParticles();
        }

        isGrounded = charController.isGrounded;
        if (isGrounded)
        {
            groundedTimer = 0.2f;

            TryGlideStop();

            audioSettings.SetTrackFade(0, false);
            audioSettings.SetTrackFade(2, true);
            audioSettings.SetTrackFade(3, true);
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



    void LandingParticles()
    {
        Vector3 landingParticlesSpawn = transform.position;
        landingParticlesSpawn.y -= halfHeight;
        Instantiate(landingParticles, landingParticlesSpawn, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
    }



    void UpdateGliding()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (!isGliding)
            {
                TryGlideStart();
            }

            else if (glideToggle)
            {
                TryGlideStop();
            }
        }

        else if (Input.GetButtonUp("Jump") && !glideToggle)
        {
            TryGlideStop();
        }
    }



    void TryGlideStart()
    {
        if (!isGliding && groundedTimer <= 0)
        {
            isGliding = true;
            gravity = glideGravity;

            audioSettings.SetTrackFade(0, true);
            audioSettings.SetTrackFade(2, false);
            audioSettings.SetTrackFade(3, false);
            windSound.SetState(FadeSFX.fadeState.FADEIN);

            //glide-y jumps are fun so this is disabled
            /*
            //prevent glide-y jumps when gliding while jumping upward (though they are fun)
            if (verticalVelocity > 0)
            {
                verticalVelocity = 0;
            }
            */

            //visual
            gliderCloths.SetActive(true);
            gliderAnim.SetBool("isGliding", true);
        }
    }



    public void TryGlideStop()
    {
        if (isGliding)
        {
            gravity = baseGravity;
            isGliding = false;
            windSound.SetState(FadeSFX.fadeState.FADEOUT);

            gliderAnim.SetBool("isGliding", false);
            gliderCloths.SetActive(false);
        }
    }



    void UpdateCrouch()
    {
        if (Input.GetButtonDown("Crouch"))
        {
            if (!crouchToggle || (crouchToggle && !isCrouching))
            {
                tryingToStand = false;
                TryCrouchStart();
            }

            else
            {
                tryingToStand = true;
                TryCrouchStop();
            }
        }

        else if ((Input.GetButtonUp("Crouch") && !crouchToggle) || tryingToStand)
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
            justStartedCrouching = true;
            charController.height = halfHeight;
            clothCapsule.height = halfHeight - 0.1f;
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
            transform.position = new Vector3(transform.position.x, transform.position.y + halfHeight, transform.position.z);
            charController.height = halfHeight * 2;
            clothCapsule.height = halfHeight * 2 - 0.1f;
        }
    }



    bool CanStand()
    {
        
        //check above the player's head (their height if they were standing)
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

        Vector3 move;

        //if (useInput)
        //{

        //X AND Z MOVEMENT
        move = GetDesiredMvmt();

        //UpdateGliderTilt();

        move = ApplySpeedModifiers(move);
        //}

        //else
        //{
        //move = new Vector3(0.0f, 0.0f, 0.0f);
        //}



        if (overlappedAirCannon == null && airCannonTimer > 0)
        {
            airCannonTimer -= Time.deltaTime;
        }

        float airCannonTimerAlpha = airCannonTimer / airCannonMaxTime;



        //if (useGravity)
        //{

        //Y MOVEMENT
        move.y = GetJumpHeight(airCannonTimerAlpha);

        //}


        UpdateSpeedLines(airCannonTimerAlpha);



        //lerp between normal and air cannon movement based on the timer alpha
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



    float GetJumpHeight(float airCannonTimerAlpha)
    {
        //immediately snap to ground when crouching
        if (justStartedCrouching == true)
        {
            justStartedCrouching = false;
            verticalVelocity = -100.0f;
        }



        //prevent the player from immediately falling supa dupa fast when air cannon boost ends
        else if (airCannonTimerAlpha > airCannonVerticalVelocityFalloffTime)
        {
            verticalVelocity = 0.0f;
        }



        //regular Y movement
        else
        {

            //prevent bouncing
            if (isGrounded && verticalVelocity < 0)
            {
                verticalVelocity = 0f;
            }

            verticalVelocity -= gravity * Time.deltaTime;


            if (Input.GetButtonDown("Jump") && groundedTimer > 0 && useInput)
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



    void UpdateSpeedLines(float airCannonTimerAlpha)
    {
        if (airCannonTimerAlpha > 0)
        {
            speedLinesScript.SetOpacity(airCannonTimerAlpha);
        }

        else if (speedLines.activeSelf)
        {
            speedLines.SetActive(false);
        }
    }



    void CheckIfFallen()
    {
        if (transform.position.y < respawnY)
        {
            transform.position = respawnPosition;

            //dialogue
            DialogueBox dialogueBox = Instantiate(dialogueGameObject, canvas.gameObject.transform);
            dialogueBox.SetContent("Oops! Better watch my step if I'm gonna deliver all these books on time!", dialogueColor);
        }
    }
}
