using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    private ParticleSystem particles;

    void Start()
    {
        //destroy the particle system after its duration
        particles = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!particles.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
