using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//this class manages fading audio in and out
//as well as matching target volumes from AudioSettings
public class FadeSFX : MonoBehaviour
{
    public enum fadeState
    {
        SILENT, FADEIN, PLAYING, FADEOUT
    }

    private AudioSettings audioSettings;

    public AudioSource audioSource;         //only public for others to access, does not need to be set
    private fadeState state = fadeState.SILENT; //current audio state
    public bool isMusic = false;            //for music tracks, use musicVolume
    public bool isWorldNoise = false;       //will always play
    public float volumeMultiplier = 1.0f;   //how much to multiply volume by
    private float maxVolume;                //fade in to match this volume and never go above it
    public float fadeSpeed = 0.5f;          //how fast to fade in/out. if isMusic, this is overwritten by AudioSettings? music volume

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

        //world noise always plays (e.g. waterfalls, air cannons)
        if (isWorldNoise)
        {
            SetState(fadeState.PLAYING);
        }
    }



    private void Update()
    {
        //fade in
        if (state == fadeState.FADEIN)
        {
            //fade in by fadeSpeed amount
            audioSource.volume += fadeSpeed * Time.deltaTime;
            if (audioSource.volume >= maxVolume)
            {
                //stop fading
                SetState(fadeState.PLAYING);
            }
        }

        //fade out
        else if (state == fadeState.FADEOUT)
        {
            //fade out by fadeSpeed amount
            audioSource.volume -= fadeSpeed * Time.deltaTime;
            if (audioSource.volume <= 0.0f)
            {
                SetState(fadeState.SILENT);
            }
        }

        //this is messy, i know
        //i just can't think of a better way to change all sfx's volumes using this script
        //FindObjectsOfType() doesn't always work since i may instantiate more sfx at runtime and i don't wanna run it on Update()
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
            //mute sound but don't stop playing (useful for synced music tracs)
            case fadeState.SILENT:
                audioSource.volume = 0.0f;
                break;

            //immediately play at full volume
            case fadeState.PLAYING:
                audioSource.volume = maxVolume;
                break;
        }
    }



    //update this FadeSFX's maxVolume and set the audioSource's volume accordingly
    public void SetMaxVolume(float volume)
    {
        maxVolume = volume * volumeMultiplier;

        //i think this function gets called from AudioSettings before Start()
        //so this is a workaround to prevent unassigned references
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        //if the volume is too loud or if a currently playing sound's volume might be lower than it should be
        if ((audioSource.volume > maxVolume) || (state == fadeState.FADEIN || state == fadeState.PLAYING || !audioSource.isPlaying))
        {
            audioSource.volume = maxVolume;
        }
    }
}
