using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

//almost all player movement
//and a bunch of other misc stuff
public class PlayerMovement : MonoBehaviour
{

    //==================================================================VARIABLES==================================================================\\

    //references
    CharacterController charController;
    public Camera cam;
    public UIManager canvas;
    public DialogueBox dialogueGameObject;
    public StartMenu startMenu;
    public GameObject gliderMesh;
    public GameObject gliderCloths;
    private Animator gliderAnim;
    public GameObject speedLines;
    private SpeedLines speedLinesScript;
    public GameObject clothCollision;               //collides with cloths (e.g. cloths on doors) since CharacterController doesn't have a referenceable capsule collider
                                                    //probably could've done this with collision channels but whatever
    private CapsuleCollider clothCapsule;           //the actual collider
    public GameObject landingParticles;
    public GameObject map;
    public GameObject settingsBook;
    private AudioSettings audioSettings;
    public FootstepManager footstepManager;
    public FadeSFX windSound;
    public FadeSFX boostSound;
    public FadeSFX glideStartSound;
    public FadeSFX glideStopSound;
    public IslandMarkerUI islandMarkerUI;           //the UI to appear on landing on a new island
    private IslandMarker overlappedIslandMarker;    //currently overlapped island marker trigger
    private string lastIslandMarker = "Cozy Isle";  //most recently shown island title
                                                    //this title will not be shown until a different one is displayed (and thus saved here)
                                                    //set to Cozy Isle by default (the starter island) so the player isn't bombarded with UI when they start up    
    public float halfHeight = 1.0f;                 //half height of the player collision capsule
                                                    //cloth collision's height should always be halfHeight * 2 - 0.1 to prevent collision issues

    //public movement
    public bool useInput = true;                    //whether or not to accept player input
    public bool useGravity = true;                  //whether or not to use gravity
    public bool glideToggle = true;                 //whether glide controls are hold or toggle
    public bool crouchToggle = false;               //whether crouch controls are hold or toggle
    public float moveSpeed = 5.0f;
    public bool useSprint = false;                  //enable/disable sprinting entirely (disabled by default)
    public float sprintSpeedMultiplier = 1.5f;
    public float crouchSpeedMultiplier = 0.5f;
    public float waterSpeedMultiplier = 0.8f;       //movement speed multiplier when in water
    public float jumpHeight = 1.0f;
    public float baseGravity = 9.81f;               //default movement gravity
    public float glideGravity = 3.0f;               //gravity when gliding
    public float glideMaxVerticalVelocity = 1.5f;   //the cap of how fast the player can go up/down while gliding
    public float airCannonBoostStrength = 10.0f;    //strength of air cannon boosts
    public float airCannonMaxTime = 0.5f;           //how long air cannon blasts last after leaving the air cannon's trigger collision
    public float airCannonVerticalVelocityFalloffTime = 0.4f;   //how much vertical velocity returns at the end of a air cannon blast

    //private movement
    private bool isMoving = false;
    private bool isCrouching = false;
    private bool justStartedCrouching = false;
    private bool tryingToStand = false;
    private bool isGliding = false;
    private bool isGrounded;
    private float groundedTimer;
    private float verticalVelocity;
    private float previousVerticalVelocity;     //verticalVelocity from last Update()
    private float gravity;                      //currently active gravity
    private Vector3 airCannonMove;              //the movement vector from air cannon
    private const float gliderTilt = 22.5f;     //how much the glider mesh can tilt
    private float respawnY = -20.0f;            //the Y position past which the player gets respawned at respawnPosition
    private Vector3 respawnPosition;            //the player's last grounded position
    private bool isMap = false;                 //is the map open
    public bool hiddenBook = false;            //is the settings book hidden

    //water
    public bool isInWater;
    private List<GameObject> overlappedWaterPlanes; //all water planes currently overlapped by the player - used to ensure isInWater stays true when moving across water planes

