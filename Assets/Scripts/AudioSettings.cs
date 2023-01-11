using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    public float musicVolume = 0.15f;
    public float sfxVolume = 0.7f;

    public float musicFadeSpeed = 0.5f; //time music takes to fade in/out

    //the 4 different tracks i collapsed the music into
    //0 - melody
    //1 - trill chords
    //2 - low chords & bass
    //3 - drums
    public FadeSFX[] music;



    private void Start()
    {
        SetMusicVolume(musicVolume);

        //start music
        SetTrackFade(0, true);
        SetTrackFade(1, true);
        SetTrackFade(2, true);
        //SetTrackFade(3, true);
    }



    public void SetTrackFade(int trackID, bool fadeIn)
    {
        if (fadeIn)
        {
            music[trackID].SetState(FadeSFX.fadeState.FADEIN);
        }

        else
        {
            music[trackID].SetState(FadeSFX.fadeState.FADEOUT);
        }
    }



    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
    }



    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;

        for (int i = 0; i < music.Length; i++)
        {
            music[i].SetMaxVolume(musicVolume);
        }
    }
}
