using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSoundEvent : MonoBehaviour
{
    [SerializeField] private string _soundName;
    
    public void PlaySound()
    {
        Managers.Sound.Play(_soundName, Sound.Effect);
    }
}