    //air cannon
    private float airCannonTimer;               //the timer for lerping between player and air cannon movement after leaving the air cannon's trigger collision
    private GameObject overlappedAirCannon;     //the air cannon the player is getting boosted by

    //characters
    private GameObject nearCharacter;           //the character the player is currently able to interact with
    private Character nearCharacterScript;      //character script
    public float lookDistance = 3.0f;           //how far away the player can interact with characters from
    private int booksLeft;                      //how many books the player has left to deliver

    //dialogue
    public Color dialogueColor;                 //color of player dialogue text (NOT characters)
    private bool openedFinalDialogue = false;   //just used for ending once the player has delivered all books

    //=============================================================================================================================================\\



    void Start()
    {
        
    }



    public void SetupPlayer()
    {
        //init variables/references
        gliderAnim = gliderMesh.GetComponent<Animator>();
        charController = GetComponent<CharacterController>();
        speedLinesScript = speedLines.GetComponent<SpeedLines>();
        clothCapsule = clothCollision.GetComponent<CapsuleCollider>();
        gravity = baseGravity;

        audioSettings = FindObjectOfType<AudioSettings>();

        overlappedWaterPlanes = new List<GameObject>();

        //set up ladders
        GiveRefToObjects();

        //update booksLeft
        booksLeft = GameObject.FindGameObjectsWithTag("Character").Length;
        canvas.SetBooksRemaining(booksLeft);

        //align to spawn point
        Transform spawnPoint = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
        transform.position = spawnPoint.position;
        transform.rotation = spawnPoint.rotation;

        //reset starting stats
        lastIslandMarker = "Cozy Isle";
        openedFinalDialogue = false;
        cam.GetComponent<CameraController>().Setup();
        hiddenBook = false;
        isMap = false;

        //spawn-in dialogue
        DialogueBox dialogueBox = Instantiate(dialogueGameObject, canvas.gameObject.transform);
        dialogueBox.SetContent("Finally... my novel is complete! Time to go deliver copies to everyone! They'll be so excited! I can't wait to see the looks on their faces!", dialogueColor);
    }



    void Update()
    {
        //check if the player tried to interact with a character
        UpdateInteractions();

        UpdateGrounded();

        //doesn't run when in dialogue/settings or on a ladder
        if (useInput)
        {
            //check if the player should start/stop gliding and act accordingly
            UpdateGliding();

            //check if the player should start/stop crouching and act accordingly
            UpdateCrouch();

            //actual movement
            Move();
        }

        //otherwise input is disabled, so stop moving and disable footsteps
        else if (isMoving)
        {
            isMoving = false;
            footstepManager.SetMovingGrounded(false);
        }

        CheckMap();
        CheckHideBook();


        //check if player fell past respawnY height and respawn them if so
        CheckIfFallen();

        //this will be used next Update() for calculating the new verticalVelocity
        previousVerticalVelocity = verticalVelocity;
    }



    private void CheckMap()
    {
        if (Input.GetButtonDown("Map") && !isMap && useInput)
        {
            isMap = true;
            map.SetActive(true);

            Character[] characters = GameObject.FindObjectsOfType<Character>();

            for (int i = 0; i < characters.Length; i++)
            {
                if (characters[i].hasBook)
                {
                    characters[i].mapIcon.SetActive(false);
                }
            }
        }

        else if (Input.GetButtonDown("Map") && isMap && useInput)
        {
            isMap = false;
            map.SetActive(false);
        }
    }



