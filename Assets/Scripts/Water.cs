using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    //private WaterAudioManager waterAudioManager;
    public FadeSFX waterSound;

    private void Start()
    {
        //waterAudioManager = FindObjectOfType<WaterAudioManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        /*
        if (other.CompareTag("Water Audio Manager"))
        {
            waterAudioManager.StartOverlapWater(gameObject);
        }
        */

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
