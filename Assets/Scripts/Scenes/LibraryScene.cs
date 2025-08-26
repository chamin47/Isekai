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
/// 1. 도서관 씬 상의 책 관리
/// 2. 도서관 씬에서의 타임라인 관리
/// 3. 배경 색 관리 
/// </summary>
public class LibraryScene : BaseScene
{
    [SerializeField] private Transform _player;

    [Header("Book")]
    [SerializeField] private GameObject _bookParent;
    [SerializeField] private GameObject[] _books;

    [Header("TimeLine")]
    [SerializeField] private PlayableDirector _startTimeLine;
    [SerializeField] private PlayableDirector _endTimeLine;
    public event Action onStartTimeLineEnd;
    public event Action onEndTimeLineEnd;

    [Header("Background")]
    [SerializeField] private MeshRenderer[] _backgrounds;
    [SerializeField] private Material _lightOn;
    [SerializeField] private Material _lightOff;
    [SerializeField] private Image _fadeImage; 

    [Header("PostProcessing")]
    [SerializeField] private Volume _volume;
    private ColorAdjustments _colorAdjustments;

    [SerializeField] private GameObject _HUDHappy;

    private Color originColor = new Color(1f, 1f, 1f);

    protected override void Init()
	{
        Managers.DebugInfo.LastWorldType = Managers.World.CurrentWorldType;
        StartCoroutine(Managers.Sound.FadeInBGM("bgm_library_sketch_4", 3f));
		
		SceneType = Scene.LibraryScene;

        // 도서관에서의 플레이어 이동속도 설정
		Managers.DB.SetPlayerData(
			new PlayerData  
			{
				moveSpeed = new List<float> { 1f, 1f, 1f }
			});

        _startTimeLine.stopped += OnStartTimeLineEnd;
        _endTimeLine.stopped += OnEndTimeLineEnd;
        
        _volume.profile.TryGet(out _colorAdjustments);

        StartCoroutine(CoFakeFootSound());
    }

    private IEnumerator CoFakeFootSound()
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

        ActiveCurrentWorldBook();
    }

    private void OnEndTimeLineEnd(PlayableDirector director)
    {
        onEndTimeLineEnd?.Invoke();
    }

    private void AdjustTimelinePosition()
    {
        Vector3 startPos = _player.position;

        foreach (var track in _endTimeLine.playableAsset.outputs)
        {
            if (track.streamName == "PlayerPosition")
            {
                Debug.Log("Adjust Timeline Position");
                Debug.Log($"Start Position: {startPos}");
                AnimationTrack animationTrack = (AnimationTrack)track.sourceObject;
                animationTrack.position = startPos;
            }
        }
    }

    public void PlayEndTimeLine()
    {
        AdjustTimelinePosition();
        _endTimeLine.Play();
    }
    #endregion

    #region BookMethod
    public void DisableBooks()
    {
        _bookParent.SetActive(false);
    } 
    public void EnableBooks()
    {
        _bookParent.SetActive(true);
    }
    
    private void ActiveCurrentWorldBook()
    {
        EnableBooks();

        WorldType currentWorldType = Managers.World.CurrentWorldType;

        int bookIndex = (int)currentWorldType;
        LibraryBook book = _books[bookIndex].GetComponent<LibraryBook>();

        book.gameObject.SetActive(true);
        //book.StartFingerBlink();
        book.EnableFinger();
	
		book.EnableClick();
    }
    #endregion

    #region BackgroundMethod
    public void SetLightOn()
    {
        foreach (var _background in _backgrounds)
        {
            _background.material = _lightOn;
        }

        EnableBooks();
    }

    public void SetLightOff()
    {
        foreach (var _background in _backgrounds)
        {
            _background.material = _lightOff;
        }

        DisableBooks();
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

    private IEnumerator CoColorConversion(float blinkTime)
    {

        Color targetColor = new Color(140 / 255f, 0f, 0f);
        Color originColor = _colorAdjustments.colorFilter.value;

        _colorAdjustments.colorFilter.value = targetColor;
        yield return WaitForSecondsCache.Get(blinkTime);
        _colorAdjustments.colorFilter.value = originColor;
        yield return WaitForSecondsCache.Get(blinkTime);
    }

    public IEnumerator FadeScene(float duration)
    {
        _fadeImage.gameObject.SetActive(true);
        yield return _fadeImage.CoFadeOut(duration);
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
