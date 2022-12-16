using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBox : MonoBehaviour
{
    public TMPro.TextMeshProUGUI textBox;
    private PlayerMovement player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        player.useInput = false;
    }



    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            player.useInput = true;
            Object.Destroy(gameObject);
        }
    }



    public void SetContent(string text, Color color)
    {
        textBox.text = text;
        textBox.color = color;
    }
}
