using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroScene : BaseScene
{
	protected override void Init()
	{
		
		SceneType = Scene.IntroScene;
		//Managers.UI.ShowSceneUI<UI_GameScene>();
		Managers.Sound.Play("bgm_real_world", Sound.Bgm);
	}

	public override void Clear()
	{

	}
}
