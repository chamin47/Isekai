using System.Collections;
using TMPro;
using UnityEngine;

public class UI_LockedBookNotice : UI_Popup
{
	[SerializeField] private CanvasGroup _canvasGroup;
	[SerializeField] private RectTransform _rect;
	[SerializeField] private TMP_Text _text;

	private static bool _isShowing;

	private const float MOVE_OFFSET_Y = -40f;
	private const float MOVE_DURATION = 0.25f;
	private const float SHOW_TIME = 2f;
	private const float FADE_OUT_TIME = 1f;

	public static bool IsShowing => _isShowing;

	public static void ResetState()
	{
		_isShowing = false;
	}

	public override void Init()
	{
		base.Init();

		_isShowing = true;

		_text.text = "지금은 열람할 수 없는 책입니다.";

		StartCoroutine(CoPlay());
	}

	private IEnumerator CoPlay()
	{
		Vector2 startPos = _rect.anchoredPosition + Vector2.up * MOVE_OFFSET_Y;
		Vector2 targetPos = _rect.anchoredPosition;

		_rect.anchoredPosition = startPos;
		_canvasGroup.alpha = 0f;

		float t = 0f;
		while (t < MOVE_DURATION)
		{
			t += Time.deltaTime;
			float r = t / MOVE_DURATION;

			_rect.anchoredPosition = Vector2.Lerp(startPos, targetPos, r);
			_canvasGroup.alpha = r;

			yield return null;
		}

		_rect.anchoredPosition = targetPos;
		_canvasGroup.alpha = 1f;

		yield return WaitForSecondsCache.Get(SHOW_TIME);

		t = 0f;
		while (t < FADE_OUT_TIME)
		{
			t += Time.deltaTime;
			_canvasGroup.alpha = 1f - (t / FADE_OUT_TIME);
			yield return null;
		}

		_canvasGroup.alpha = 0f;

		_isShowing = false;
		Managers.UI.ClosePopupUI(this);
	}

	private void OnDestroy()
	{
		_isShowing = false;
	}
}
