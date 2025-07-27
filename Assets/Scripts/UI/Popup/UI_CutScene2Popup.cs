using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_CutScene2Popup : UI_Popup
{
	public override void Init()
	{
		base.Init();
	}

	public void ClosePopup()
	{
		Managers.UI.ClosePopupUI(this);
	}
}
