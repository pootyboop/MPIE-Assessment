using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    //world space code stolen from:
    //https://youtu.be/7XVSLpo97k0

    public Transform lookAt;
    public Vector3 offset;
    public TMPro.TextMeshProUGUI characterName;
    public TMPro.TextMeshProUGUI desc;

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }



    void Update()
    {
        Vector3 pos = cam.WorldToScreenPoint(lookAt.position + offset);
        if (transform.position != pos)
        {
            transform.position = pos;
        }
    }



    public void SetInfo(string charName, Transform newLookAt)
    {
        characterName.text = charName;

        //Transform test = newLookAt;
        //test.position -= cam.transform.right;
        lookAt = newLookAt;
    }



    public void GaveBook()
    {
        desc.text = "Has a book!";
        desc.color = new Color(0.4f, 0.4f, 0.4f);
    }
}
