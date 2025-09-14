using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 엔딩을 본 뒤 진‑엔딩 루트로 넘어가는 전용 타이틀 UI
/// </summary>
public class UI_TrueEndingTitleScene : UI_Scene
{
	[Header("Main UI")]
	[SerializeField] private CanvasGroup _inputCanvasGroup; // 장면2 UI용
	[SerializeField] private TMP_InputField _inputField;

	[Header("Intro‑2")]
	[SerializeField] private CanvasGroup _intro2Group;
	[SerializeField] private TMP_Text _intro2Text;   // 정방향
	[SerializeField] private TMP_Text _mirrorText;   // 좌우 + 상하 Flip

	[Header("Choice UI")]
	[SerializeField] private CanvasGroup _choiceGroup;
	[SerializeField] private TMP_Text _yesText;
	[SerializeField] private TMP_Text _noText;

	[Header("Bubble UI")]
	[SerializeField] CanvasGroup _bubbleGroup;          // Bubble ‑ CanvasGroup
	[SerializeField] TMP_Text _bubbleText;              // BubbleText

	[Header("World Objects")]
	[SerializeField] PlayerController _player;

	[Header("Fade")]
	[SerializeField] private float _fadeDuration = 1f;

	[SerializeField] private float _errorFlashDuration = 0.12f;   // 번쩍 유지 시간
	private int _flashCounter;

	// Internal State 
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

	private readonly List<string> _demoVersionLines = new()
	{
		"지금까지 '이 세계 증후군' 데모 버전을\n 플레이해주셔서 감사합니다.",
		"주인공은 과연 이 세계의 나를 지켜낼 수 있을까요?",
		"그 여정은 정식 버전을 통해 확인해보세요\n 감사합니다 :)",
		"이 세계 증후군"
	};

	public override void Init()
	{
		base.Init();

		// UI 초기화
		_inputCanvasGroup.alpha = 0f;
		_intro2Group.alpha = 0f;
		_bubbleGroup.alpha = 0f;
		_intro2InitPos = _intro2Text.rectTransform.anchoredPosition;

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

		// 실제 입력 필드 텍스트 변경 감지
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
		_intro2Text.alignment = TextAlignmentOptions.Center;

		yield return new WaitForSeconds(2f);               // 블랙 유지

		_intro2Group.alpha = 1f;

		Managers.Sound.Play("intro_typing2", Sound.SubEffect);
		Coroutine c1 = StartCoroutine(TypeText(_intro2Text, Intro2LineA, 0.075f));
		Coroutine c2 = StartCoroutine(TypeText(_mirrorText, Intro2LineB, 0.075f));
		yield return c1; yield return c2;
		Managers.Sound.PauseSubEffect();

		yield return WaitForSecondsCache.Get(2f);

		yield return FadeCanvas(_intro2Group, 0f, _fadeDuration);   // 알파 0으로
		_intro2Text.text = "";
		_mirrorText.text = "";

		//_intro2Text.rectTransform.localScale =
		//_mirrorText.rectTransform.localScale = Vector3.one;

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
			new Vector2(_intro2Text.rectTransform.anchoredPosition.x, 102.2f);
		_intro2Text.alignment = TextAlignmentOptions.Top;

		yield return TypeWithEllipsis(_intro2Text, _line6);

		yield return WaitForSecondsCache.Get(2f);

		yield return ShowChoice();
	}

	IEnumerator ShowChoice()
	{
		_choiceGroup.alpha = 1;
		_choiceGroup.interactable = _choiceGroup.blocksRaycasts = true;

		_choiceIndex = 0;               // 0 = YES, 1 = NO
		RefreshChoiceUI();

		int noPress = 0;            // NO 연타 횟수
		float firstNoTime = 0f;         // 첫 NO 확정 시각

		while (true)
		{
			/* ← / → 포커스 이동 ------------------------------------ */
			if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
			{
				_choiceIndex = 1 - _choiceIndex;    // 0↔1 토글
				RefreshChoiceUI();
				Managers.Sound.Play("intro_type_short", Sound.Effect);

				/* ▶ 포커스를 NO 에서 벗어나면 카운터 리셋 */
				if (_choiceIndex != 1)
				{
					noPress = 0;
					firstNoTime = 0f;
				}
			}

			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
			{
				if (_choiceIndex == 0)
				{
					Coroutine c1 = StartCoroutine(FadeCanvas(_intro2Group, 0f, _fadeDuration));
					Coroutine c2 = StartCoroutine(FadeCanvas(_choiceGroup, 0f, _fadeDuration));
					yield return c1; yield return c2;
					yield return YesSequence();
					yield break;
				}

				if (_choiceIndex == 1)
				{
					if (noPress == 0)                       // 첫 번째 NO
						firstNoTime = Time.time;

					noPress++;
					//Managers.Sound.Play("intro_type_short", Sound.Effect);
					Managers.Sound.Play("click_error3", Sound.Effect);
					StartCoroutine(CoFlashInvalid());

					/* 5 초 안에 5회 성공 시 */
					if (noPress >= 5 && Time.time - firstNoTime <= 5f)
					{
						Coroutine c1 = StartCoroutine(FadeCanvas(_intro2Group, 0f, _fadeDuration));
						Coroutine c2 = StartCoroutine(FadeCanvas(_choiceGroup, 0f, _fadeDuration));
						yield return c1; yield return c2;

						yield return NoSequence();
						yield break;
					}

					/* 제한 시간 초과 시 카운터 초기화 */
					if (Time.time - firstNoTime > 5f)
					{
						noPress = 0;
						firstNoTime = 0f;
					}
				}
			}
			yield return null;
		}
	}

