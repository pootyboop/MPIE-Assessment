using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterAudioManager : MonoBehaviour
{
    private List<GameObject> overlappedWaterPlanes;
    public FadeSFX waterSound;

    //public GameObject player;

    private void Start()
    {
        overlappedWaterPlanes = new List<GameObject>();
    }



    private void Update()
    {
        //gameObject.transform.position = player.transform.position;
    }



    public void StartOverlapWater(GameObject other)
    {
        print("SUCCESS!!!!!!!!!!!");

        if (overlappedWaterPlanes.Count == 0)
        {
            waterSound.SetState(FadeSFX.fadeState.FADEIN);
        }

        overlappedWaterPlanes.Add(other.gameObject);
    }



    private void OnTriggerEnter(Collider other)
    {
        print("overlapped " + other);

        if (other.CompareTag("Water"))
        {
            print("SUCCESS!!!!!!!!!!!");

            if (overlappedWaterPlanes.Count == 0)
            {
                waterSound.SetState(FadeSFX.fadeState.FADEIN);
            }

            overlappedWaterPlanes.Add(other.gameObject);
        }
    }



    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            overlappedWaterPlanes.Remove(other.gameObject);
            if (overlappedWaterPlanes.Count == 0)
            {
                waterSound.SetState(FadeSFX.fadeState.FADEOUT);
            }
        }
    }
}
