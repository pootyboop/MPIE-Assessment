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
        //float xRot = obj.localRotation.x; //map(obj.rotation.x, -90f, 90f, -1f, 1f);
        //transform.localRotation = Quaternion.Euler(0f, transform.localRotation.y, transform.localRotation.z);
        //transform.LookAt(obj);

        //transform.rotation = Quaternion.Euler(-90f, 0f, 0f);
    }



    float map(float s, float a1, float a2, float b1, float b2)
    {
        return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
    }
}
