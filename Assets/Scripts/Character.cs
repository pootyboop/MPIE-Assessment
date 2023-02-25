using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    public enum animState
    {
        IDLE, LAY, SIT1, SIT2, TPOSE
    }

    public GameObject mapIcon;  //map icon displayed on map

    public GameObject head; //character UI attaches here
    public GameObject bookMesh; //mesh to appear in left hand when given a book
    public bool hasBook = false;
    public GameObject bookParticles;    //particles played on receiving a book

    public string characterName = "Unnamed Character";
    public string dialogue = "Thanks!";
    public animState anim = animState.IDLE; //animation to play
    private Animator animator;

    public Canvas UICanvas;
    public Canvas worldspaceCanvas;
    public DialogueBox characterDialogueGameObject;
    public CharacterUI characterUIGameObject;
    public CharacterUI charUI;

    private AudioSettings audioSettings;
    public AudioSource getBookSound;
    public AudioSource voiceSound;
    public float voiceVolume = 1.0f;    //voiceSound.volume will be multiplied by this and AudioSettings' sfxVolume



    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetInteger("Animation ID", (int)anim); //translate enum into animation ID

        audioSettings = FindObjectOfType<AudioSettings>();
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


        //different UI if the character already has a book
        if (hasBook)
        {
            charUI.GaveBook();
        }
    }



    public bool TryGiveBook()
    {
        if (!hasBook)
        {
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
        hasBook = true;

        //activate book mesh and spawn particles
        bookMesh.SetActive(true);
        Instantiate(bookParticles, transform.position, Quaternion.Euler(-90.0f, 0.0f, 0.0f));

        //play audio
        getBookSound.volume = audioSettings.sfxVolume;
        voiceSound.volume = audioSettings.sfxVolume * voiceVolume;
        getBookSound.Play();
        voiceSound.Play();

        //turn off charUI when dialogue is open
        charUI.GaveBook();
        charUI.gameObject.SetActive(false);

        //create dialogue
        DialogueBox dialogueBox = Instantiate(characterDialogueGameObject, UICanvas.transform);
        dialogueBox.SetContent(dialogue, new Color(0.0f, 0.0f, 0.0f), this);

        mapIcon.SetActive(false);
    }
}
