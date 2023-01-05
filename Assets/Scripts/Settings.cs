using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour
{
    public GameObject settingsBook;
    public GameObject bookCanvas;
    public GameObject booksRemainingIcon;
    public AudioSettings audioSettings;
    private Animator animator;
    private PlayerMovement playerMvmt;
    private CameraController cam;

    public TMPro.TextMeshProUGUI fullscreenToggle;
    public TMPro.TextMeshProUGUI glideToggle;
    public TMPro.TextMeshProUGUI crouchToggle;

    public FadeSFX openBookSound;
    public FadeSFX closeBookSound;

    private bool isOpen = false;
    private bool previousUseInput;



    void Start()
    {
        animator = settingsBook.GetComponent<Animator>();
        playerMvmt = gameObject.GetComponent<PlayerMovement>();
        cam = Camera.main.GetComponent<CameraController>();

        //animator.SetTrigger("closeBook");
    }



    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
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
        previousUseInput = playerMvmt.useInput;
        playerMvmt.useInput = false;

        cam.useMouseInput = false;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;

        booksRemainingIcon.SetActive(false);

        openBookSound.audioSource.Play();

        animator.SetTrigger("openBook");
    }



    void CloseSettings()
    {
        isOpen = false;
        playerMvmt.useInput = previousUseInput;

        cam.useMouseInput = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        booksRemainingIcon.SetActive(true);

        closeBookSound.audioSource.Play();

        animator.SetTrigger("closeBook");
    }



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
}
