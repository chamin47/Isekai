using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_EditNamePopup : UI_Popup
{
	[SerializeField] private TMP_InputField _input;
	[SerializeField] private Image _fadeImage;

	private string _lastComposition = "";
	private string _lastText = "";
	[SerializeField] private float _soundDelay = 0.075f;
	private float _soundTimer;

	private void Awake()
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
	}

	public override void Init()
	{
		base.Init();
		_input.characterLimit = 4;
		_input.text = IntroRuntime.PlayerName;

		_input.lineType = TMP_InputField.LineType.SingleLine;
		_input.contentType = TMP_InputField.ContentType.Standard;
		_input.customCaretColor = true;
		_input.caretColor = _input.textComponent.color;
		_input.caretWidth = 2;

		_input.onSubmit.AddListener(OnSubmit);

		// 입력 중 공백 제거하고 한글 조합 안정성
		_input.onValueChanged.AddListener(val =>
		{
			// 공백 제거
			if (!string.IsNullOrEmpty(val) && val.Contains(" "))
				_input.text = val.Replace(" ", "");
		});

		_input.Select();
		_input.ActivateInputField();
	}

	void Update()
	{
		if (Input.compositionString != _lastComposition)
		{
			if (!string.IsNullOrEmpty(Input.compositionString))
				Tick();
			_lastComposition = Input.compositionString;
		}

		if (_input.text != _lastText)
		{
			Tick();
			_lastText = _input.text;
		}

		_soundTimer += Time.deltaTime;
	}

	void Tick()
	{
		if (_soundTimer < _soundDelay) return;
		_soundTimer = 0f;
		Managers.Sound.Play("intro_type_short", Sound.Effect);
	}

	void OnSubmit(string t)
	{
		t = t?.Trim() ?? "";
		if (t.Length < 1) return;
		IntroRuntime.SetName(t);
		StartCoroutine(To0005());
	}

	IEnumerator To0005()
	{
		yield return _fadeImage.CoFadeOut(2f, 1f);
		Managers.Sound.Play("s2_book1", Sound.Effect);
		Managers.UI.ClosePopupUI(this);
		Managers.UI.ShowPopupUI<UI_PrologueBookPopup>(); // 0005
	}
}
