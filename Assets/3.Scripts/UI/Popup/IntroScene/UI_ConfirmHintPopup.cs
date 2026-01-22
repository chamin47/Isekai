using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ConfirmHintPopup : UI_Popup
{
	private enum PopupState
	{
		Closed,
		Opening,
		Opened,
		Closing
	}

	private PopupState _state = PopupState.Closed;

	[Header("Texts")]
	[SerializeField] private TMP_Text _questionText;
	[SerializeField] private TMP_Text _yesText;
	[SerializeField] private TMP_Text _noText;

	[Header("FX")]
	[SerializeField] private Image _fadeImage;
	[SerializeField] private Image _popupOverlay;
	[SerializeField] private GameObject _infoImage;

	[SerializeField] private RectTransform _root;

	[SerializeField] Button _closeButton;

	private DialogueDatabaseRuntime _database;
	private CalendarHintController _hintController;

	private Color _initColor;

	private bool _isTransitioning = false;
	private bool _isYesFocused = false;
	private bool _isNoFocused = false;

	private string _hintId;

	private HintType _hintType;

	private UI_Popup _parentPopup;

	private void Awake()
	{
		_database = gameObject.AddComponent<DialogueDatabaseRuntime>();

		_database.LoadAllFromResources();
	}

	public void Init(string id, HintType hintType, CalendarHintController calendarHintController)
	{
		_hintId = id;
		_hintType = hintType;
		_hintController = calendarHintController;

		if (_hintType == HintType.CalendarHint)
		{
			_hintController.Pause();
		}

		_initColor = _yesText.color;

		bool success = _database.TryGet(_hintId, out var row);

		Debug.Log("");

		if (success)
			_questionText.text = row.script;
		else
			_questionText.text = "힌트를 보시겠습니까?";

		_infoImage.GetComponent<CanvasGroup>().alpha = 0f;

		UI_EventHandler _yesTextEvent = _yesText.GetComponent<UI_EventHandler>();
		_yesTextEvent.OnPointerEnterHandler += (PointerEventData data) =>
		{
			OnTextFocus(_yesText); OffTextFocus(_noText);
			_isYesFocused = true;
			_isNoFocused = false;
		};
		_yesTextEvent.OnPointerExitHandler += (PointerEventData data) =>
		{
			OffTextFocus(_yesText); OffTextFocus(_noText);
			_isYesFocused = false;
			_isNoFocused = false;
		};
		_yesTextEvent.OnPointerUpHandler += (PointerEventData data) =>
		{
			_infoImage.SetActive(false);
			OnClickYesButton();
		};

		UI_EventHandler _noTextEvent = _noText.GetComponent<UI_EventHandler>();
		_noTextEvent.OnPointerEnterHandler += (PointerEventData data) =>
		{
			OnTextFocus(_noText); OffTextFocus(_yesText);
			_isYesFocused = true;
			_isNoFocused = false;
		};
		_noTextEvent.OnPointerExitHandler += (PointerEventData data) =>
		{
			OffTextFocus(_noText); OffTextFocus(_noText);
			_isYesFocused = false;
			_isNoFocused = false;
		};
		_noTextEvent.OnPointerUpHandler += (PointerEventData data) =>
		{
			OnClickNoButton();
		};

		_closeButton.onClick.AddListener(OnClickNoButton);

		PlayPopupScale();
	}

	public void SetParentPopup(UI_Popup popup)
	{
		_parentPopup = popup;
	}

	private void OnClickNoButton()
	{
		if (_state != PopupState.Opened)
			return;

		PlayPopupScaleReverse();
	}

	private void OnClickYesButton()
	{
		if (_state != PopupState.Opened)
			return;

		Managers.UI.ClosePopupUI();
		Managers.UI.ShowPopupUI<UI_HintPopup>().Init(_hintType, _hintController);
	}

	public void OnTextFocus(TMP_Text text) => text.color = Color.red;
	public void OffTextFocus(TMP_Text text) => text.color = _initColor;

	private void PlayPopupScale()
	{
		if (_state != PopupState.Closed)
			return;

		_state = PopupState.Opening;

		// 팝업 초기 크기
		_root.localScale = Vector3.one * 0.7f;

		// 배경 초기 알파
		Color color = _popupOverlay.color;
		color.a = 0f;
		_popupOverlay.color = color;

		// 배경 먼저 어둡게
		_popupOverlay.DOFade(0.77f, 0.7f)
			.SetEase(Ease.Linear)
			.SetLink(gameObject)
			.OnComplete(() =>
			{
				_infoImage.GetComponent<CanvasGroup>().alpha = 1f;
				// 팝업 나중에 커지기
				_root.DOScale(1f, 0.32f)
					 .SetEase(Ease.OutBack)
					 .SetLink(gameObject)
					 .OnComplete(() =>
					 {
						 _state = PopupState.Opened;

					 });
			});
	}

	private void PlayPopupScaleReverse()
	{
		if (_state != PopupState.Opened)
			return;

		_state = PopupState.Closing;

		_root.DOScale(0f, 0.32f)
					 .SetEase(Ease.Linear)
					 .SetLink(gameObject)
					 .OnComplete(() =>
					 {
						 _popupOverlay.DOFade(0f, 0.7f)
						 .SetEase(Ease.Linear)
						 .SetLink(gameObject)
						 .OnComplete(() =>
						 {
							 _state = PopupState.Closed;
							 Managers.UI.ClosePopupUI();						 
						 });
					 });
	}

	private void OnDestroy()
	{
		if (_parentPopup is UI_CalendarMiniGamePopup calendarMiniGamePopup)
		{
			calendarMiniGamePopup.IsHintButtonClicked = false;
			return;
		}
		if (_parentPopup is UI_ClockMiniGamePopup clockMiniGamePopup)
		{
			clockMiniGamePopup.IsHintButtonClicked = false;
		}
	}
}