	IEnumerator CoFlashInvalid()
	{
		_flashCounter++;

		// 전체 선택지 패널을 살짝 어둡게
		const float DIM_ALPHA = 0.35f;
		_choiceGroup.alpha = DIM_ALPHA;

		yield return WaitForSecondsCache.Get(_errorFlashDuration);

		_flashCounter--;
		if (_flashCounter == 0)          // 마지막 플래시가 끝났을 때만 원복
			_choiceGroup.alpha = 1f;
	}

	void RefreshChoiceUI()
	{
		_yesText.text = _choiceIndex == 0 ? "▶ YES" : "  YES";
		_noText.text = _choiceIndex == 1 ? "▶ NO" : "  NO";

		_yesText.color = (_choiceIndex == 0) ? _focusCol : _normalCol;
		_noText.color = (_choiceIndex == 1) ? _focusCol : _normalCol;
	}

	IEnumerator YesSequence()
	{
		_intro2Text.rectTransform.anchoredPosition = _intro2InitPos;
		_intro2Text.text = _mirrorText.text = "";
		yield return FadeCanvas(_intro2Group, 1f, 0f);


		_intro2Text.alignment = TextAlignmentOptions.Center;
		Coroutine a = StartCoroutine(TypeWithEllipsis(_intro2Text, _yesLineA));
		Coroutine b = StartCoroutine(TypeWithEllipsis(_mirrorText, _yesLineB));
		yield return a; yield return b;

		yield return WaitForSecondsCache.Get(2f);

		yield return FadeCanvas(_intro2Group, 0f, 1f);
		_intro2Text.text = _mirrorText.text = "";
		_mirrorText.gameObject.SetActive(false);   
		yield return FadeCanvas(_intro2Group, 1f, 2f);

		_intro2Text.rectTransform.anchoredPosition = new Vector2(_intro2Text.rectTransform.anchoredPosition.x, 0f);

		_intro2Text.alignment = TextAlignmentOptions.Center;

		const float KEEP = 2f;                  
		foreach (string line in _demoVersionLines)
		{
			_intro2Text.text = line;               
			yield return WaitForSecondsCache.Get(KEEP);

			yield return FadeCanvas(_intro2Group, 0f, _fadeDuration);
			_intro2Text.text = "";
			yield return FadeCanvas(_intro2Group, 1f, 2f);
		}

		yield return WaitForSecondsCache.Get(2f);

		yield return FadeCanvas(_intro2Group, 0f, 1f);
		yield return WaitForSecondsCache.Get(1f);

		PlayerPrefs.DeleteAll();  // 모든 플레이어 정보 초기화
        Managers.Scene.LoadScene(Scene.TitleScene);   // 처음으로
	}


	IEnumerator NoSequence()
	{
		yield return FadeCanvas(_intro2Group, 0, 1f);
		_intro2Group.gameObject.SetActive(false);
		yield return WaitForSecondsCache.Get(1f);

		Managers.Scene.LoadScene(Scene.TrueEnding_NoRouteScene);
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
		// <color> 보호 
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

		int idx = full.IndexOf("...");
		string pre = idx >= 0 ? full[..idx] : full;
		string post = idx >= 0 ? full[(idx + 3)..] : "";

		txt.text = openTag;

		if (!string.IsNullOrEmpty(pre))
			Managers.Sound.Play("intro_typing2", Sound.SubEffect);

		foreach (char ch in pre)
		{
			txt.text += ch;
			yield return WaitForSecondsCache.Get(0.075f);
		}

		Managers.Sound.StopSubEffect();

		if (pre.Length > 0)
			yield return WaitForSecondsCache.Get(1f);   

		for (int i = 0; i < 3; ++i)
		{
			Managers.Sound.Play("intro_type_short", Sound.Effect);
			txt.text += ".";
			yield return WaitForSecondsCache.Get(1f);
		}

		if (!string.IsNullOrEmpty(post))
			Managers.Sound.Play("intro_typing2", Sound.SubEffect);

		foreach (char ch in post)
		{
			txt.text += ch;
			yield return WaitForSecondsCache.Get(0.075f);
		}
		Managers.Sound.StopSubEffect();

		txt.text += closeTag;
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
