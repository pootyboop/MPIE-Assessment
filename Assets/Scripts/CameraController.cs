using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float sensitivity = 400.0f;
    Vector2 rot = new Vector2(0f, 0f);

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        //general concepts from:
        //https://youtu.be/f473C43s8nE
        //http://gyanendushekhar.com/2020/02/06/first-person-movement-in-unity-3d/

        Vector2 mousePos;

        mousePos.x = Input.GetAxisRaw("Mouse X") * Time.deltaTime * sensitivity;
        mousePos.y = Input.GetAxisRaw("Mouse Y") * Time.deltaTime * sensitivity;

        rot.y += mousePos.x;
        rot.x = Mathf.Clamp(rot.x - mousePos.y, -90f, 90f);

        //transform.eulerAngles = new Vector3(rot.x, rot.y, 0f);
        transform.rotation = Quaternion.Euler(rot.x, 0.0f, 0.0f);
        transform.parent.gameObject.transform.Rotate(new Vector3(0f, rot.y, 0f));
        //transform.parent.gameObject.transform.rotation = Quaternion.Euler(0f, rot.y, 0f);
    }
}
