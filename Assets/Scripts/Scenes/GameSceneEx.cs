using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;
using UnityEngine.UI;

/// <summary>
/// ���� ���۰� ���� ���� ���� ������ �����ϰ� �ִ� Ŭ����
/// </summary>
public class GameSceneEx : BaseScene
{
    [SerializeField] private WorldType _worldType = WorldType.Pelmanus;

	// �̸� �̿��ϴ� ���� ���Ŀ� ObjectManager�� ����ϸ� player�� �����ϰ� ����� ���� ���ڴ�.
	public static Transform player;

    // �̴ϰ����� �����Ѵ�
	[SerializeField] private MiniGameFactory _miniGameFactory;

	[SerializeField] private Transform _player;

    [Header("UI")]
	[SerializeField] private Image _fadeImage;

    [Header("���̵� �ð�")]
    [SerializeField] private float _fadeTime = 3f;
    [SerializeField] private float _waitTimeAfterFade = 0f;
    [SerializeField] private float _waitTimeBeforeFade = 0f;

    [Header("��Ż ��ġ")]
    [SerializeField] private float _potalSpawnOffsetX = 8f;
    [SerializeField] private float _potalSpawnOffsetY = -2.5f;

    [Header("����Ʈ ���μ���")]
    [SerializeField] private Volume _volume;
    protected override void Init()
	{
		base.Init();
		SceneType = Scene.GameScene;

        _worldType = Managers.World.CurrentWorldType;
        GameSceneEx.player = _player;

		Managers.Resource.Instantiate($"Background/{_worldType.ToString()}World");
        Managers.Happy.ChangeHappiness(20f);

        StartCoroutine(GameStart());
	}

    private IEnumerator GameStart()
    {
        yield return StartCoroutine(_fadeImage.CoFadeIn(_fadeTime, _waitTimeAfterFade));

        //������� ���
        Managers.Sound.Play("anotherWorldBgm", Sound.Bgm);

        _miniGameFactory.Init();
        _miniGameFactory.OnGameEnd += GameOver;
    }

    //���� �����
    public void GameOver(bool isWin)
	{
		if (isWin)
        { 
            if(Managers.World.CurrentWorldType == WorldType.Pelmanus)
            {
                StartCoroutine(EnterEndingScene());
                return;
            }

			// ���Ǽ���� �̵��ϴ� ��Ż�� �����ȴ�
			GameObject go = Managers.Resource.Instantiate("Item/Portal");
			Vector3 newPosition = _player.position + new Vector3(_potalSpawnOffsetX, _potalSpawnOffsetY, 0);
            go.transform.position = newPosition;

            Portal portal = go.GetComponent<Portal>();
            portal.SetPortalPosition(Scene.RealGameScene);
            portal.onEnterEvent += ClearEvent;
        }
		else
		{
            // �� ����� ��Ӱ� ó���ǰ�, ���ӿ���â�� ��������
            Managers.UI.ShowPopupUI<UI_GameOver>();
        }
	}

    //Film Grain, Vignette, Chromatic Aberration����
    public void SetPostProcessing(int strength)
    {
        
        switch (strength) 
        {
            case 1:
                AdjustVolume(0.05f, 0.1f, 0.3f);
                break;
            case 2:
                AdjustVolume(0.1f, 0.3f, 0.5f);
                break;
            case 3:
                AdjustVolume(0.15f, 0.4f, 0.7f);
                break;
        }

    }

    private void AdjustVolume(float filmIntensity, float vignetteIntensity, float chromaticAberrationIntensity)
    {
        _volume.gameObject.SetActive(true);
        if (_volume.profile.TryGet(out FilmGrain filmGrain))
        {
            filmGrain.intensity.Override(filmIntensity);
        }
        if (_volume.profile.TryGet(out Vignette vignette))
        {
            vignette.intensity.Override(vignetteIntensity);
        }
        if (_volume.profile.TryGet(out ChromaticAberration chromaticAberration))
        {
            chromaticAberration.intensity.Override(chromaticAberrationIntensity);
        }
    }

    private IEnumerator EnterEndingScene()
    {
        yield return StartCoroutine(_fadeImage.CoFadeOut(_fadeTime, _waitTimeAfterFade, _waitTimeBeforeFade));

        Managers.Scene.LoadScene(Scene.EndingScene);
    }

    private void ClearEvent()
	{
        StartCoroutine(_fadeImage.CoFadeOut(3f));
    }



    public override void Clear()
	{

	}
}
