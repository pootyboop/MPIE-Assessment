using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FINBook : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }



    void Update()
    {
        //print(transform.position);

        if (Input.GetKeyDown("t"))
        {
            animator.SetTrigger("closeBook");
        }

        if (Input.GetKeyDown("y"))
        {
            animator.SetTrigger("openBook");
        }
    }
}
