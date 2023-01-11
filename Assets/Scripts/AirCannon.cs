using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirCannon : MonoBehaviour
{
    public float airCannonStrength = 1.0f;



    public Vector3 Boost()
    {
        return transform.forward.normalized * airCannonStrength;
    }
}
