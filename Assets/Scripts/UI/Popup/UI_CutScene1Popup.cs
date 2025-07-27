using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_CutScene1Popup : UI_Popup
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
