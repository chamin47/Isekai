using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{
    [ContextMenu("Test")]
    public void Test()
    {
        Managers.UI.MakeSubItem<UI_EnterBook>();
    }
}
