using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//destroy particles after they play
public class ParticleAutoDestroy : MonoBehaviour
{
    private ParticleSystem particles;



    private void Start()
    {
        particles = GetComponent<ParticleSystem>();
    }



    private void Update()
    {
        //destroy the particle system after its duration
        if (!particles.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
