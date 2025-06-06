using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// 도서관 씬 상의 책 관리
/// 도서관 씬에서의 타임라인 관리
/// 배경 관리
/// </summary>
public class LibraryScene : BaseScene
{
    [Header("Book")]
    [SerializeField] private GameObject _bookParent;
    [SerializeField] private GameObject[] _books;

    [Header("TimeLine")]
    [SerializeField] private PlayableDirector _startTimeLine;
    [SerializeField] private PlayableDirector _endTimeLine;
    public event Action onStartTimeLineEnd;
    public event Action onEndTimeLineEnd;

    [Header("Background")]
    [SerializeField] private MeshRenderer _background;
    [SerializeField] private Material _lightOn;
    [SerializeField] private Material _lightOff;

    [Header("PostProcessing")]
    [SerializeField] private Volume _volume;

    [SerializeField] private GameObject _HUDHappy;
    private ColorAdjustments _colorAdjustments;

    protected override void Init()
	{
        StartCoroutine(Managers.Sound.FadeInBGM("bgm_library_sketch_4", 3f));
        //Managers.Sound.Play("bgm_library_sketch_4", Sound.Bgm);
		
		SceneType = Scene.LibraryScene;

        // 도서관에서의 플레이어 이동속도 설정
		Managers.DB.SetPlayerData(
			new PlayerData  
			{
				moveSpeed = new List<float> { 1f, 1f, 1f }
			});

        foreach (var book in _books)
        {
            book.GetComponent<LibraryBook>().Init();
        }

        _startTimeLine.stopped += OnStartTimeLineEnd;
        _endTimeLine.stopped += OnEndTimeLineEnd;

        _volume.profile.TryGet(out _colorAdjustments);

        StartCoroutine(TempFakeFootSound());
    }

    private IEnumerator TempFakeFootSound()
    {
        yield return WaitForSecondsCache.Get(1f);
        float _footStepInterval = 0.63f;
        float curVolume = 0f;
        for(int i = 0; i < 10; i++) 
        {
            curVolume += 0.1f;
            Managers.Sound.Play("all_s_walk2", Sound.Effect, curVolume);
            yield return WaitForSecondsCache.Get(_footStepInterval);
        }
    }

    #region TimeLineMethod
    // 책 전부 밝혀주기
    private void OnStartTimeLineEnd(PlayableDirector director)
    {
        onStartTimeLineEnd?.Invoke();

        // 특정 책 만 켜주기
        BookSwitch();
    }

    private void OnEndTimeLineEnd(PlayableDirector director)
    {
        onEndTimeLineEnd?.Invoke();
    }

    public void PlayEndTimeLine()
    {
        _endTimeLine.Play();
    }
    #endregion

    #region BookMethod
    public void DisableBookSelect()
    {
        _bookParent.SetActive(false);
    } 
    public void EnableBookSelect()
    {
        _bookParent.SetActive(true);
    }
    //현재월드에 해당하는 책만 상호작용이 가능하게 한다
    private void BookSwitch()
    {
        EnableBookSelect();

        WorldType currentWorldType = Managers.World.CurrentWorldType;

        int bookIndex = (int)currentWorldType;
        LibraryBook book = _books[bookIndex].GetComponent<LibraryBook>();
        book.StartFingerBlink();
        book.SetCanClicked();
    }
    #endregion

    #region BackgroundMethod
    public void SetLightOn()
    {
        _background.material = _lightOn;
        EnableBookSelect();
    }

    public void SetLightOff()
    {
        _background.material = _lightOff;
        DisableBookSelect();
    }
    #endregion

    #region PostProcessing
    private Coroutine _colorConversionCoroutine;
    public void ColorConversion(float blinkTime)
    {
        _colorConversionCoroutine = StartCoroutine(CoColorConversion(blinkTime));
    }

    public void StopColorConversion()
    {
        if (_colorConversionCoroutine != null)
        {
            _colorAdjustments.colorFilter.value = originColor;
            StopCoroutine(_colorConversionCoroutine);
            _colorConversionCoroutine = null;
        }
    }

    private Color originColor = new Color(1f, 1f, 1f);
    private IEnumerator CoColorConversion(float blinkTime)
    {

        Color targetColor = new Color(140 / 255f, 0f, 0f);
        Color originColor = _colorAdjustments.colorFilter.value;

        _colorAdjustments.colorFilter.value = targetColor;
        yield return WaitForSecondsCache.Get(blinkTime);
        _colorAdjustments.colorFilter.value = originColor;
        yield return WaitForSecondsCache.Get(blinkTime);
    }

    public void DisableHUDHappy()
    {
        _HUDHappy.SetActive(false);
    }
    public override void Clear()
    {
    }
    #endregion
}
