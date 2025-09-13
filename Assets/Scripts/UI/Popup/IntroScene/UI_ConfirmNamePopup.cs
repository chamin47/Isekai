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

	[SerializeField] private GameObject _infoImage;


	private Color _initColor;

	private bool _isTransitioning = false;
	private bool _isYesFocused = false;
	private bool _isNoFocused = false;

	public override void Init()
	{
		base.Init();

		_initColor = _yesText.color;

		_questionText.text = "당신의 이름이 맞나요?";

		UI_EventHandler _yesTextEvent = _yesText.GetComponent<UI_EventHandler>(); 
		_yesTextEvent.OnPointerEnterHandler += (PointerEventData data) => { OnTextFocus(_yesText); OffTextFocus(_noText);
			_isYesFocused = true;
			_isNoFocused = false;
		};
		_yesTextEvent.OnPointerExitHandler += (PointerEventData data) => { OffTextFocus(_yesText); OffTextFocus(_noText); 
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
	}

	private void OnClickYesButton()
	{
		StartCoroutine(GoTo0005());
	}

	private void OnClickNoButton()
	{
		if (_isTransitioning) return;
		_isTransitioning = true;

		Managers.UI.ClosePopupUI(this);
		Managers.UI.ClosePopupUI();
		Managers.UI.ShowPopupUI<UI_EditNamePopup>(); // 0004_N
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
}