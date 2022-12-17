using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum animState
    {
        IDLE, LAY, SIT1, SIT2, TPOSE
    }

    public GameObject head;
    public GameObject bookMesh;
    public bool hasBook = false;
    public GameObject bookParticles;

    public string characterName = "Unnamed Character";
    public string dialogue = "Thanks!";
    public animState anim = animState.IDLE;
    private Animator animator;

    public Canvas UICanvas;
    public Canvas worldspaceCanvas;
    public DialogueBox characterDialogueGameObject;
    public CharacterUI characterUIGameObject;
    public CharacterUI charUI;



    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("Animation ID", (int)anim);
    }



    private void OnTriggerEnter(Collider other)
    {
        //show character UI when player is nearby
        if (other.gameObject.tag == "Player" && charUI == null)
        {
            SpawnCharacterUI();
        }
    }



    private void OnTriggerExit(Collider other)
    {
        //hide character UI when player isn't nearby
        if (other.gameObject.tag == "Player" && charUI != null)
        {
            Object.Destroy(charUI.gameObject);
        }
    }



    public void SpawnCharacterUI()
    {
        charUI = Instantiate(characterUIGameObject);
        charUI.transform.SetParent(worldspaceCanvas.transform);

        charUI.SetInfo(characterName, head.transform);

        if (hasBook)
        {
            charUI.GaveBook();
        }
    }



    public bool TryGiveBook()
    {
        if (!hasBook)
        {
            hasBook = true;

            GiveBook();

            return true;
        }

        else
        {
            return false;
        }
    }



    private void GiveBook()
    {
        bookMesh.SetActive(true);
        Instantiate(bookParticles, transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));

        //turn of charUI when dialogue is open
        charUI.GaveBook();
        charUI.gameObject.SetActive(false);

        //create dialogue
        DialogueBox dialogueBox = Instantiate(characterDialogueGameObject, UICanvas.transform);
        //dialogueBox.transform.SetParent(UICanvas.transform);
        dialogueBox.SetContent(dialogue, new Color(0.0f, 0.0f, 0.0f), this);
    }
}
