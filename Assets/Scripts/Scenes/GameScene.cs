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
public class GameScene : BaseScene
{

	[SerializeField] private PlayerController _player;
    public PlayerController Player => _player;

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
        SceneType = Scene.GameScene;   
	}

    private void Start()
    {
        _player.canMove = true;

        _worldType = Managers.World.CurrentWorldType;

        Managers.Resource.Instantiate($"Background/{_worldType.ToString()}World");
        Managers.Happy.AddHappiness(20f);

        // �뷱��
        if (Managers.Happy.Happiness < 40f && (Managers.World.CurrentWorldType == WorldType.Pelmanus
            || Managers.World.CurrentWorldType == WorldType.Gang))
        {
            Managers.Happy.Happiness = 40f;
        }

        StartCoroutine(GameStart());
    }

    private IEnumerator GameStart()
    {
        yield return _fadeImage.CoFadeIn(_fadeTime, waitAfter : _waitTimeAfterFade);

        // ������� ���
        switch (_worldType)
        {
            case WorldType.Vinter:
                Managers.Sound.Play("bgm_isekai_world_bin", Sound.Bgm);
                break;
            case WorldType.Chaumm:
                Managers.Sound.Play("bgm_isekai_world_choi", Sound.Bgm);
                break;
            case WorldType.Gang:
                Managers.Sound.Play("bgm_isekai_world_gang", Sound.Bgm);
                break;
            case WorldType.Pelmanus:
                Managers.Sound.Play("bgm_isekai_world_pel", Sound.Bgm);
                break;
            default:
                Managers.Sound.Play("bgm_isekai_world_bin", Sound.Bgm);
                break;
        }

        // �̴ϰ��� ����
        _miniGameFactory.OnGameEnd += GameOver;
        _miniGameFactory.Init();
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
			Vector3 newPosition = _player.transform.position + new Vector3(_potalSpawnOffsetX, _potalSpawnOffsetY, 0);
            go.transform.position = newPosition;

            Portal portal = go.GetComponent<Portal>();
            portal.SetPortalPosition(Scene.LoadingScene);
            portal.onEnterEvent += ClearEvent;

            Managers.Sound.Play("i_mini_portal1", Sound.Effect);
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
            case 2:
                AdjustVolume(0.05f, 0.1f, 0.3f);
                break;
            case 3:
                AdjustVolume(0.1f, 0.3f, 0.5f);
                break;
            case 4:
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
        _player.canMove = false;

        Managers.Sound.StopBGM();
        yield return WaitForSecondsCache.Get(1f);

        _fadeImage.gameObject.SetActive(true);
        _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 1);

        yield return WaitForSecondsCache.Get(4f);

        Managers.Scene.LoadScene(Scene.EndingScene);
    }

    private IEnumerator ClearEvent()
	{
        Managers.Sound.FadeOutBGM(3f);
        yield return _fadeImage.CoFadeOut(3f);
    }

    public override void Clear()
	{

	}
}
