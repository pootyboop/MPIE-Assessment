using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

//manages footstep noises
public class FootstepManager : MonoBehaviour
{
    //materials are generalised into these 4 types
    //cloth is the default for anything that doesn't obviously fall under another category
    public enum groundType
    {
        CLOTH, GRASS, WOOD, WATER
    }

    public PlayerMovement player;
    private AudioSettings audioSettings;
    public AudioSource footstepPrefab;

    private bool isMovingGrounded = false;  //updated from PlayerMovement
    private float timer;                    //timer for footsteps

    public float timeBtwnSteps = 0.3f;
    public float pitchRandomPercentage = 0.1f;  //pitch randomness - does not affect random footstep sound
    private bool isCrouched;                    //so crouched footsteps are quieter and slower
    public float crouchFootstepVolumeModifier = 0.5f;

    private groundType ground;
    public AudioClip[] cloth, grass, wood, water;   //set in the GameObject. these arrays hold all the footstep AudioClips



    private void Start()
    {
        audioSettings = FindObjectOfType<AudioSettings>();
        timer = timeBtwnSteps / 2.0f;   //start halfway between footsteps. sounds nicer in my opinion
    }



    private void Update()
    {
        UpdateTimer();
    }



    private void UpdateTimer()
    {
        if (isMovingGrounded)
        {
            timer += Time.deltaTime;
            if (timer > timeBtwnSteps)
            {
                timer = 0.0f;
                Step();
            }
        }
    }



    public void SetMovingGrounded(bool newIsMovingGrounded)
    {
        if (newIsMovingGrounded != isMovingGrounded)
        {
            isMovingGrounded = newIsMovingGrounded;

            if (!isMovingGrounded)
            {
                timer = timeBtwnSteps / 2.0f;   //reset footstep time
            }
        }
    }



    public void SetCrouched(bool newCrouched)
    {
        if (isCrouched == newCrouched)
        {
            return;
        }

        isCrouched = newCrouched;

        //crouched footsteps are twice as slow
        if (isCrouched)
        {
            timeBtwnSteps *= 2.0f;
        }

        else {
            timeBtwnSteps /= 2.0f;
        }
    }



    //play the footstep. also called on landing from PlayerMovement
    public void Step()
    {
        AudioSource step = Instantiate(footstepPrefab);

        UpdateGround();

        //randomize footsteps so they aren't as repetitive
        step.clip = SelectFootstepClip();
        step.volume = audioSettings.sfxVolume - Random.Range(0.0f, audioSettings.sfxVolume * 0.2f);
        if (isCrouched) {
            step.volume *= crouchFootstepVolumeModifier;
        }

        step.pitch = Random.Range(1.0f - pitchRandomPercentage, 1.0f + pitchRandomPercentage);

        step.Play();

        if (ground == groundType.WATER)
        {
            player.LandingParticles();
        }
    }



    private void UpdateGround()
    {
        //water is easy since the player figures it out for us
        if (player.isInWater)
        {
            ground = groundType.WATER;
            return;
        }

        //otherwise, we're going over here
        ground = GetFloorMat();
    }


    private groundType GetFloorMat()
    {
        //raycast to see ground beneath our feet
        RaycastHit hitInfo;

        //if we're above anything
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, 2.0f))
        {
            //credit for this line...
            Renderer renderer = hitInfo.transform.gameObject.GetComponent<Renderer>();

            if (renderer != null)
            {
                ///...and this one:
                //https://forum.unity.com/threads/how-to-get-material-of-gameobject.1070414/
                //grab the material from the ground
                Material sharedMaterial = renderer.sharedMaterial;

                switch (sharedMaterial.name)
                {
                    //fairly easy. grass = grass
                    case "M_GroundGrass":
                        return groundType.GRASS;

                    //house and tree materials. assume this is wood
                    case "M_Bark":
                    case "Wood":
                    case "Brick":
                        return groundType.WOOD;

                    //otherwise, use cloth. it's the most generic footstep sound
                    default:
                        return groundType.CLOTH;
                }
            }

            else
            {
                //if there's no renderer, it's most likely grabbed an animator (probably one of the windmills)
                //can't figure out how to get the renderer from the animator
                //and we need the renderer to get its material
                //so i just assume here that animators are made of wood
                return groundType.WOOD;
            }

        }

        //didn't find a ground mesh so no need to change anything
        else
        {
            return ground;
        }
    }



    //just randomly return a footstep AudioClip depending on the groundType
    private AudioClip SelectFootstepClip()
    {
        switch (ground)
        {
            case groundType.GRASS:
                return GetRandomFootstep(grass);
            case groundType.WOOD:
                return GetRandomFootstep(wood);
            case groundType.WATER:
                return GetRandomFootstep(water);
            default:
                return GetRandomFootstep(cloth);

        }
    }



    private AudioClip GetRandomFootstep(AudioClip[] array)
    {
        return array[Random.Range(0, array.Length)];
    }
}
