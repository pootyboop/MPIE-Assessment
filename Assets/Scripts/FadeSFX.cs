using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FadeSFX : MonoBehaviour
{
    public enum fadeState
    {
        SILENT, FADEIN, PLAYING, FADEOUT
    }

    private AudioSettings audioSettings;

    public AudioSource audioSource;
    private fadeState state = fadeState.SILENT;
    public bool isMusic = false;
    public bool isWorldNoise = false;
    public float volumeMultiplier = 1.0f;
    private float maxVolume;
    public float fadeSpeed = 0.5f;  //how fast to fade in/out. if isMusic, this is overwritten by AudioSettings´ music volume

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSettings = FindObjectOfType<AudioSettings>();

        if (isMusic)
        {
            maxVolume = audioSettings.musicVolume * volumeMultiplier;
            fadeSpeed = audioSettings.musicFadeSpeed;
        }

        else
        {
            maxVolume = audioSettings.sfxVolume * volumeMultiplier;
        }

        if (isWorldNoise)
        {
            SetState(fadeState.PLAYING);
        }
    }



    private void Update()
    {
        if (state == fadeState.FADEIN)
        {
            audioSource.volume += fadeSpeed * Time.deltaTime;
            if (audioSource.volume >= maxVolume)
            {
                SetState(fadeState.PLAYING);
            }
        }

        else if (state == fadeState.FADEOUT)
        {
            audioSource.volume -= fadeSpeed * Time.deltaTime;
            if (audioSource.volume <= 0.0f)
            {
                SetState(fadeState.SILENT);
            }
        }

        //this is messy, i know
        //i just can´t think of a better way to change all sfx´s volumes using this script
        //FindObjectsOfType() doesn´t always work since i may instantiate more sfx at runtime and i don´t wanna run it on Update()
        if (!isMusic)
        {
            SetMaxVolume(audioSettings.sfxVolume);
        }
    }



    public void SetState(fadeState newState)
    {
        state = newState;
        switch (state)
        {
            case fadeState.SILENT:
                audioSource.volume = 0.0f;
                break;

            case fadeState.PLAYING:
                audioSource.volume = maxVolume;
                /*

            case fadeState.FADEIN:
                if (!audioSource.isPlaying)
                {
                    audioSource.Play();
                }
                */
                break;
        }
    }



    public void SetMaxVolume(float volume)
    {
        maxVolume = volume * volumeMultiplier;

        //i think this function gets called from AudioSettings before Start()
        //so this is a workaround to prevent unassigned references
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        //if the volume´s too loud or if a currently playing sound´s volume might be lower than it should be
        if ((audioSource.volume > maxVolume) || (state == fadeState.FADEIN || state == fadeState.PLAYING || !audioSource.isPlaying))
        {
            audioSource.volume = maxVolume;
        }
    }
}
