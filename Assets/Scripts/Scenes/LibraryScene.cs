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


    protected override void Init()
	{
		base.Init();

		SceneType = Scene.LibraryScene;

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

    #region BackgroundMethod
    public void SetLightOn()
    {
        _background.material = _lightOn;
    }

    public void SetLightOff()
    {
        _background.material = _lightOff;
    }
    #endregion
    public override void Clear()
	{
        // �÷��̾� �̵��ӵ� �ʱ�ȭ
		Managers.DB.ResetPlayerData();

        _startTimeLine.stopped -= OnStartTimeLineEnd;
        _endTimeLine.stopped -= OnEndTimeLineEnd;
    }
}
