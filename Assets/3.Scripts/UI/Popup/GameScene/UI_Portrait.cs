using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Portrait : UI_Popup
{
    public IEnumerator Disapear(float time)
    {
        yield return WaitForSecondsCache.Get(time);
        Destroy(gameObject);
    }
}
