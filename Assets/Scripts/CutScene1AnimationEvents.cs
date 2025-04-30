using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene1AnimationEvents : MonoBehaviour
{
	private UI_CutScene1Popup cutScene1Popup;

	private void Start()
	{
		cutScene1Popup = GetComponentInParent<UI_CutScene1Popup>();
	}

	public void OnCutSceneAnimationStart()
	{
		Managers.Sound.Play("s1_wind", Sound.Effect);
	}

	public void OnReadLetter()
	{
		Managers.Sound.Play("s1_letter", Sound.Effect);
	}

	public void OnCutSceneAnimationEnd()
	{
		cutScene1Popup.ClosePopupUI();
		Managers.UI.ShowPopupUI<UI_LetterPopup>();
	}
}
