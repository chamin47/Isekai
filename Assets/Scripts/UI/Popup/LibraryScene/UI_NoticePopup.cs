using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UI_NoticePopup : UI_Popup
{
    [SerializeField] protected Toggle _checkToggle;
    [SerializeField] protected Image _fadeImage;
    [SerializeField] protected RectTransform _backgroundParent;
    [SerializeField] protected TMP_Text _noticeText;

    protected Canvas _canvas;
    protected LibraryScene _libraryScene;
    protected LibraryBook _book;

    protected int _popupIndex = 0;
    protected bool _canHandle = true;
    protected Vector2 _position;

    public override void Init()
    {
        base.Init();
        _libraryScene = Managers.Scene.CurrentScene as LibraryScene;

        _libraryScene.DisableBookSelect();
        _libraryScene.SetLightOff();        

        _checkToggle.onValueChanged.AddListener(OnCheckToggleIsOn);

        // postprocessing ȿ���� ���� ī�޶� ����
        _canvas = GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = Camera.main;
    }

    public virtual void Init(LibraryBook book)
    {
        _book = book;
    }

    public virtual void Init(int index, bool canHandle, Vector2 position)
    {
        _canHandle = canHandle;
        _popupIndex = index;
        _position = position;
        _backgroundParent.anchoredPosition = position;
    }

    protected virtual void Update()
    {
        if (!_canHandle) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    protected virtual void ClosePopup()
    {
        _libraryScene.EnableBookSelect();
        _libraryScene.SetLightOn();
        if (_book != null)
        {
            _book.SetCanClicked();
            _book.StartFingerBlink();
        }
        Managers.UI.ClosePopupUI(this);
    }

    public virtual void OnCheckToggleIsOn(bool isOn)
    {
        if (!isOn) return;

        // �� ���� Ÿ�Ժ� ó���� ���� Ŭ�������� ����
        ProcessWorldInteraction();
    }

    protected virtual void ProcessWorldInteraction()
    {
        _libraryScene.PlayEndTimeLine();
        StartCoroutine(CoFadeOut());
    }

    private IEnumerator CoFadeOut()
    {
        _fadeImage.gameObject.SetActive(true);
        StartCoroutine(_fadeImage.CoFadeOut(2f));
        yield return WaitForSecondsCache.Get(1f);
        Managers.Sound.Play("s2_book1", Sound.Effect);

        yield return WaitForSecondsCache.Get(2f);
        Managers.UI.CloseAllPopupUI();
        Managers.Scene.LoadScene(Scene.LoadingScene);
    }

    public void SetActiveFalse()
    {
        _noticeText.gameObject.SetActive(false);
        _checkToggle.gameObject.SetActive(false);
    }
}
