using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BookPopup : UI_Popup
{
	[SerializeField] private Button _AnyClick;
	[SerializeField] private Image _selectImage;
	[SerializeField] private GameObject _bookClickArea;
	[SerializeField] private Animator _bookAnimator;
	[SerializeField] private Button _nextButton;

	[Header("0:Vinter, 1:Chaumm, 2:Gang, 3:Pelmanus")]
	[SerializeField] private List<Sprite> _icon;
	[SerializeField] private Image innerPotrait;

	private LibraryBook _book;
	private Color defaultColor;
	private GameObject _hud;

	[SerializeField] private GameObject _decisionHintBubble;
	[SerializeField] private TMP_Text _decisionHintText;
	[SerializeField] private CanvasGroup _canvasGroup;
	[SerializeField] private SimpleBookFlip _bookFlip;

	private const float _decisionFadeTime = 1f;

	private LibraryScene _libraryScene;
	private bool _isHovering;

	public GameObject NextButton => _nextButton.gameObject;
	public Image BookClickArea => _bookClickArea.GetComponent<Image>();

	private void Awake()
	{
		gameObject.GetComponent<Canvas>().worldCamera = Camera.main;
	}

	public override void Init()
	{
		base.Init();

		Debug.Log(_icon.Count);

		if (Managers.Scene.CurrentScene is LibraryScene)
		{
			_libraryScene = (LibraryScene)Managers.Scene.CurrentScene;

			_libraryScene.SetLightOff();
		}

		WorldType currentWorldType = Managers.World.CurrentWorldType;

		int bookIndex = (int)currentWorldType;
		LibraryBook book = _libraryScene.Books[bookIndex].GetComponent<LibraryBook>();
		book.SetHighlight(false);

		SetImage();

		innerPotrait.gameObject.SetActive(false);
		_bookFlip.BackPageObject.SetActive(false);

		_selectImage.gameObject.SetActive(false);
		_selectImage.gameObject.BindEvent(OnClickdecision);
		_selectImage.gameObject.BindEvent(OnPointerBookEnter, UIEvent.Enter);
		_selectImage.gameObject.BindEvent(OnPointerBookExit, UIEvent.Exit);
		_nextButton.onClick.AddListener(_bookFlip.FlipOnce);
		_bookClickArea.BindEvent(OnPlayAnimation);
		_bookFlip.Finished += EnableDecision;

		defaultColor = _selectImage.color;

		_decisionHintBubble.SetActive(false);
		_decisionHintText.text = "결정하면 아드리안의 세계로 향합니다.";
	}

	public void Init(LibraryBook book, GameObject hud)
	{
		Managers.Sound.Play("book", Sound.Effect);
		_book = book;
		_hud = hud;
	}

	public void ClosePopup()
	{
		if (_libraryScene != null)
		{
			_libraryScene.EnableBooks();
			_libraryScene.SetLightOn();
		}

		if (_book != null)
		{
			_book.EnableClick();
			_book.EnableFinger();
		}

		WorldType currentWorldType = Managers.World.CurrentWorldType;

		int bookIndex = (int)currentWorldType;
		LibraryBook book = _libraryScene.Books[bookIndex].GetComponent<LibraryBook>();
		book.SetHighlight(true);

		Managers.UI.ClosePopupUI(this);
		_hud?.SetActive(true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ClosePopup();
		}
	}

	private void SetImage()
	{
		WorldType currentWorldType = Managers.World.CurrentWorldType;

		switch (currentWorldType)
		{
			case WorldType.Vinter:
				Debug.Log(_icon.Count);
				innerPotrait.sprite = _icon[0];
				break;
			case WorldType.Chaumm:
				innerPotrait.sprite = _icon[1];
				break;
			case WorldType.Gang:
				innerPotrait.sprite = _icon[2];
				break;
			case WorldType.Pelmanus:
				innerPotrait.sprite = _icon[3];
				break;
		}
	}

	private void OnClickdecision(PointerEventData eventData)
	{
		StartCoroutine(CoDecisionFadeSequence());
	}

	public void OnPointerBookEnter(PointerEventData eventData)
	{
		if (_isHovering)
			return;

		if (eventData.pointerEnter == _selectImage.gameObject)
		{
			_isHovering = true;
			_selectImage.color = Color.gray;
			_decisionHintBubble.SetActive(true);
		}
	}

	public void OnPointerBookExit(PointerEventData eventData)
	{
		if (!_isHovering)
			return;

		Debug.Log(eventData.pointerEnter);
		if (eventData.pointerEnter == _selectImage.gameObject)
		{
			_isHovering = false;
			_selectImage.color = defaultColor;
			_decisionHintBubble.SetActive(false);
		}
	}

	private void OnPlayAnimation(PointerEventData eventData)
	{
		_bookAnimator.CrossFade("book_open", 0.1f);
	}

	private void EnableDecision()
	{
		_nextButton.gameObject.SetActive(false);
		_selectImage.gameObject.SetActive(true);
	}

	private IEnumerator CoDecisionFadeSequence()
	{
		// 중복 클릭 방지
		_selectImage.raycastTarget = false;

		Managers.Sound.FadeOutBGM(_decisionFadeTime);

		// UI Fade Out (1s)
		float t = 0f;
		while (t < _decisionFadeTime)
		{
			t += Time.deltaTime;
			_canvasGroup.alpha = 1f - (t / _decisionFadeTime);
			yield return null;
		}
		_canvasGroup.alpha = 0f;

		Managers.UI.ClosePopupUI(this);

		var ui = NoticePopupFactory.CreatePopup(Managers.World.CurrentWorldType);
		ui.Init(_book);
	}

	private void OnDestroy()
	{
		_bookFlip.Finished -= EnableDecision;
	}
}
