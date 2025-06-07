using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
public class Test : MonoBehaviour
{
    public string prevString = "";
    public TextMeshProUGUI _warningText;
    private void Start()
    {
        StartCoroutine(_warningText.CoTypingEffectPerChar("거기서 당장 나", 0.5f, true, "getout_short", true));
        
        
    }
    private void Update()
    {
        if(Input.compositionString != prevString)
        {
            if(!string.IsNullOrEmpty(Input.compositionString))
            {   
                Managers.Sound.Play("intro_type_short", Sound.Effect);
                prevString = Input.compositionString;
            }
        }
    }


}
