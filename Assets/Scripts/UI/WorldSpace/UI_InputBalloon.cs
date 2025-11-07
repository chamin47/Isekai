using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 월드 스페이스 입력 말풍선.
/// TMP_InputField를 표시해 사용자의 텍스트 입력을 수집하고 페이드/위치 갱신을 수행한다.
/// </summary>
public class UI_InputBalloon : UI_Base
{
	[SerializeField] private RectTransform root;     // 말풍선 루트(배경 포함)
	[SerializeField] private CanvasGroup cg;
	[SerializeField] private Image _image;
	[SerializeField] private TMP_Text promptLabel;
	[SerializeField] private TMP_InputField inputField;

	[Header("Layout")]
	[SerializeField] private Vector2 screenOffset = new Vector2(0, 80f);
	[SerializeField] private bool rebuildWhileTyping = true;

	[Header("Behavior")]
	[SerializeField] private float fadeIn = 0.15f;
	[SerializeField] private float fadeOut = 0.12f;
	[SerializeField] private float caretBlinkRate = 0.6f;

	private Transform _anchor;
	private const int _charLimit = 25;

	private RectTransform _textRt;
	private readonly List<RectTransform> _caretRects = new();

	public void Init(Transform anchor)
	{
		_anchor = anchor;

		if (cg != null)
			cg.alpha = 0f;

		_textRt = inputField.textComponent.rectTransform;

		FixBubbleSize();
		SetPosition();

		// TMP 세팅
		if (inputField)
		{
			inputField.scrollSensitivity = 0;
			inputField.characterLimit = _charLimit;
			inputField.text = "";
			inputField.caretBlinkRate = caretBlinkRate;
			inputField.onValueChanged.AddListener(str =>
			{
				FixBubbleSize();
				SetPosition();
				Managers.Sound.Play("intro_type_short", Sound.Effect);
			});

			// 포커스 주기
			StartCoroutine(ActivateNextFrame());
		}
	}

	// TMP_InputField 내부의 실제 캐럿/선택 하이라이트 RectTransform을 수집
	private void BindCaretsIfNeeded()
	{
		if (inputField == null) return;

		// 이미 바인딩되어 있고 유효하면 패스
		_caretRects.RemoveAll(rt => rt == null);
		if (_caretRects.Count > 0) return;

		// TMP_SelectionCaret은 캐럿/선택 모두에 쓰인다. 전부 잡아서 같은 보정 적용
		var carets = inputField.GetComponentsInChildren<TMP_SelectionCaret>(true);
		foreach (var c in carets)
		{
			var rt = c.GetComponent<RectTransform>();
			if (rt != null) _caretRects.Add(rt);
		}
	}

	private void LateUpdate()
	{
		if (_textRt == null) return;

		// 캐럿 RectTransform 캐시(포커스 직후 생성되므로 필요할 때마다 확인)
		BindCaretsIfNeeded();

		// TMP 내부 스크롤로 인해 바뀐 x를 0으로 고정
		var p = _textRt.anchoredPosition;
		float px = p.x;
		if (Mathf.Abs(px) > 0.01f)
		{
			// 캐럿/선택 Rect들도 같은 양을 반대로 이동시켜 ‘보이는 텍스트’와 정렬
			//    (텍스트는 오른쪽으로 -px만큼 “되돌아가므로”, 캐럿은 좌표상 +(-px) 이동이 필요)
			foreach (var crt in _caretRects)
			{
				if (!crt) continue;
				var lp = crt.localPosition;
				lp.x -= px;                  // 핵심 한 줄
				crt.localPosition = lp;
			}

			// 텍스트는 x=0으로 고정
			_textRt.anchoredPosition = new Vector2(0f, p.y);
		}
	}


	private void FixBubbleSize()
	{
		Debug.Log($"Length: {promptLabel.text.Length} Width: {promptLabel.preferredWidth} Height: {promptLabel.preferredHeight}");

		//float preferredWidth = Mathf.Clamp(promptLabel.preferredWidth * 0.02f , 0.7f, 6f);

		float preferredWidth = Mathf.Clamp(Mathf.Sqrt(promptLabel.preferredWidth * 0.05f), 0.7f, 6f); // 루트 기반 보정(휴리스틱)
		float preferredHeight = Mathf.Max(promptLabel.preferredHeight * 0.05f - 0.05f, 0.7f);

		Vector2 preferredSize = new Vector2(preferredWidth, preferredHeight);

		// root 혹은 배경 이미지 RectTransform 크기 갱신
		_image.rectTransform.sizeDelta = preferredSize;
	}

	private void SetPosition()
	{
		if (_anchor == null)
			return;
		var cam = Camera.main;
		if (cam == null)
			return;

		// 기준 앵커의 월드 좌표
		Vector3 anchorWorld = _anchor.position;

		// 카메라 기준 Viewport 오프셋 (비율 단위)
		Vector3 viewportOffset = new Vector3(screenOffset.x, screenOffset.y, cam.nearClipPlane);

		// Viewport 비율 → 월드 거리로 변환
		Vector3 baseWorld = cam.ViewportToWorldPoint(Vector3.zero);
		Vector3 offsetWorld = cam.ViewportToWorldPoint(viewportOffset) - baseWorld;

		// 풍선 반폭 보정
		float bubbleHalfWidth = _image.rectTransform.sizeDelta.x * 0.5f * root.lossyScale.x;

		Vector3 pos = anchorWorld + offsetWorld;
		pos.x += bubbleHalfWidth;
		pos.z = anchorWorld.z;

		root.position = pos;
	}

	private IEnumerator ActivateNextFrame()
	{
		while (true)
		{
			yield return null;
			inputField?.ActivateInputField();
			inputField?.Select();
		}
	}

	public IEnumerator CoPrompt(string prompt, Action<string> onDone)
	{
		if (promptLabel) promptLabel.text = prompt ?? "";

		// 페이드 인
		if (cg != null)
			yield return cg.FadeCanvas(1f, fadeIn);

		string result = null;

		// Enter 키
		while (result == null)
		{
			if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
			{
				result = inputField.text;
				break;
			}
			yield return null;
		}

		onDone?.Invoke(result ?? "");
		Destroy(gameObject);
	}

	public override void Init() { }
}
