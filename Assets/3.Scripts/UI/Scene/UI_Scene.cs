using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 씬 상에 하나만 존재하는 UI 영구적으로
/// </summary>
public class UI_Scene : UI_Base
{
	public override void Init()
	{
		Managers.UI.SetCanvas(gameObject, false);
	}
}