    private void CheckHideBook()
    {
        if (Input.GetButtonDown("HideBook") && !hiddenBook && useInput)
        {
            hiddenBook = true;
            settingsBook.transform.localScale = new Vector3(0f, 0f, 0f);
        }

        else if (Input.GetButtonDown("HideBook") && hiddenBook && useInput)
        {
            hiddenBook = false;
            settingsBook.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        switch (other.gameObject.tag)
        {
            //entered water
            case "Water":
                overlappedWaterPlanes.Add(other.gameObject);
                isInWater = true;

                TryCrouchStop();

                break;

            //entered an air cannon's boost trigger
            case "Air Cannon":
                airCannonTimer = airCannonMaxTime;
                overlappedAirCannon = other.gameObject;
                airCannonMove = AirCannonBoost(overlappedAirCannon);

                MidairMusic();
                boostSound.audioSource.Play();

                speedLines.SetActive(true);
                speedLinesScript.SetOpacity(1.0f);
                break;

            //entered a character's interaction area
            case "Character":
                nearCharacter = other.gameObject;
                nearCharacterScript = nearCharacter.GetComponent<Character>();
                break;

            //entered the area around an island
            case "Island Marker":
                overlappedIslandMarker = other.gameObject.GetComponent<IslandMarker>();
                break;
        }
    }



    private void OnTriggerExit(Collider other)
    {
        switch (other.gameObject.tag)
        {
            //left water -- may still be in another water plane
            case "Water":
                overlappedWaterPlanes.Remove(other.gameObject);
                if (overlappedWaterPlanes.Count == 0)
                {
                    isInWater = false;
                }

                break;

            //left air cannon trigger -- done boosting
            case "Air Cannon":
                overlappedAirCannon = null;
                break;

            //left character's interaction radius
            case "Character":
                nearCharacter = null;
                nearCharacterScript = null;
                break;

            //left the area around an island
            case "Island Marker":
                overlappedIslandMarker = null;
                break;
        }
    }



    //figure out the Vector3 to put into the player's Move()
    Vector3 AirCannonBoost(GameObject airCannon)
    {
        Vector3 finalBoost = airCannon.GetComponent<AirCannon>().Boost() * airCannonBoostStrength;
        return (finalBoost);
    }



    void UpdateInteractions()
    {
        //player trying to interact?
        if (Input.GetButtonDown("Interact"))
        {
            if (nearCharacter != null)
            {
                //if the player can actually give the character the book
                if (nearCharacterScript.TryGiveBook())
                {
                    //update number of books left to give
                    booksLeft--;
                    canvas.SetBooksRemaining(booksLeft);
                }
            }
        }
    }



    //just pass a Player reference to ladders
    //this is so that the player can be set active whenever and ladders will still work
    private void GiveRefToObjects()
    {
        Ladder[] ladders = FindObjectsOfType<Ladder>();
        for (int i = 0; i < ladders.Length; i++)
        {
            ladders[i].mvmtScript = this;
        }
    }



    void UpdateGrounded()
    {
        //charController knows isGrounded before this script does
        //so when they disagree, we can use it for events for leaving the ground and landing

        if (!charController.isGrounded && isGrounded)
        {
            OnLeftGround();
        }

        else if (charController.isGrounded && !isGrounded)
        {
            OnLanded();
        }

        //now that the events are done they should be equal
        isGrounded = charController.isGrounded;


        if (isGrounded)
        {
            //this timer is for coyote time for gliding and jumping
            //mainly helps with slopes so the character walks naturally downhill
            groundedTimer = 0.2f;
        }

        if (groundedTimer > 0)
        {
            groundedTimer -= Time.deltaTime;
        }
    }



    private void OnLeftGround()
    {
        respawnPosition = transform.position;
        footstepManager.SetMovingGrounded(false);
    }



    private void OnLanded()
    {
        //stop gliding
        TryGlideStop();
        //play grounded music
        GroundedMusic();
        //and check if the player should see an island title popup
        TryIslandMarker();

        //also turn on footsteps if the player is moving
        if (isMoving)
        {
            footstepManager.SetMovingGrounded(true);
        }

        //if landing hard, play particles and a footstep sound
        if (previousVerticalVelocity <= -glideMaxVerticalVelocity)
        {
            LandingParticles();
            footstepManager.Step();
        }
    }



    public void LandingParticles()
    {
        Vector3 landingParticlesSpawn = transform.position;
        landingParticlesSpawn.y -= halfHeight;  //place them at the character's feet. crouching is never an option when this function is called so halfHeight will always be the same
        Instantiate(landingParticles, landingParticlesSpawn, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
    }



    private void TryIslandMarker()
    {
        //if on an island the player hasn't already just seen the title for
        if (overlappedIslandMarker != null && lastIslandMarker != overlappedIslandMarker.text)
        {
            islandMarkerUI.gameObject.SetActive(true);
            islandMarkerUI.SetText(overlappedIslandMarker.text);

            //update lastIslandMarker so we can check against it
            //the player can now go back to the previous island and will see its title again
            lastIslandMarker = overlappedIslandMarker.text;
        }
    }



    void UpdateGliding()
    {
        if (Input.GetButtonDown("Jump"))
        {
            //start gliding if not
            if (!isGliding)
            {
                TryGlideStart();
            }

            //but if glide is currently active and set to toggle (on by default), stop gliding
            else if (glideToggle)
            {
                TryGlideStop();
            }
        }

        else if (Input.GetButtonUp("Jump") && !glideToggle)
        {
            //stop gliding if the button is released and glide mode is Hold
            TryGlideStop();
        }
    }



    void TryGlideStart()
    {
        //groundedTimer here prevents the player from immediately gliding
        //which can be annoying when you jump and immediately glide if spamming spacebar
        if (!isGliding && groundedTimer <= 0)
        {
            isGliding = true;
            gravity = glideGravity;

            //glide audio
            MidairMusic();
            windSound.SetState(FadeSFX.fadeState.FADEIN);
            glideStartSound.audioSource.Play();

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

            //audio
            windSound.SetState(FadeSFX.fadeState.FADEOUT);
            glideStopSound.audioSource.Play();

            gliderAnim.SetBool("isGliding", false);
            gliderCloths.SetActive(false);  //these are hidden when glider isn't active to prevent them from getting in the player's face
                                            //also a little less cloth sim for unity to worry about
        }
    }



    void UpdateCrouch()
    {
        //same system as gliding for toggle vs hold
        //except this time there's an additional check "tryingToStand"
        //this is further explained in CanStand()

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

            //resize colliders
            charController.height = halfHeight;
            clothCapsule.height = halfHeight - 0.1f;

            //snap player to the floor AFTER resizing colliders
            transform.position = new Vector3(transform.position.x, transform.position.y - halfHeight, transform.position.z);

            //tone down footsteps while crouching
            footstepManager.SetCrouched(true);
        }
    }



    public void TryCrouchStop()
    {
        bool canStand = CanStand();
        tryingToStand = !canStand;

        if (isCrouching && canStand)
        {
            isCrouching = false;

            //move player back up BEFORE resizing colliders
            transform.position = new Vector3(transform.position.x, transform.position.y + halfHeight, transform.position.z);

            //resize colliders
            charController.height = halfHeight * 2;
            clothCapsule.height = halfHeight * 2 - 0.1f; //-0.1f to prevent clothCapsule from screwing with regular player collision

            //return footsteps to normal
            footstepManager.SetCrouched(false);
        }
    }



    //i prevent the player from uncrouching into an object by constantly checking above them when they want to uncrouch
    //if anything's inside the area their head would be, they can't uncrouch
    //once they're in the clear they automatically uncrouch unless they started intentionally crouching again
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

        //X AND Z MOVEMENT
        move = GetDesiredMvmt();

        move = ApplySpeedModifiers(move);



        //if player just left air cannon trigger, tick down the timer
        if (overlappedAirCannon == null && airCannonTimer > 0)
        {
            airCannonTimer -= Time.deltaTime;
        }

        //used to lerp between air cannon boost and normal movement states
        float airCannonTimerAlpha = airCannonTimer / airCannonMaxTime;

        //Y MOVEMENT
        move.y = GetJumpHeight(airCannonTimerAlpha);

        //speed lines shown when air canon boosting
        UpdateSpeedLines(airCannonTimerAlpha);



        //lerp between normal and air cannon movement based on the timer alpha
        Vector3 finalMove = Vector3.Lerp(move, airCannonMove, airCannonTimerAlpha);

        //and finally, actually move
        charController.Move(finalMove * Time.deltaTime);
    }



