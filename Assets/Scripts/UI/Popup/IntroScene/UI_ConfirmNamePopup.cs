using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ConfirmNamePopup : UI_Popup
{
	[Header("Texts")]
	[SerializeField] private TMP_Text _questionText;
	[SerializeField] private TMP_Text _yesText;
	[SerializeField] private TMP_Text _noText;

	[Header("FX")]
	[SerializeField] private Image _fadeImage;
	[SerializeField] private Image _popupOverlay;
	[SerializeField] private GameObject _infoImage;

	[SerializeField] private RectTransform _root;

	private Color _initColor;

	private bool _isTransitioning = false;
	private bool _isYesFocused = false;
	private bool _isNoFocused = false;

	public override void Init()
	{
		base.Init();

		_initColor = _yesText.color;

		_questionText.text = "당신의 이름이 맞나요?";

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

		PlayPopupScale();
	}

	private void OnClickYesButton()
	{
		StartCoroutine(GoTo0005());
	}

	private void OnClickNoButton()
	{
		if (_isTransitioning) return;
		_isTransitioning = true;

		PlayPopupScaleReverse();
	}

	/// <summary>
	/// 팝업을 순차적으로 닫는 이유: 
	/// 맨 먼저 열려있던 UI_IntroBookPopup이 떠 있는 상태에서
	/// UI_ConfirmNamePopup를 띄우고 UI_PrologueBookPopup이 열릴 때 한꺼번에 닫기 위해
	/// UI_IntroBookPopup 배경 위에 UI_ConfirmNamePopup이게 떠 있어야 해서.
	/// </summary>
	private IEnumerator GoTo0005()
	{
		if (_isTransitioning) yield break;
		_isTransitioning = true;

		GetComponentInChildren<Image>().gameObject.SetActive(false);

		var popup = FindAnyObjectByType<UI_IntroBookPopup>();
		yield return popup._canvasGroup.FadeCanvas(0f, 3f);

		Managers.Sound.Play("s2_book1", Sound.Effect);

		// **순서 중요**: 현재 팝업을 먼저 닫고 → 새 팝업 오픈
		Managers.UI.ClosePopupUI(this);    // UI_ConfirmNamePopup를 먼저 닫고
		Managers.UI.ClosePopupUI();        // UI_IntroBookPopup를 순차적으로 닫는다!
		Managers.UI.ShowPopupUI<UI_PrologueBookPopup>(); // 0005
	}

	public void OnTextFocus(TMP_Text text) => text.color = Color.red;
	public void OffTextFocus(TMP_Text text) => text.color = _initColor;

	private void PlayPopupScale()
	{
		// 팝업 초기 크기
		_root.localScale = Vector3.one * 0.7f;

		// 배경 초기 알파
		Color color = _popupOverlay.color;
		color.a = 0f;
		_popupOverlay.color = color;

		// 배경 먼저 어둡게
		_popupOverlay.DOFade(0.77f, 0.7f)
			.SetEase(Ease.Linear)
			.OnComplete(() =>
			{
				_infoImage.GetComponent<CanvasGroup>().alpha = 1f;
				// 팝업 나중에 커지기
				_root.DOScale(1f, 0.32f)
					 .SetEase(Ease.OutBack);
			});
	}

	private void PlayPopupScaleReverse()
	{
		_root.DOScale(0f, 0.32f)
					 .SetEase(Ease.Linear)
					 .OnComplete(() =>
					 {
						 _popupOverlay.DOFade(0f, 0.7f)
						 .SetEase(Ease.Linear)
						 .OnComplete(() =>
						 {
							 Managers.UI.ClosePopupUI(this);
							 Managers.UI.ClosePopupUI();
							 Managers.UI.ShowPopupUI<UI_EditNamePopup>(); // 0004_N});
						 });
					 });
	}
}