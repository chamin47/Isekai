using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PrologueBookPopup : UI_Popup
{
	[Header("Toggle")]
	[SerializeField] private Sub_Toggle _historyToggle;

	[Header("Label")]
	[SerializeField, TextArea] private string _label = "지금까지의 이야기 보기";
	[SerializeField] private float _charInterval = 0.075f;

	[Header("FX")]
	[SerializeField] private Image _fadeImage;  // 화면 전체 페이드
	[SerializeField] private Sprite _onImage;

	[SerializeField] private CanvasGroup _canvasGroup;

	[Header("Blink Target")]
	[SerializeField] private Image _checkmark;  // 깜빡일 이미지

	// 체크마크 블링크 파라미터
	[SerializeField] private float _blinkMinAlpha = 0.12f;
	[SerializeField] private float _blinkMaxAlpha = 1f;
	[SerializeField] private float _blinkSpeed = 2f;
	[SerializeField] private float _blinkLowHold = 0f;
	[SerializeField] private float _blinkHighHold = 0f;
	[SerializeField] private bool _blinkUseUnscaledTime = true;

	private bool _isNavigating;
	private Coroutine _blinkRoutine;

	private void Awake()
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
	}

	public override void Init()
	{
		base.Init();

		_historyToggle.Toggle.onValueChanged.AddListener(OnToggleChanged);
		_historyToggle.Toggle.SetIsOnWithoutNotify(false);
		_historyToggle.Toggle.interactable = false;
		_historyToggle.Toggle.onValueChanged.Invoke(false);

		// 체크마크 보장(토글 off여도 이미지 자체는 보이도록)
		if (_checkmark)
		{
			_checkmark.raycastTarget = false;
			SetImageAlpha(_checkmark, 0f); // 시작은 투명
			_checkmark.enabled = true;
		}

		StartCoroutine(TypeLabelThenEnable());
	}

	private IEnumerator TypeLabelThenEnable()
	{
		yield return _canvasGroup.FadeCanvas(1f, 3f);

		_checkmark.raycastTarget = true;
		_historyToggle.Toggle.interactable = true;
		_historyToggle.BackGround.SetActive(true);

		// 여기서 체크마크 깜빡임 시작 (UI_Image 알파 조절 방식)
		if (_checkmark)
		{
			StopBlink();
			_blinkRoutine = StartCoroutine(CoBlinkImageAlpha(
				_checkmark,
				_blinkMinAlpha,
				_blinkMaxAlpha,
				_blinkSpeed,
				_blinkLowHold,
				_blinkHighHold,
				_blinkUseUnscaledTime
			));
		}

	}

	private void OnToggleChanged(bool on)
	{
		if (!on || _isNavigating) return;

		_isNavigating = true;
		_historyToggle.Toggle.interactable = false;
		_checkmark.raycastTarget = false;

		_historyToggle.Toggle.image.sprite = _onImage;

		// 토글되면 깜빡임 정지 + 알파 1로 고정
		if (_checkmark)
		{
			StopBlink();
			SetImageAlpha(_checkmark, 1f);
		}

		StartCoroutine(GoToIntro2());
	}

	private IEnumerator GoToIntro2()
	{
		yield return _canvasGroup.FadeCanvas(0f, 3f);
		yield return WaitForSecondsCache.Get(2f);

		Managers.UI.ShowSceneUI<UI_Intro2Video>();
		Managers.UI.ClosePopupUI(this);
	}

	private void OnDestroy()
	{
		if (_historyToggle != null && _historyToggle.Toggle != null)
			_historyToggle.Toggle.onValueChanged.RemoveListener(OnToggleChanged);

		StopBlink();
	}

	private void StopBlink()
	{
		if (_blinkRoutine != null)
		{
			StopCoroutine(_blinkRoutine);
			_blinkRoutine = null;
		}
	}

	private void SetImageAlpha(Image img, float a)
	{
		if (!img) return;
		var c = img.color; c.a = a; img.color = c;
	}

	/// <summary>
	/// Image.color.a를 min~max로 부드럽게 왕복(핑퐁)시키는 깜빡임.
	/// </summary>
	private IEnumerator CoBlinkImageAlpha(
		Image img,
		float minAlpha,
		float maxAlpha,
		float speed,
		float lowHold,
		float highHold,
		bool unscaled
	)
	{
		if (!img) yield break;

		float t = 0f;
		const float eps = 0.02f;

		while (true)
		{
			t += unscaled ? Time.unscaledDeltaTime : Time.deltaTime;

			float p = Mathf.PingPong(t * speed, 1f);
			float a01 = Mathf.SmoothStep(0f, 1f, p);
			float alpha = Mathf.Lerp(minAlpha, maxAlpha, a01);
			SetImageAlpha(img, alpha);

			if (alpha >= maxAlpha - eps)
			{
				if (highHold > 0f)
					yield return unscaled ? new WaitForSecondsRealtime(highHold) : new WaitForSeconds(highHold);
			}
			else if (alpha <= minAlpha + eps)
			{
				if (lowHold > 0f)
					yield return unscaled ? new WaitForSecondsRealtime(lowHold) : new WaitForSeconds(lowHold);
			}
			else
			{
				yield return null;
			}
		}
	}
}
