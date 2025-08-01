using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene2AnimationEvents : MonoBehaviour
{
	private UI_CutScene2Popup cutScene2Popup;

	private void Start()
	{
		cutScene2Popup = GetComponentInParent<UI_CutScene2Popup>();
	}

	public void OnCutScene2AnimationEnd()
	{
		Managers.Resource.Instantiate("CutScene/after cutscene Variant");
		cutScene2Popup.ClosePopupUI();
    }

	public void DoorSound()
	{
		Managers.Sound.Play("s1_glitter2", Sound.Effect);
	}
}
