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
/// 1. ���� ���� ����
/// 2. ���� ���۽� �̺�Ʈ
///     �������
///     ���̵���
/// 3. ���� ����� �̺�Ʈ
///     ��Ż����
/// 4. postprocessing�� ���� ȿ�� ����  
/// 5. ���� ������ �̵��� �̺�Ʈ
/// </summary>
public class GameSceneEx : BaseScene
{

	public Transform player;

    // �̴ϰ����� �����Ѵ�
	[SerializeField] private MiniGameFactory _miniGameFactory;

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

    private WorldType _worldType;
    protected override void Init()
	{
		base.Init();
		SceneType = Scene.GameScene;

        _worldType = Managers.World.CurrentWorldType;

		Managers.Resource.Instantiate($"Background/{_worldType.ToString()}World");
        Managers.Happy.ChangeHappiness(20f);

        StartCoroutine(GameStart());
	}

    private IEnumerator GameStart()
    {
        yield return StartCoroutine(_fadeImage.CoFadeIn(_fadeTime, waitAfter : _waitTimeAfterFade));

        // ������� ���
        Managers.Sound.Play("anotherWorldBgm", Sound.Bgm);

        // �̴ϰ��� ����
        _miniGameFactory.Init();
        _miniGameFactory.OnGameEnd += GameOver;
    }

    /// <summary>
    /// �¸��� :
    ///     ���Ǽ���� �̵��ϴ� ��Ż ����
    /// �й�� :
    ///     ���ӿ��� UI ����
    /// </summary>
    public void GameOver(bool isWin)
	{
		if (isWin)
        { 
            if(Managers.World.CurrentWorldType == WorldType.Pelmanus)
            {
                StartCoroutine(EnterEndingScene());
                return;
            }

			GameObject go = Managers.Resource.Instantiate("Item/Portal");
			Vector3 newPosition = player.position + new Vector3(_potalSpawnOffsetX, _potalSpawnOffsetY, 0);
            go.transform.position = newPosition;

            Portal portal = go.GetComponent<Portal>();
            portal.SetPortalPosition(Scene.RealGameScene);
            portal.onEnterEvent += ClearEvent;
        }
		else
		{
            Managers.UI.ShowPopupUI<UI_GameOver>();
        }
	}

    /// <summary>
    /// ������ ���� postprocessing ȿ��
    /// </summary>
    /// <param name="strength">postprocessing ���� ����</param>
    // Film Grain, Vignette, Chromatic Aberration����
    public void SetPostProcessing(int strength)
    {
        
        switch (strength) 
        {
            case 4:
                AdjustVolume(0.05f, 0.1f, 0.3f);
                break;
            case 5:
                AdjustVolume(0.1f, 0.3f, 0.5f);
                break;
            case 6:
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
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 1);

        yield return WaitForSecondsCache.Get(2f);

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
