using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textBox;
    public TMPro.TextMeshProUGUI charNameTextBox;
    private PlayerMovement player;
    private CameraController cam;
    private Character speakingCharacter;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        player.useInput = false;

        cam = Camera.main.GetComponent<CameraController>();
        cam.useMouseInput = false;
    }



    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
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

            Object.Destroy(gameObject);
        }
    }



    //default dialogue box
    public void SetContent(string text, Color color)
    {
        textBox.text = text;
        textBox.color = color;
    }



    //character dialogue
    public void SetContent(string text, Color color, Character character)
    {
        textBox.text = text;
        textBox.color = color;

        charNameTextBox.gameObject.SetActive(true);
        speakingCharacter = character;
        charNameTextBox.text = speakingCharacter.characterName;
        //charNameTextBox.color = color;

    }
}
