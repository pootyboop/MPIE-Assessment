using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

//manages start menu cameras, UI, and starting the game
public class StartMenu : MonoBehaviour
{
    public float camSpeed = 0.3f;   //how fast cameras move forward
    private float camTimer = 0.0f;
    public float maxCamTime = 3.0f; //how long to spend on a single camera
    private int index = 0;          //current camera index

    private bool isEnd = false;     //is this the start or end screen

    public Image fadeFromBlack;     //the black image used for fading from black
    private float fadeTimer;
    public float fadeTime = 1.0f;   //time to fade from black

    public TMPro.TextMeshProUGUI title, pressStart;
    public GameObject[] activateOnStart, deactivateOnStart; //these objects will be activated exclusively inside or outside of the menu and end screen
    public Camera[] cams;           //cameras to swap between
    private Vector3[] camStarts;    //camera starting positions (to reset to on loop 2 and onward)
    private AudioSettings audioSettings;



    private void Start()
    {
        camStarts = new Vector3[cams.Length];
        camStarts[index] = cams[index].transform.position;

        audioSettings = FindObjectOfType<AudioSettings>();

        OpenMenu();
    }



    private void Update()
    {
        UpdateFadeIn();

        UpdateCams();

        CheckInput();
    }



    //fade from black
    private void UpdateFadeIn()
    {
        if (fadeTimer < fadeTime)
        {
            fadeTimer += Time.deltaTime;

            float alpha = fadeTimer / fadeTime;
            fadeFromBlack.color = new UnityEngine.Color(0.0f, 0.0f, 0.0f, Mathf.Lerp(1.0f, 0.0f, alpha));
        }
    }



    //update the panning cameras
    private void UpdateCams()
    {
        //move the camera forward
        cams[index].transform.position += cams[index].transform.forward * camSpeed * Time.deltaTime;

        camTimer += Time.deltaTime;

        if (camTimer > maxCamTime)
        {
            //increment camera index (switch cameras)
            camTimer = 0.0f;

            cams[index].gameObject.SetActive(false);

            index++;

            if (index == cams.Length)
            {
                index = 0;
            }

            cams[index].gameObject.SetActive(true);

            if (camStarts[index] == new Vector3(0,0,0))
            {
                camStarts[index] = cams[index].transform.position;
            }

            else
            {
                cams[index].transform.position = camStarts[index];
            }
        }
    }



    //start or quit the game when the player presses spacebar
    private void CheckInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            CloseMenu();
            /*
            if (isEnd)
            {
                Application.Quit();
            }

            else
            {
                CloseMenu();
            }
            */
        }
    }



    //set stuff up
    public void OpenMenu()
    {
        for (int i = 0; i < deactivateOnStart.Length; i++)
        {
            deactivateOnStart[i].SetActive(true);
        }

        for (int i = 0; i < activateOnStart.Length; i++)
        {
            activateOnStart[i].SetActive(false);
        }

        //fade from black
        fadeFromBlack.gameObject.SetActive(true);
        fadeTimer = 0.0f;

        //play all tracks except drums
        audioSettings.SetTrackFade(0, true);
        audioSettings.SetTrackFade(1, true);
        audioSettings.SetTrackFade(2, true);
        audioSettings.SetTrackFade(3, false);

        cams[index].gameObject.SetActive(true);
    }


    //shut stuff down
    public void CloseMenu()
    {
        fadeFromBlack.color = new UnityEngine.Color(0.0f, 0.0f, 0.0f, 1.0f);
        fadeFromBlack.gameObject.SetActive(false);

        for (int i = 0; i < activateOnStart.Length; i++)
        {
            activateOnStart[i].SetActive(true);
        }

        for (int i = 0; i < deactivateOnStart.Length; i++)
        {
            deactivateOnStart[i].SetActive(false);
        }

        index = 0;
        cams[index].gameObject.SetActive(false);
        gameObject.SetActive(false);

        Character[] characters = GameObject.FindObjectsOfType<Character>();
        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].ResetHasBook();
        }

        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().SetupPlayer();
    }



    //called on starting up the end menu
    public void EndMenu()
    {
        //make sure to destroy most recent character UI
        CharacterUI recentCharUI = GameObject.FindObjectOfType<CharacterUI>();
        if (recentCharUI != null)
        {
            Destroy(recentCharUI.gameObject);
        }

        /*a
        GameObject settingsCanvas = GameObject.FindGameObjectWithTag("SettingsCanvas");
        if (settingsCanvas != null)
        {
            settingsCanvas.SetActive(false);
        }
        */

        Settings settings = GameObject.FindObjectOfType<Settings>();
        if (settings.isOpen)
        {
            settings.CloseSettings();
        }

        isEnd = true;
        title.text = ("The End");
        pressStart.text = ("(Press Space to play again)");

        OpenMenu();
    }



    public void RestartGame()
    {
        EndMenu();
        CloseMenu();
    }
}