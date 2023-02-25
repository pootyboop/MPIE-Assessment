using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//just plays water noises around the player to make the audio more realistic
public class Water : MonoBehaviour
{
    public FadeSFX waterSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            waterSound.audioSource.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            waterSound.audioSource.Stop();
        }
    }
}
