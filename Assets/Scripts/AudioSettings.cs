using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSettings : MonoBehaviour
{
    public float musicVolume = 0.15f;
    public float sfxVolume = 0.7f;

    public float musicFadeSpeed = 0.5f;

    //the 4 different tracks i collapsed the music into
    //1 - melody
    //2 - trill chords
    //3 - low chords & bass
    //4 - drums
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



    private void Update()
    {

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
