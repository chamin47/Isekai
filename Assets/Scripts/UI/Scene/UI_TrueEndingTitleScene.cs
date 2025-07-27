using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 엔딩을 본 뒤 진‑엔딩 루트로 넘어가는 전용 타이틀 UI<br/>
/// </summary>
public class UI_TrueEndingTitleScene : UI_Scene
{
	[Header("Main UI")]
	[SerializeField] private CanvasGroup _inputCanvasGroup; // 장면2 UI용
	[SerializeField] private TMP_InputField _inputField;

	[SerializeField] private CanvasGroup _intro2Group;
	[SerializeField] private TMP_Text _intro2Text;   // 정방향
	[SerializeField] private TMP_Text _mirrorText;   // 좌우 + 상하 Flip

	[Header("Choice UI")]
	[SerializeField] private CanvasGroup _choiceGroup;
	[SerializeField] private TMP_Text _yesText;
	[SerializeField] private TMP_Text _noText;

	[Header("Fade")]
	[SerializeField] private float _fadeDuration = 1f;

	// Internal State ──────────────────────────────────────────────────
	private Vector2 _intro2InitPos;
	private int _choiceIndex;                       // 0 = YES / 1 = NO
	private readonly Color _normalCol = new(0.33f, 0.33f, 0.33f, 1); // #555555
	private readonly Color _focusCol = Color.white;

	// Typing‑Sound Helper
	private string _lastInputText = "";
	private string _lastComposition = "";
	[SerializeField] private float _soundDelay = 0.075f;
	private float _soundTimer;

	// Dialogue Constants
	private const string Intro2LineA = "이 세계에서는 행복하셨나요?";
	private const string Intro2LineB = "<color=#FF0000>이세계에서만 행복하셨어요?</color>";

	private readonly List<string> _lines5 = new()
	{
		"...하지만 결국 이 세계의 나를 잃어버리셨군요.",
		"<color=#FF0000>...하지만 결국 이 세계의 나는 죽고야말았군요.</color>"
	};

	private const string _line6 =
		"새로운 기회가 주어진다면...\n이번엔 이 세계의 나를 지킬 수 있나요?";

	private const string _yesLineA = "이번에는 ... 꼭 행복해지길 :)";
	private const string _yesLineB = "<color=#FF0000>이번에는 ... 꼭 실패하시길 :)</color>";

	public override void Init()
	{
		base.Init();

		// UI 초기화
		_intro2Group.alpha = 0;
		_intro2InitPos = _intro2Text.rectTransform.anchoredPosition;

		_inputCanvasGroup.alpha = 0f;
		_inputField.characterLimit = 20;
		_inputField.onSubmit.AddListener(OnInputSubmit);

		StartCoroutine(PlayIntroSequence2());
	}

	void Update()
	{
		// 한글 감지를 위한 작업
		if (Input.compositionString != _lastComposition)
		{
			if (!string.IsNullOrEmpty(Input.compositionString))
			{
				OnInputFieldChanged();
			}
			_lastComposition = Input.compositionString;
		}

		// 2) 실제 입력 필드 텍스트 변경 감지
		if (_inputField.text != _lastInputText)
		{
			OnInputFieldChanged();
			_lastInputText = _inputField.text;
		}

		_soundTimer += Time.deltaTime;
	}

	private void OnInputFieldChanged()
	{
		if (_soundTimer < _soundDelay)
			return;

		_soundTimer = 0f;
		Managers.Sound.Play("intro_type_short", Sound.Effect);
	}


