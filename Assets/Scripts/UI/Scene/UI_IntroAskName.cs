using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_IntroAskName : UI_Scene
{
	[Header("BG / ����")]
	[SerializeField] private Image _bg;

	[Header("0001 : ���� Ÿ����")]
	[SerializeField] private TMP_Text _questionText;
	[SerializeField] private CanvasGroup _questionGroup;
	[SerializeField] private float _fade = 1f;

	[Header("0002 : �Է� �ʵ� ����")]
	[SerializeField] private CanvasGroup _inputGroup;          // ��ȣ + ��ǲ ��ü�� ���δ� �׷�
	[SerializeField] private TMP_Text _leftBracket;            // ��[��
	[SerializeField] private TMP_Text _rightBracket;           // ��]��
	[SerializeField] private TMP_InputField _input;            // ���� �Է� �ʵ�

	[SerializeField] private Image _fadeImage;

	private string _lastComposition = "";
	private string _lastText = "";
	[SerializeField] private float _soundDelay = 0.075f;
	private float _soundTimer;

	const string LINE = "����� �̸��� �����ΰ���?";

	public override void Init()
	{
		base.Init();

		// �ʱ� ����
		_questionText.text = "";
		_questionGroup.alpha = 1f;
		_inputGroup.alpha = 0f;

		// ��ȣ ���� �ؽ�Ʈ
		_leftBracket.text = "[";
		_rightBracket.text = "]";
		// Ŭ�� ���� �� �ϵ���
		_leftBracket.raycastTarget = false;
		_rightBracket.raycastTarget = false;

		// ��ǲ ����
		_input.characterLimit = 4;
		_input.lineType = TMP_InputField.LineType.SingleLine;
		_input.contentType = TMP_InputField.ContentType.Standard;
		_input.customCaretColor = true;                        
		_input.caretColor = _input.textComponent.color;
		_input.caretWidth = 2;

		_input.onSubmit.AddListener((value) => { StartCoroutine(OnSubmit(value)); } );

		// �Է� �� ���� �����ϰ� �ѱ� ���� ������
		_input.onValueChanged.AddListener(val =>
		{
			// ���� ����
			if (!string.IsNullOrEmpty(val) && val.Contains(" "))
				_input.text = val.Replace(" ", "");
		});

		StartCoroutine(Flow());
	}

	IEnumerator Flow()
	{
		// 0001 : ���� Ÿ����
		Managers.Sound.Play("intro_typing2", Sound.SubEffect);
		yield return _questionText.CoTypingEffect(LINE, 0.075f);
		Managers.Sound.PauseSubEffect();

		yield return WaitForSecondsCache.Get(2f);

		// 0002 : ��ȣ + ��ǲ ���� (Ŀ�� �����̸� �Է� ����)
		yield return _inputGroup.CoFadeOut(0);

		_input.text = IntroRuntime.IsValid ? IntroRuntime.PlayerName : "";
		_input.Select();
		_input.ActivateInputField();
	}

	void Update()
	{
		// �ѱ� ����/�Է� ���� �� Ÿ�� ����
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
		if (t.Length < 1) yield break; ; // �ּ� 1��

		IntroRuntime.SetName(t);

		yield return _fadeImage.CoFadeOut(3f);
		Managers.UI.ShowPopupUI<UI_IntroBookPopup>(); // 0003
		gameObject.SetActive(false);
	}
}
