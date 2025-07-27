using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 시작화면 UI
/// </summary>
public class UI_Main : UI_Scene
{
    [SerializeField] private float _fadeTime = 2f;
    [SerializeField] private float _waitTimeAfterFade = 1f;

    [SerializeField] private Image _fadeImage;
    [SerializeField] private Button _startButton;

    // 배경 이미지
    [SerializeField] private GameObject _endingBackGround;
    [SerializeField] private GameObject _commonBackGround;

    public override void Init()
    {
        base.Init();

        // 엔딩 유무에 따른 배경 이미지 설정
        if (Managers.Game.IsShowEnding)
        {
            _endingBackGround.SetActive(true);
            _commonBackGround.SetActive(false);
        }
        else
        {
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

        yield return _fadeImage.CoFadeOut(_fadeTime, _waitTimeAfterFade);

        if (!Managers.Game.IsShowEnding)
            Managers.UI.ShowSceneUI<UI_TitleScene>();
        else
            Managers.UI.ShowSceneUI<UI_TrueEndingTitleScene>();

		this.gameObject.SetActive(false);
    }
}
