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

	[SerializeField] private DreamSystem _dreamSystem;

    protected override void Init()
	{
		SceneType = Scene.IntroScene;

		Managers.Sound.Play("bgm_real", Sound.Bgm);

		_playerController.gameObject.SetActive(false);
		_dreamSystem.gameObject.SetActive(true);
        StartCoroutine(_dreamSystem.StartSystem());
	}

	public override void Clear()
	{

	}
}