    Vector3 GetDesiredMvmt()
    {
        //get the forward and right vectors
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        //just take the x value
        forward.y = 0.0f;
        right.y = 0.0f;

        //normalize
        forward = forward.normalized;
        right = right.normalized;

        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");

        UpdateIsMoving(horizontal, vertical);

        //multiply normalized vectors by player movement input (meaning the character moves relative to its rotation)
        Vector3 forwardRelativeInput = forward * vertical;
        Vector3 rightRelativeInput = right * horizontal;

        //and when all that's done, multiply by moveSpeed
        return (forwardRelativeInput + rightRelativeInput) * moveSpeed;
    }



    //TLDR: lets the footstep manager know if we're moving
    void UpdateIsMoving (float horizontal, float vertical)
    {
        if (!isMoving && (vertical != 0.0f || horizontal != 0.0f))
        {
            isMoving = true;

            if (isGrounded)
            {
                footstepManager.SetMovingGrounded(true);
            }
        }

        else if (isMoving && vertical == 0.0f && horizontal == 0.0f)
        {

            isMoving = false;
            footstepManager.SetMovingGrounded(false);
        }
    }



    //fairly straightforward, just take the modifiers into account
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

            //take gravity into account
            verticalVelocity -= gravity * Time.deltaTime;


            //if player is allowed to jump
            if (Input.GetButtonDown("Jump") && groundedTimer > 0 && useInput)
            {
                TryCrouchStop();

                groundedTimer = 0;

                //jump
                verticalVelocity += Mathf.Sqrt(jumpHeight * 2 * gravity);
            }

