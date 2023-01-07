using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public float camSpeed = 0.3f;
    private float camTimer = 0.0f;
    public float maxCamTime = 3.0f;
    private int index = 0;

    private bool isEnd = false;

    public Image fadeFromBlack;
    private float fadeTimer;
    public float fadeTime = 1.0f;

    public TMPro.TextMeshProUGUI title, pressStart;
    public GameObject[] activateOnStart, deactivateOnStart;
    public Camera[] cams;
    private Vector3[] camStarts;
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



    private void UpdateCams()
    {
        cams[index].transform.position += cams[index].transform.forward * camSpeed * Time.deltaTime;

        camTimer += Time.deltaTime;
        if (camTimer > maxCamTime)
        {
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



    private void UpdateFadeIn()
    {
        if (fadeTimer < fadeTime) {
            fadeTimer += Time.deltaTime;

            float alpha = fadeTimer / fadeTime;
            fadeFromBlack.color = new UnityEngine.Color(0.0f, 0.0f, 0.0f, Mathf.Lerp(1.0f, 0.0f, alpha));
        }
    }



    private void CheckInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isEnd)
            {
                Application.Quit();
            }

            else
            {
                CloseMenu();
            }
        }
    }



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

        fadeFromBlack.gameObject.SetActive(true);
        fadeTimer = 0.0f;

        //all tracks except drums
        audioSettings.SetTrackFade(0, true);
        audioSettings.SetTrackFade(1, true);
        audioSettings.SetTrackFade(2, true);
        audioSettings.SetTrackFade(3, false);

        cams[index].gameObject.SetActive(true);
    }



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
    }



    public void EndMenu()
    {
        //make sure to destroy most recent character UI
        Destroy(FindObjectOfType<CharacterUI>().gameObject);

        isEnd = true;
        title.text = ("The End");
        pressStart.text = ("(Press Space to quit)");
        //pressStart.gameObject.SetActive(false);

        OpenMenu();
    }
}