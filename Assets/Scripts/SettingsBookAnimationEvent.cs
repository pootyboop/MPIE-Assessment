using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsBookAnimationEvent : MonoBehaviour
{
    //workaround script since i can't link an animation event to a parent gameobject :(
    public Settings settings;
    public void AnimEventToSettings()
    {
        settings.OnBookAnimationEvent();
    }
}
