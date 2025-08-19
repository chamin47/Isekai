using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UI_CutScene3Popup : UI_Popup
{
	[SerializeField] private GameObject _portal;
	private PlayerController _playerController;

	public override void Init()
	{
        base.Init();

		_playerController = (Managers.Scene.CurrentScene as IntroScene).Player;

		Managers.Resource.Instantiate("Item/TrigerEnter");
		Managers.Sound.Play("s1_glitter2", Sound.Effect);

        _playerController.canMove = true;
	}

	public void ClosePopup()
	{
		Managers.UI.ClosePopupUI(this);
	}
}
