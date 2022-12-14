using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GliderRotation : MonoBehaviour
{
    public Transform obj;
    Vector3 rot;
    float prevRot;


    private void Start()
    {
        rot = transform.rotation.eulerAngles;
    }

    void LateUpdate()
    {
        /*
        prevRot = rot.y;
        rot.y = transform.eulerAngles.y;

        print(rot.y - prevRot);

        if (rot.y - prevRot > 0)
        {
            transform.eulerAngles = new Vector3(rot.x, -rot.y, rot.z);
        }
        
        transform.eulerAngles = rot;
        */

        //print(obj.rotation);
        //float xRot = obj.localRotation.x;
        //transform.localRotation = Quaternion.Euler(0f, transform.localRotation.y, transform.localRotation.z);
        //transform.LookAt(obj);

        //transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }
}
