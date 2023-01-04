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

    private AudioSource audioSource;
    private fadeState state = fadeState.SILENT;
    public bool isMusic = false;
    public bool isWorldNoise = false;
    private float maxVolume;
    public float fadeSpeed = 0.5f;  //how fast to fade in/out. if isMusic, this is overwritten by AudioSettings´ music volume

    private void Start()
    {
        audioSettings = FindObjectOfType<AudioSettings>();
        audioSource = GetComponent<AudioSource>();

        if (isMusic)
        {
            maxVolume = audioSettings.musicVolume;
            fadeSpeed = audioSettings.musicFadeSpeed;
        }

        else
        {
            maxVolume = audioSettings.sfxVolume;
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
        //FindObjectsOfType() doesn´t always work since i may instantiate more sfx at runtime
        if (state != fadeState.SILENT && !isMusic && audioSource.volume > audioSettings.sfxVolume)
        {
            audioSource.volume = audioSettings.sfxVolume;
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
                break;
        }
    }



    public void SetMaxVolume(float volume)
    {
        maxVolume = volume;

        if (audioSource.volume > maxVolume)
        {
            audioSource.volume = maxVolume;
        }
    }
}