            //verticalVelocity is clamped when gliding so that you actually glide
            if (isGliding)
            {
                verticalVelocity = Mathf.Clamp(verticalVelocity, -glideMaxVerticalVelocity, glideMaxVerticalVelocity);
            }
        }

        return verticalVelocity;
    }



    //fades speed lines out when leaving an air cannon trigger
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
        //if player fell off the map
        if (transform.position.y < respawnY)
        {
            //respawn
            transform.position = respawnPosition;

            //cheeky respawn dialogue
            DialogueBox dialogueBox = Instantiate(dialogueGameObject, canvas.gameObject.transform);
            dialogueBox.SetContent("Oops! Better watch my step if I'm gonna deliver all these books!", dialogueColor);
        }
    }



    public void OnDialogueClosed()
    {
        //end of game stuff
        if (booksLeft == 0)
        {
            if (!openedFinalDialogue)
            {
                //final dialogue

                DialogueBox dialogueBox = Instantiate(dialogueGameObject, canvas.gameObject.transform);
                dialogueBox.SetContent("They're all delivered. I can't wait to read the reviews! Well then, I suppose it's time to start a new book!", dialogueColor);
                openedFinalDialogue = true;
            }

            else
            {
                //end the game
                startMenu.gameObject.SetActive(true);
                startMenu.EndMenu();
            }


        }
    }



    //music that plays while gliding or air cannon boosting
    private void MidairMusic()
    {
        //melody, trill chords
        audioSettings.SetTrackFade(0, true);
        audioSettings.SetTrackFade(1, true);
        audioSettings.SetTrackFade(2, false);
        audioSettings.SetTrackFade(3, false);
    }



    //music that plays when on the ground
    private void GroundedMusic()
    {
        //trill chords, low chords and bass, drums
        audioSettings.SetTrackFade(0, false);
        audioSettings.SetTrackFade(2, true);
        audioSettings.SetTrackFade(3, true);
    }
}
