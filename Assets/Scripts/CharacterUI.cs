using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    //world space code stolen from:
    //https://youtu.be/7XVSLpo97k0

    public Transform lookAt;    //worldspace target
    public Vector3 offset;      //worldspace offset
    public TMPro.TextMeshProUGUI characterName;
    public TMPro.TextMeshProUGUI desc;

    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
    }



    private void Update()
    {
        //always stay at the target's location
        Vector3 pos = cam.WorldToScreenPoint(lookAt.position + offset);
        if (transform.position != pos)
        {
            transform.position = pos;
        }
    }



    public void SetInfo(string charName, Transform newLookAt)
    {
        characterName.text = charName;
        lookAt = newLookAt;
    }



    public void GaveBook()
    {
        //update UI so it doesn't always tell the player to give a book
        desc.text = "Has a book!";
        desc.color = new Color(0.4f, 0.4f, 0.4f);
    }
}
