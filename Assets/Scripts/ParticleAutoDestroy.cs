using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAutoDestroy : MonoBehaviour
{
    private ParticleSystem particleSystem;

    void Start()
    {
        //destroy the particle system after its duration
        particleSystem = GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        if (!particleSystem.IsAlive())
        {
            Destroy(gameObject);
        }
    }
}
