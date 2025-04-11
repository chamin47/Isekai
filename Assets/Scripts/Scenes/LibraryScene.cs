using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// ������ �� ���� å ����
/// ������ �������� Ÿ�Ӷ��� ����
/// ��� ����
/// </summary>
public class LibraryScene : BaseScene
{
    // å ����
    [SerializeField] private GameObject _bookParent;
    [SerializeField] private GameObject[] _books;

    // Ÿ�Ӷ��� ����
	[SerializeField] private PlayableDirector _startTimeLine;
    [SerializeField] private PlayableDirector _endTimeLine;
    public event Action onStartTimeLineEnd;
    public event Action onEndTimeLineEnd;

    // ��� ����
    // [SerializeField] private GameObject _background;
    // [SerializeField] private Material[] _backgroundMaterials;

    // �̵� ����
    [SerializeField] private Volume _volume;
    [SerializeField] private ColorAdjustments _noticePopupVolume;

    protected override void Init()
	{
		base.Init();

		SceneType = Scene.LibraryScene;

        _volume.profile.TryGet(out _noticePopupVolume);

        // ������������ �÷��̾� �̵��ӵ� ����
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
    }

    #region TimeLineMethod
    // å ���� �����ֱ�
    private void OnStartTimeLineEnd(PlayableDirector director)
    {
        onStartTimeLineEnd?.Invoke();

        // Ư�� å �� ���ֱ�
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
    //������忡 �ش��ϴ� å�� ��ȣ�ۿ��� �����ϰ� �Ѵ�
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

    public override void Clear()
	{
        // �÷��̾� �̵��ӵ� �ʱ�ȭ
		Managers.DB.ResetPlayerData();

        _startTimeLine.stopped -= OnStartTimeLineEnd;
        _endTimeLine.stopped -= OnEndTimeLineEnd;
    }


    /// <summary>
    /// Gangril ���忡�� noticepopup�� ������ ȿ�� �ֱ�
    /// </summary>
    private Coroutine _colorConversionCoroutine;
    public void ColorConversion(float blinkTime)
    {
        _colorConversionCoroutine =  StartCoroutine(CoColorConversion(blinkTime));
    }

    public void StopColorConversion()
    {
        if (_colorConversionCoroutine != null)
        {
            _noticePopupVolume.colorFilter.value = originColor;
            StopCoroutine(_colorConversionCoroutine);
            _colorConversionCoroutine = null;
        }
    }

    private Color originColor = new Color(1f, 1f, 1f);
    private IEnumerator CoColorConversion(float blinkTime)
    {

        Color targetColor = new Color(140/255f, 0f, 0f);
        Color originColor = _noticePopupVolume.colorFilter.value;
      
        _noticePopupVolume.colorFilter.value = targetColor;
        yield return WaitForSecondsCache.Get(blinkTime);
        _noticePopupVolume.colorFilter.value = originColor;
        yield return WaitForSecondsCache.Get(blinkTime);
    }
}
