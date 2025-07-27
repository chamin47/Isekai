using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 1. 현재 씬 세팅
/// 2. BGM 재생
/// </summary>
public class IntroScene : BaseScene
{
	[SerializeField] private PlayerController _playerController;
	public PlayerController Player => _playerController;

    protected override void Init()
	{
		
		SceneType = Scene.IntroScene;

		Managers.Sound.Play("bgm_real_world", Sound.Bgm);
	}

	public override void Clear()
	{

	}
}
