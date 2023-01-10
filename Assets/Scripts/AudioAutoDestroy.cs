using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioAutoDestroy : MonoBehaviour
{
    private AudioSource audioSource;
    void Start()
    {
        audioSource= GetComponent<AudioSource>();
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
