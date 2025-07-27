using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

/// <summary>
/// �ʱ� ���� ����
/// </summary>
public class TitleScene : BaseScene
{
	protected override void Init()
	{ 
		SceneType = Scene.TitleScene;

		Managers.World.CurrentWorldType = WorldType.Vinter;
    }

	public override void Clear()
	{
		Debug.Log("TitleScene Clear!");
	}
}
