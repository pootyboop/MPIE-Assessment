using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAudioManager : MonoBehaviour
{
    private List<GameObject> overlappedWaterPlanes;
    public FadeSFX waterSound;

    private void Start()
    {
        overlappedWaterPlanes = new List<GameObject>();
    }



    private void Update()
    {
        
    }



    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Water"))
        {
            print("yup");

            if (overlappedWaterPlanes.Count == 0)
            {
                waterSound.SetState(FadeSFX.fadeState.FADEIN);
            }

            overlappedWaterPlanes.Add(other.gameObject);
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Water"))
        {
            overlappedWaterPlanes.Remove(other.gameObject);
            if (overlappedWaterPlanes.Count == 0)
            {
                waterSound.SetState(FadeSFX.fadeState.FADEOUT);
            }
        }
    }
}