	private IEnumerator PlayIntroSequence2()
	{
		_inputField.textComponent.color = Color.red;
		_inputField.placeholder.GetComponent<TextMeshProUGUI>().color = Color.red;

		yield return new WaitForSeconds(2f);               // 블랙 유지

		_intro2Group.alpha = 1f;

		Managers.Sound.Play("intro_typing2", Sound.SubEffect);
		Coroutine c1 = StartCoroutine(TypeText(_intro2Text, Intro2LineA, 0.075f));
		Coroutine c2 = StartCoroutine(TypeText(_mirrorText, Intro2LineB, 0.075f));
		yield return c1; yield return c2;
		Managers.Sound.PauseSubEffect();

		yield return WaitForSecondsCache.Get(1f);

		yield return FadeCanvas(_intro2Group, 0f, _fadeDuration);   // 알파 0으로
		_intro2Text.text = "";
		_mirrorText.text = "";

		_intro2Text.rectTransform.localScale =
		_mirrorText.rectTransform.localScale = Vector3.one;

		yield return FadeCanvas(_intro2Group, 1, _fadeDuration);

		yield return ShowInputField();
		while (_inputCanvasGroup.alpha > 0f && _inputField.gameObject.activeSelf)
			yield return null;

		Coroutine l5A = StartCoroutine(TypeWithEllipsis(_intro2Text, _lines5[0]));
		Coroutine l5B = StartCoroutine(TypeWithEllipsis(_mirrorText, _lines5[1]));
		yield return l5A; yield return l5B;

		yield return WaitForSecondsCache.Get(2f);                 // 2초 정지
		yield return FadeCanvas(_intro2Group, 0, _fadeDuration);   // 전체 아웃
		_intro2Text.text = _mirrorText.text = "";
		yield return FadeCanvas(_intro2Group, 1, _fadeDuration);   // 복구

		// 마지막 질문 한 줄
		_intro2Text.rectTransform.anchoredPosition =
			new Vector2(_intro2Text.rectTransform.anchoredPosition.x, 170f);

		yield return TypeWithEllipsis(_intro2Text, _line6);

		yield return ShowChoice();
	}


	IEnumerator ShowChoice()
	{
		_choiceGroup.alpha = 1;
		_choiceGroup.interactable = _choiceGroup.blocksRaycasts = true;
		_choiceIndex = 0;
		RefreshChoiceUI();

		while (true)
		{
			if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
			{
				_choiceIndex = 1 - _choiceIndex;          // 0↔1 토글
				RefreshChoiceUI();
				Managers.Sound.Play("intro_type_short", Sound.Effect);
			}
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
				break;

			yield return null;
		}

		yield return FadeCanvas(_choiceGroup, 0, _fadeDuration);

		if (_choiceIndex == 0)          // YES 
		{
			yield return YesSequence();
		}
		else
		{

		}
	}

	void RefreshChoiceUI()
	{
		// 현재 포커스에만 화살표 + 밝은 색
		_yesText.text = _choiceIndex == 0 ? "▶ YES" : "  YES";
		_noText.text = _choiceIndex == 1 ? "▶ NO" : "  NO";

		_yesText.color = (_choiceIndex == 0) ? _focusCol : _normalCol;
		_noText.color = (_choiceIndex == 1) ? _focusCol : _normalCol;
	}

	IEnumerator YesSequence()
	{
		_intro2Text.rectTransform.anchoredPosition = _intro2InitPos;
		_intro2Text.text = _mirrorText.text = "";

		// ‘행복/실패’ 두 줄 동시 출력
		Coroutine a = StartCoroutine(TypeWithEllipsis(_intro2Text, _yesLineA));
		Coroutine b = StartCoroutine(TypeWithEllipsis(_mirrorText, _yesLineB));
		yield return a; yield return b;

		yield return WaitForSecondsCache.Get(2f);

		yield return FadeCanvas(_intro2Group, 0, 1f);   // 1 초 fade out
		yield return WaitForSecondsCache.Get(2f);       // 검정 유지 2 초

		Managers.Scene.LoadScene(Scene.LibraryScene);
	}

	private IEnumerator ShowInputField()
	{
		// 입력 필드 표시
		_inputField.text = "";
		_inputField.gameObject.SetActive(true);

		float elapsed = 0f;
		float startAlpha = _inputCanvasGroup.alpha;
		while (elapsed < _fadeDuration)
		{
			elapsed += Time.deltaTime;
			_inputCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed / _fadeDuration);
			yield return null;
		}
		_inputCanvasGroup.alpha = 1f;

