using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutSceneDirector : MonoBehaviour
{
	private void Start()
    {
		Managers.UI.ShowPopupUI<UI_CutScene1Popup>();
	}
}
