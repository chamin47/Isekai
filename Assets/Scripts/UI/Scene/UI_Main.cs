using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ����ȭ�� UI
/// </summary>
public class UI_Main : UI_Scene
{
    [SerializeField] private float _fadeTime = 2f;
    [SerializeField] private float _waitTimeAfterFade = 1f;

    [SerializeField] private Image _fadeImage;
    [SerializeField] private Button _startButton;

    // ��� �̹���
    [SerializeField] private GameObject _endingBackGround;
    [SerializeField] private GameObject _commonBackGround;

    public override void Init()
    {
        base.Init();

        // ���� ������ ���� ��� �̹��� ����
        if (Managers.Game.IsShowEnding)
        {
			Managers.Sound.Play("tv_noise", Sound.Bgm);
			
            _endingBackGround.SetActive(true);
            _commonBackGround.SetActive(false);
        }
        else
        {
			Managers.Sound.Play("bgm_isekai_world_bin", Sound.Bgm);

			_endingBackGround.SetActive(false);
            _commonBackGround.SetActive(true);
        }

        _startButton.onClick.AddListener(ShowTitleUI);
    }

    protected override void Start()
    {
        base.Start();
    }

    private void ShowTitleUI()
    {
        StartCoroutine(CoShowSceneUI());
    }

    IEnumerator CoShowSceneUI()
    {
        _startButton.enabled = false;

        if (Managers.Game.IsShowEnding)
            _fadeImage.color = new Color(0, 0, 0, 0);

        Coroutine c1 = StartCoroutine(_fadeImage.CoFadeOut(_fadeTime, _waitTimeAfterFade));
        Coroutine c2 = StartCoroutine(Managers.Sound.FadeOutBGM(3f));

		yield return c1; yield return c2;

		if (!Managers.Game.IsShowEnding)
			Managers.UI.ShowSceneUI<UI_IntroAskName>();
		else
			Managers.UI.ShowSceneUI<UI_TrueEndingTitleScene>();

		this.gameObject.SetActive(false);
    }
}
