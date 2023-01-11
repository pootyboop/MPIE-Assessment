using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAutoDestroy : MonoBehaviour
{
    private AudioSource audioSource;



    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }



    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