		_inputField.Select();
		_inputField.ActivateInputField();
	}

	private void OnInputSubmit(string userInput)
	{
		// 엔터 입력 시 입력 필드 페이드 아웃
		StartCoroutine(HideInputField());
	}

	private IEnumerator HideInputField()
	{
		float elapsed = 0f;
		float startAlpha = _inputCanvasGroup.alpha;
		while (elapsed < _fadeDuration)
		{
			elapsed += Time.deltaTime;
			_inputCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / _fadeDuration);
			yield return null;
		}
		_inputCanvasGroup.alpha = 0f;

		_inputField.onSubmit.RemoveListener(OnInputSubmit);

		_inputField.gameObject.SetActive(false);
	}

	IEnumerator TypeText(TMP_Text target, string content, float speed)
	{
		target.text = "";
		int i = 0;

		while (i < content.Length)
		{
			// 1. 태그 시작이면 한 번에 몰아서 출력
			if (content[i] == '<')
			{
				int close = content.IndexOf('>', i);
				if (close == -1)   // 혹시 닫는 괄호가 없으면 안전하게 탈출
					break;

				target.text += content.Substring(i, close - i + 1);
				i = close + 1;     // 인덱스 점프
				yield return null; // 태그는 지연 없이 한 프레임에 처리
			}
			// 2. 일반 글자면 한 글자씩 출력
			else
			{
				target.text += content[i];
				i++;
				yield return new WaitForSeconds(speed);
			}
		}
	}

	IEnumerator TypeWithEllipsis(TMP_Text txt, string full)
	{
		/* 0) <color=…></color> 태그 보호 --------------------------- */
		string openTag = "", closeTag = "";
		if (full.StartsWith("<"))
		{
			int e = full.IndexOf('>');
			openTag = full[..(e + 1)];
			full = full[(e + 1)..];
		}
		int c = full.LastIndexOf("</");
		if (c != -1)
		{
			closeTag = full[c..];
			full = full[..c];
		}

		/* 1) pre / dots / post 분리 ------------------------------- */
		int idx = full.IndexOf("...");
		string pre = idx > -1 ? full[..idx] : full;
		string post = idx > -1 ? full[(idx + 3)..] : "";

		/* 2‑A) pre : 0.075 s 타이핑 + 효과음 ----------------------- */
		txt.text = openTag;
		int tick = 0;
		foreach (char ch in pre)
		{
			txt.text += ch;
			if (ch != ' ' && ch != '\n')
			{
				if (++tick % 2 == 0)
					Managers.Sound.Play("intro_type_short", Sound.Effect);
			}
			yield return WaitForSecondsCache.Get(0.075f);
		}

		if (pre.Length > 0)
			yield return WaitForSecondsCache.Get(1f);

		/* 2‑B) dots : 1 초 간격 3번 + 긴 타자음 ------------------- */
		Managers.Sound.Play("intro_typing2", Sound.SubEffect);
		for (int i = 0; i < 3; ++i)
		{
			txt.text += ".";
			yield return WaitForSecondsCache.Get(1f);
		}
		Managers.Sound.PauseSubEffect();

		/* 2‑C) post : 0.075 s 타이핑 + 효과음 ---------------------- */
		tick = 0;
		foreach (char ch in post)
		{
			txt.text += ch;
			if (ch != ' ' && ch != '\n')
			{
				if (++tick % 2 == 0)
					Managers.Sound.Play("intro_type_short", Sound.Effect);
			}
			yield return WaitForSecondsCache.Get(0.075f);
		}

		txt.text += closeTag;    // 닫힘 태그
	}

	IEnumerator FadeCanvas(CanvasGroup cg, float to, float time)
	{
		float from = cg.alpha;
		float t = 0f;
		while (t < time)
		{
			t += Time.deltaTime;
			cg.alpha = Mathf.Lerp(from, to, t / time);
			yield return null;
		}
		cg.alpha = to;
	}
}
