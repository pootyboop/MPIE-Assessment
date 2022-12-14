using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum animState
    {
        IDLE, LAY, SIT1, SIT2, TPOSE
    }

    public GameObject bookMesh;
    public bool hasBook = false;

    public string characterName = "Unnamed Character";
    public animState anim = animState.IDLE;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("Animation ID", (int)anim);
    }

    public bool TryGiveBook()
    {
        if (!hasBook)
        {
            hasBook = true;
            print("Gave " + characterName + " a book!");

            return true;
        }

        else
        {
            print(characterName + " already has a book!");

            return false;
        }
    }
}
