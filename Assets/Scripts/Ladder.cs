using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour
{
    //script adapted from:
    //https://youtu.be/138WGOIgUeI

    public PlayerMovement mvmtScript;
    public GameObject player;
    public float climbSpeed = 12f;

    private bool isClimbed = false;



    void Start()
    {
        mvmtScript = player.GetComponent<PlayerMovement>();
    }



    private void Update()
    {
        if (isClimbed)
        {
            if (Input.GetButtonDown("Jump"))
            {
                use(false);
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
        use(true);
    }



    private void OnTriggerExit(Collider other)
    {
        use(false);
    }



    void use(bool use)
    {
        if (use)
        {
            isClimbed = true;
            mvmtScript.useInput = false;
            mvmtScript.TryGlideStop();

        }

        else
        {
            isClimbed = false;
            mvmtScript.useInput = true;
        }
    }
}
