using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class UI_NoticePopup : UI_Popup
{
	[SerializeField] private PlayableDirector _playableDirector;
	[SerializeField] private Material[] _material;

	private MeshRenderer _meshRenderer;
	private LibraryBook _book;

    [SerializeField] private Toggle _checkToggle;
	[SerializeField] private Image _fadeImage;
	[SerializeField] private TMP_Text _gangText;
	[SerializeField] private RectTransform _background;
    [SerializeField] private TMP_Text _warningText;
    [SerializeField] private GameObject _noticeText;
    [SerializeField] private Sprite _changeSprite;
    [SerializeField] private Sprite _toggleSprite;

    private Canvas _canvas;

    private LibraryScene _libraryScene;
    
	private int _popupIndex = 0;
	private bool _canHandle = true;
    public override void Init()
	{
		base.Init();
		_playableDirector = GameObject.Find("TimeLineEnd").GetComponent<PlayableDirector>();
		_meshRenderer = GameObject.Find("Quad").GetComponent<MeshRenderer>();
		_meshRenderer.material = _material[0];
		if(_popupIndex == 0)
		{
            _libraryScene = Managers.Scene.CurrentScene as LibraryScene;
            _libraryScene.DisableBookSelect();
		}
        _checkToggle.onValueChanged.AddListener(CheckNotice);

        _canvas = GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = Camera.main;

        _lastPopup = this;
    }
	
	public void Init(LibraryBook book)
	{
		_book = book;
	}

	public void Init(int index, bool canHandle, Vector2 position)
	{
		_canHandle = canHandle;
		_popupIndex = index;
        _background.anchoredPosition = position;
	}

	private void Update()
    {
		if (!_canHandle) return;

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			_libraryScene.EnableBookSelect();
            _book.SetCanClicked();
            _book.StartFingerBlink();
            Managers.UI.ClosePopupUI(this);
        }
    }

    public void CheckNotice(bool isOn)
	{
		if (!isOn) return;

		switch (Managers.World.CurrentWorldType)
		{
			case WorldType.Pelmanus:
				if(_popupIndex == 2)
				{
                    LibraryScene scene = Managers.Scene.CurrentScene as LibraryScene;
                    MakeInfinityPopup(_background.anchoredPosition);
				}
				else
				{
                    UI_NoticePopup popup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
                    popup.Init(_popupIndex + 1, false, _background.anchoredPosition + new Vector2(_xOffset, -_yOffset));
                    this._canHandle = false;
                    SetActiveFalse();
                }
                break;
            case WorldType.Gang:
                GangrilSequence();
                break;
            default:
                _playableDirector.Play();
                Managers.UI.CloseAllPopupUI();
				Managers.Scene.LoadScene(Scene.GameScene);
                break;
        }
	}

    private void GangrilSequence()
    {
        _playableDirector.Play();
        // 1. ȭ���� ���� �������
        // 2. �ȭ�鿡 ������ 1�ʵڿ� ���ο� Noticeâ�� �����ȴ�
        // 3. Noticeâ�� ũ��� 0.7, 1.0, 1.3 ������ �Ǿ������� 3������ Noticeâ�� �����ȴ�
        // 3��°�� üũǥ�ÿ��� ���׸� ���迡 �����ϰ� �ȴ�.
        Sequence sequence = DOTween.Sequence();
        sequence.AppendInterval(2f);
        sequence.AppendCallback(() => _fadeImage.gameObject.SetActive(true));
        sequence.Append(_fadeImage.DOFade(1, 2f).SetEase(Ease.Linear));
        sequence.AppendInterval(1f);
        sequence.OnComplete(() =>
        {
            Managers.UI.ClosePopupUI(this);
            Managers.UI.MakeSubItem<UI_GangrilNotice>().Init(1);
        });
        sequence.Play();
    }

    private float minSpawnTime = 0.1f; // �ּ� ���� �ӵ�
    private float startSpawnTime = 1.0f; // ���� �ӵ�
    private float acceleration = 0.8f; // ���ӵ� (1���� ������ ���� ������)
    private float popupWidth, popupHeight;

    private float currentSpawnTime;
	private float spawnX, spawnY;
    private float screenWidth, screenHeight;
    private float leftTopX, leftTopY;

    private float _xOffset = 70;
    private float _yOffset = 50;

    private UI_NoticePopup _lastPopup;
    private void MakeInfinityPopup(Vector3 startPosition)
    {
        // 50 -50 �� �Ʒ��� �˾��� �������� �����ϴµ� ��ũ�� ������ �Ѿ�� ���� ������ �ٽ� ������ �Ʒ��� �����Ѵ�
        // ó������ õõ�� �����ߴٰ� ���� ������ �����ǰ� �������� ������ �ӵ��� ��� �����Ѵ�
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        spawnX = startPosition.x;
        spawnY = startPosition.y;

        currentSpawnTime = startSpawnTime;

        popupWidth = _background.GetComponent<RectTransform>().rect.width;
        popupHeight = _background.GetComponent<RectTransform>().rect.height;

        GetFirstPosition();

        StartCoroutine(InfinityPopupCoroutine());
    }

    /// <summary>
    /// �ʱ� ��ġ �ʱ�ȭ
    /// </summary>
    private void GetFirstPosition()
    {
        leftTopX = -popupWidth / 2;
        leftTopY = popupHeight / 2;

        while (leftTopX >= -(screenWidth / 2) && leftTopY <= screenHeight / 2)
        {
            leftTopX -= _xOffset;
            leftTopY += _yOffset;
        }

        leftTopX += _xOffset;
        leftTopY -= _yOffset;
        leftTopX += popupWidth / 2;
        leftTopY -= popupHeight / 2;
    }

    private IEnumerator InfinityPopupCoroutine()
    {
        int spawnCount = 0;
        int maxSpawnCount = 20;

        while (true)
        {
            LibraryScene scene = Managers.Scene.CurrentScene as LibraryScene;
            scene.StopColorConversion();
            scene.ColorConversion(Mathf.Max(currentSpawnTime * 0.5f, 0.15f));
            SpawnPopup();

            yield return new WaitForSeconds(currentSpawnTime);

            // ���� ���� (�ּ� �ӵ����� ũ�� ��� ����)
            if (currentSpawnTime > minSpawnTime)
            {
                currentSpawnTime *= acceleration;
                if (currentSpawnTime < minSpawnTime)
                    currentSpawnTime = minSpawnTime;
            }

            spawnCount++;
            if (spawnCount >= maxSpawnCount)
            {
                break;
            }
        }

        StartCoroutine(_lastPopup.BlackOutAndSetText());
    }

    public void SetActiveFalse()
    {
        _background.gameObject.GetComponent<Image>().sprite = _changeSprite;
        _noticeText.SetActive(false);
        _checkToggle.gameObject.SetActive(false);
    }
    private IEnumerator BlackOutAndSetText()
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(_warningText.CoTypingEffect("�ű⼭ ���� ����", 0.5f, true));
        yield return new WaitForSeconds(2f);

        Managers.Scene.LoadScene(Scene.GameScene);
    }

    /// <summary>
    /// �˾� ����
    /// Ư�� ��ġ�� ����
    /// </summary>
    private void SpawnPopup()
    {
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        UI_NoticePopup popup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
      
        // ���� ��ġ ���
        spawnX += _xOffset;
        spawnY -= _yOffset;
        if (spawnX + (popupWidth / 2) > screenWidth / 2 || spawnY + -(popupHeight / 2) > screenHeight / 2)
        {
            _lastPopup.ChangeSprite();

            _lastPopup = null;
            spawnX = leftTopX;
            spawnY = leftTopY;
        }

        popup.Init(_popupIndex + 1, false, new Vector3(spawnX, spawnY));

        if (_lastPopup != null)
        {
            _lastPopup.SetActiveFalse();
        }

        _lastPopup = popup;
    }

    private void ChangeSprite()
    {
        _checkToggle.onValueChanged.RemoveAllListeners();
        _checkToggle.isOn = true;
    }

    private void OnDestroy()
	{
		_meshRenderer.material = _material[1];
	}
}
