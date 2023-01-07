using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;

public class FootstepManager : MonoBehaviour
{
    public enum groundType
    {
        CLOTH, GRASS, WOOD, WATER
    }

    public PlayerMovement player;
    private AudioSettings audioSettings;
    public AudioSource footstepPrefab;

    private bool isMovingGrounded = false;
    private float timer;

    private bool isCrouched;
    public float timeBtwnSteps = 0.3f;
    public float pitchRandomPercentage = 0.1f;
    public float crouchFootstepVolumeModifier = 0.5f;

    private groundType ground;
    public AudioClip[] cloth, grass, wood, water;



    private void Start()
    {
        audioSettings = FindObjectOfType<AudioSettings>();
        timer = timeBtwnSteps / 2.0f;
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

            if (isMovingGrounded)
            {

            }

            else
            {
                timer = timeBtwnSteps / 2.0f;
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

        if (isCrouched)
        {
            timeBtwnSteps *= 2.0f;
        }

        else {
            timeBtwnSteps /= 2.0f;
        }
    }



    public void Step()
    {
        AudioSource step = Instantiate(footstepPrefab);

        UpdateGround();

        //randomize footsteps so they aren´t as repetitive
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
        if (player.isInWater)
        {
            ground = groundType.WATER;
            return;
        }

        ground = GetFloorMat();
    }


    private groundType GetFloorMat()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, 2.0f))
        {
            //credit for these 2 lines:
            //https://forum.unity.com/threads/how-to-get-material-of-gameobject.1070414/
            Renderer renderer = hitInfo.transform.gameObject.GetComponent<Renderer>();

            if (renderer != null)
            {

                Material sharedMaterial = renderer.sharedMaterial;

                print(sharedMaterial.name);

                switch (sharedMaterial.name)
                {
                    case "M_GroundGrass":
                        return groundType.GRASS;
                    case "M_Bark":
                    case "Wood":
                    case "Brick":
                        return groundType.WOOD;
                    default:
                        return groundType.CLOTH;
                }
            }

            else
            {
                //if there´s no renderer, it´s most likely animated (probably one of the windmills)
                //can´t figure out how to get the renderer from the animator
                //so i just assume here that animated objects are made of wood
                return groundType.WOOD;
            }

        }

        else
        {
            return ground;
        }
    }



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
