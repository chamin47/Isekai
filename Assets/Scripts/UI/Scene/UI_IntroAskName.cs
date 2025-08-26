using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_IntroAskName : UI_Scene
{
	[Header("BG / 색상")]
	[SerializeField] private Image _bg;

	[Header("0001 : 질문 타이핑")]
	[SerializeField] private TMP_Text _questionText;
	[SerializeField] private CanvasGroup _questionGroup;
	[SerializeField] private float _fade = 1f;

	[Header("0002 : 입력 필드 묶음")]
	[SerializeField] private CanvasGroup _inputGroup;          // 괄호 + 인풋 전체를 감싸는 그룹
	[SerializeField] private TMP_Text _leftBracket;            // “[”
	[SerializeField] private TMP_Text _rightBracket;           // “]”
	[SerializeField] private TMP_InputField _input;            // 실제 입력 필드

	[SerializeField] private Image _fadeImage;

	private string _lastComposition = "";
	private string _lastText = "";
	[SerializeField] private float _soundDelay = 0.075f;
	private float _soundTimer;

	const string LINE = "당신의 이름은 무엇인가요?";

	public override void Init()
	{
		base.Init();

		// 초기 상태
		_questionText.text = "";
		_questionGroup.alpha = 1f;
		_inputGroup.alpha = 0f;

		// 괄호 고정 텍스트
		_leftBracket.text = "[";
		_rightBracket.text = "]";
		// 클릭 방해 안 하도록
		_leftBracket.raycastTarget = false;
		_rightBracket.raycastTarget = false;

		// 인풋 설정
		_input.characterLimit = 4;
		_input.lineType = TMP_InputField.LineType.SingleLine;
		_input.contentType = TMP_InputField.ContentType.Standard;
		_input.customCaretColor = true;                        
		_input.caretColor = _input.textComponent.color;
		_input.caretWidth = 2;

		_input.onSubmit.AddListener((value) => { StartCoroutine(OnSubmit(value)); } );

		// 입력 중 공백 제거하고 한글 조합 안정성
		_input.onValueChanged.AddListener(val =>
		{
			// 공백 제거
			if (!string.IsNullOrEmpty(val) && val.Contains(" "))
				_input.text = val.Replace(" ", "");
		});

		StartCoroutine(Flow());
	}

	IEnumerator Flow()
	{
		// 0001 : 질문 타이핑
		Managers.Sound.Play("intro_typing2", Sound.SubEffect);
		yield return _questionText.CoTypingEffect(LINE, 0.075f);
		Managers.Sound.PauseSubEffect();

		yield return WaitForSecondsCache.Get(2f);

		// 0002 : 괄호 + 인풋 등장 (커서 깜빡이며 입력 가능)
		yield return _inputGroup.CoFadeOut(0);

		_input.text = IntroRuntime.IsValid ? IntroRuntime.PlayerName : "";
		_input.Select();
		_input.ActivateInputField();
	}

	void Update()
	{
		// 한글 조합/입력 감지 → 타건 사운드
		if (Input.compositionString != _lastComposition)
		{
			if (!string.IsNullOrEmpty(Input.compositionString))
				PlayTypeSound();
			_lastComposition = Input.compositionString;
		}

		if (_input.isActiveAndEnabled && _input.text != _lastText)
		{
			PlayTypeSound();
			_lastText = _input.text;
		}

		_soundTimer += Time.deltaTime;
	}

	void PlayTypeSound()
	{
		if (_soundTimer < _soundDelay) return;
		_soundTimer = 0f;
		Managers.Sound.Play("intro_type_short", Sound.Effect);
	}

	IEnumerator OnSubmit(string t)
	{
		t = (t ?? "").Trim();
		if (t.Length < 1) yield break; ; // 최소 1자

		IntroRuntime.SetName(t);

		yield return _fadeImage.CoFadeOut(3f);
		Managers.UI.ShowPopupUI<UI_IntroBookPopup>(); // 0003
		gameObject.SetActive(false);
	}
}
