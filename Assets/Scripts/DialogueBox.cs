using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textBox;
    public TMPro.TextMeshProUGUI charNameTextBox;
    private PlayerMovement player;
    private CameraController cam;
    private Character speakingCharacter;    //only used if this is a character dialogue
    private AudioSettings audioSettings;

    private void Start()
    {
        //don't want player to move or look around while dialogue is open
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        player.useInput = false;

        cam = Camera.main.GetComponent<CameraController>();
        cam.useMouseInput = false;

        audioSettings = FindObjectOfType<AudioSettings>();

        audioSettings.SetTrackFade(0, false);
        audioSettings.SetTrackFade(1, false);
        audioSettings.SetTrackFade(2, true);
        audioSettings.SetTrackFade(3, true);
    }



    private void Update()
    {
        //jump button also manages dialogue
        if (Input.GetButtonDown("Jump"))
        {
            //give player back input
            player.useInput = true;
            cam.useMouseInput = true;

            //turn character UI back on
            if (speakingCharacter != null)
            {
                if (speakingCharacter.charUI != null)
                {
                    speakingCharacter.charUI.gameObject.SetActive(true);
                }
            }

            //grounded music
            audioSettings.SetTrackFade(0, false);
            audioSettings.SetTrackFade(1, true);
            audioSettings.SetTrackFade(2, true);
            audioSettings.SetTrackFade(3, true);
            
            player.OnDialogueClosed();

            Object.Destroy(gameObject);
        }
    }



    //default dialogue box
    public void SetContent(string text, Color color)
    {
        textBox.text = text;
        textBox.color = color;
    }



    //character dialogue override
    public void SetContent(string text, Color color, Character character)
    {
        textBox.text = text;
        textBox.color = color;

        //add the character's name above the dialogue box
        charNameTextBox.gameObject.SetActive(true);
        speakingCharacter = character;
        charNameTextBox.text = speakingCharacter.characterName;
        //charNameTextBox.color = color;

    }
}
