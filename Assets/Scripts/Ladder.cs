using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    //script adapted from:
    //https://youtu.be/138WGOIgUeI

    private PlayerMovement mvmtScript;
    public float climbSpeed = 0.5f;

    private bool isClimbed = false;



    void Start()
    {
        mvmtScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }



    private void Update()
    {
        if (isClimbed)
        {
            if (Input.GetButtonDown("Jump"))
            {
                Use(false);
            }

            else if (Input.GetAxis("Vertical") > 0)
            {
                mvmtScript.transform.position += Vector3.up / climbSpeed * Time.deltaTime;
            }

            else if (Input.GetAxis("Vertical") < 0)
            {
                mvmtScript.transform.position += Vector3.down / climbSpeed * Time.deltaTime;
            }
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        Use(true);
    }



    private void OnTriggerExit(Collider other)
    {
        Use(false);
    }



    void Use(bool use)
    {
        if (use)
        {
            isClimbed = true;
            mvmtScript.useInput = false;
            mvmtScript.TryGlideStop();
            mvmtScript.TryCrouchStop();

        }

        else
        {
            isClimbed = false;
            mvmtScript.useInput = true;
        }
    }
}
