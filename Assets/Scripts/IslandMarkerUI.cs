using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class IslandMarkerUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI text;
    private float timer = 0.0f;
    public float fadeInOutTime = 1.0f;
    public float stayTime = 2.0f;

    private bool fadingOut = false;



    private void Start()
    {
        //text.rectTransform.anchoredPosition = new Vector3(0, 230, 0);
    }



    private void Update()
    {
        timer += Time.deltaTime;

        float alpha = 0.0f; //just assigned to make visual studio happy

        if (!fadingOut)
        {
            //fading in
            if (timer < fadeInOutTime)
            {
                alpha = timer / fadeInOutTime;
            }

            //staying
            else if (timer < fadeInOutTime + stayTime)
            {
                alpha = 1.0f;
            }

            else
            {
                fadingOut = true;
                timer = 0.0f;
            }
        }

        if (fadingOut)
        {
            //fading out
            if (timer < fadeInOutTime)
            {
                alpha = 1.0f - timer / fadeInOutTime;
            }

            //dead
            else
            {
                timer = 0.0f;
                fadingOut = false;
                gameObject.SetActive(false);
            }
        }

        /*
        //fading out
        else if (timer < fadeInOutTime * 2.0f + stayTime + 0.1f)
        {
            alpha = 1.0f - ((timer + fadeInOutTime + stayTime) / (fadeInOutTime * 2.0f + stayTime));
        }

        //done
        else
        {
            Destroy(gameObject);
            alpha = 0.0f;    //this is just here to juke an error - it doesn't realize it'll never use this
        }
        */


        text.color = new Color(0.0f, 0.0f, 0.0f, alpha);
    }



    public void SetText(string newText)
    {
        text.text = newText;
        timer = 0.0f;
        fadingOut = false;
    }
}
