using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject settingsBook;         //settings book (with the "Esc" bookmark)
    public GameObject bookCanvas;           //settings UI
    public GameObject booksRemainingIcon;   //this is hidden when settings are open
    public AudioSettings audioSettings;     //audioSettings takes care of volume sliders
    private Animator animator;
    private PlayerMovement playerMvmt;
    private CameraController cam;

    public TMPro.TextMeshProUGUI fullscreenToggle;
    public TMPro.TextMeshProUGUI glideToggle;
    public TMPro.TextMeshProUGUI crouchToggle;

    public FadeSFX openBookSound;
    public FadeSFX closeBookSound;

    private bool isOpen = false;            //is player in settings
    private bool previousUseInput;          //used to reset useInput to whatever it was before opening settings

    private CharacterUI charUI;


    void Start()
    {
        animator = settingsBook.GetComponent<Animator>();
        playerMvmt = gameObject.GetComponent<PlayerMovement>();
        cam = Camera.main.GetComponent<CameraController>();

        //animator.SetTrigger("closeBook");
    }



    void Update()
    {
        //open/close settings with Escape if the player isn't in dialogue
        if (Input.GetButtonDown("Cancel") && FindObjectOfType<DialogueBox>() == null)
        {
            if (!isOpen)
            {
                OpenSettings();
            }

            else if (isOpen)
            {
                CloseSettings();
            }
        }
    }



    void OpenSettings()
    {
        isOpen = true;
        //save current useInput value for later
        previousUseInput = playerMvmt.useInput;
        playerMvmt.useInput = false;

        //give the mouse cursor back and lock the camera
        cam.useMouseInput = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        //hide books left icon in bottom left
        booksRemainingIcon.SetActive(false);

        openBookSound.audioSource.Play();
        animator.SetTrigger("openBook");

        //hide charUI if one's nearby
        charUI = FindObjectOfType<CharacterUI>();
        if (charUI != null)
        {
            charUI.gameObject.SetActive(false);
        }
    }



    void CloseSettings()
    {
        isOpen = false;
        //put useInput back to its old value
        playerMvmt.useInput = previousUseInput;

        //hide mouse and give back camera control
        cam.useMouseInput = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //put back books left icon in bottom left
        booksRemainingIcon.SetActive(true);

        closeBookSound.audioSource.Play();
        animator.SetTrigger("closeBook");

        //unhide charUI if there was one
        if (charUI != null)
        {
            charUI.gameObject.SetActive(true);
            charUI = null;
        }
    }



    //make UI appear/disappear at a certain point in the animation to make it look as though it's inside the book
    public void OnBookAnimationEvent()
    {
        if (isOpen)
        {
            bookCanvas.SetActive(true);
        }

        else
        {
            bookCanvas.SetActive(false);
        }
    }



    //the rest of these functions are just to set the respective values from buttons and sliders
    public void SetSensitivity(float sensitivity)
    {
        cam.sensitivity = sensitivity;
    }



    public void SetMusicVolume(float volume)
    {
        audioSettings.SetMusicVolume(volume);
    }



    public void SetSfxVolume(float volume)
    {
        audioSettings.SetSFXVolume(volume);
    }



    public void ToggleFullscreen()
    {
        if (Screen.fullScreen)
        {
            Screen.fullScreen = false;
            fullscreenToggle.text = "Disabled";
        }

        else
        {
            Screen.fullScreen = true;
            fullscreenToggle.text = "Enabled";
        }
    }



    public void ToggleGlide()
    {
        if (playerMvmt.glideToggle)
        {
            playerMvmt.glideToggle = false;
            glideToggle.text = "Hold";
        }

        else
        {
            playerMvmt.glideToggle = true;
            glideToggle.text = "Toggle";
        }
    }



    public void ToggleCrouch()
    {
        if (playerMvmt.crouchToggle)
        {
            playerMvmt.crouchToggle = false;
            crouchToggle.text = "Hold";
        }

        else
        {
            playerMvmt.crouchToggle = true;
            crouchToggle.text = "Toggle";
        }
    }



    public void QuitGame()
    {
        Application.Quit();
    }
}
