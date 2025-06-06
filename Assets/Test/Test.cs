using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Test : MonoBehaviour
{
    public string prevString = "";

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
