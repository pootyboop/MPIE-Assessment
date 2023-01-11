using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//manages speed lines' opacity
public class SpeedLines : MonoBehaviour
{
    public Material mat;
    public float maxLineOpacity = 0.5f;



    public void SetOpacity(float alpha)
    {
        alpha = Mathf.Clamp(alpha, 0.0f, maxLineOpacity);
        mat.SetColor("_BaseColor", new Color(1, 1, 1, alpha));
    }
}
