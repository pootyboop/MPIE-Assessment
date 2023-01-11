using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    //script adapted from:
    //https://youtu.be/138WGOIgUeI

    public PlayerMovement mvmtScript;
    public float climbSpeed = 0.5f;

    private bool isClimbed = false;



    private void Start()
    {
        //mvmtScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
    }



    private void Update()
    {
        //if climbing the ladder
        if (isClimbed)
        {
            //jump to drop off
            if (Input.GetButtonDown("Jump"))
            {
                Use(false);
            }

            //W to move forward
            else if (Input.GetAxis("Vertical") > 0)
            {
                mvmtScript.transform.position += Vector3.up / climbSpeed * Time.deltaTime;
            }

            //S to go down
            else if (Input.GetAxis("Vertical") < 0)
            {
                mvmtScript.transform.position += Vector3.down / climbSpeed * Time.deltaTime;
            }
        }
    }



    //auto-climb when trigger is entered
    private void OnTriggerEnter(Collider other)
    {
        Use(true);
    }



    //auto-stop when trigger is exited
    private void OnTriggerExit(Collider other)
    {
        Use(false);
    }



    //setup stuff on player for climbing
    void Use(bool use)
    {
        if (use)
        {
            isClimbed = true;
            mvmtScript.useInput = false;
            //do not want the player gliding or crouching on a ladder
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
