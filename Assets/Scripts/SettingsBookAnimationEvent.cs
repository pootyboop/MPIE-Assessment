using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//workaround script since i can't link an animation event to a parent gameobject :(
public class SettingsBookAnimationEvent : MonoBehaviour
{
    public Settings settings;
    public void AnimEventToSettings()
    {
        settings.OnBookAnimationEvent();
    }
}
