using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UILetterboxOverlay : MonoBehaviour
{
	public event Action OnBackgroundClicked;

	private RectTransform _top, _bottom;
	private Image _blocker;

	private static UILetterboxOverlay _instance;

	public static UILetterboxOverlay GetOrCreate()
	{
		if (_instance != null) return _instance;

		var go = new GameObject("UILetterboxOverlay",
			typeof(Canvas), typeof(CanvasGroup), typeof(GraphicRaycaster));
		var cv = go.GetComponent<Canvas>();
		cv.renderMode = RenderMode.ScreenSpaceOverlay;
		cv.worldCamera = Camera.main;
		cv.sortingOrder = 9999;
		DontDestroyOnLoad(go);

		_instance = go.AddComponent<UILetterboxOverlay>();
		_instance.Build();
		return _instance;
	}

	private void Build()
	{
		var root = transform as RectTransform ?? gameObject.AddComponent<RectTransform>();
		root.anchorMin = Vector2.zero; root.anchorMax = Vector2.one;
		root.offsetMin = Vector2.zero; root.offsetMax = Vector2.zero;

		_blocker = MakeBlocker(root);
		_top = MakeBar(root, "TopBar", true);
		_bottom = MakeBar(root, "BottomBar", false);

		_blocker.gameObject.SetActive(false);
		SetBarHeight(0f);
	}

	private Image MakeBlocker(RectTransform parent)
	{
		var go = new GameObject("BgBlocker", typeof(RectTransform), typeof(Image), typeof(Button));
		var r = go.GetComponent<RectTransform>();
		r.SetParent(parent, false);
		r.anchorMin = Vector2.zero; r.anchorMax = Vector2.one;
		r.offsetMin = Vector2.zero; r.offsetMax = Vector2.zero;

		var img = go.GetComponent<Image>();
		img.color = new Color(0, 0, 0, 0);
		img.raycastTarget = true;

		var btn = go.GetComponent<Button>();
		btn.transition = Selectable.Transition.None;
		btn.onClick.AddListener(() => OnBackgroundClicked?.Invoke());

		return img;
	}

	private RectTransform MakeBar(RectTransform parent, string name, bool pivotTop)
	{
		var go = new GameObject(name, typeof(RectTransform), typeof(Image));
		var img = go.GetComponent<Image>();
		img.color = Color.black;
		img.raycastTarget = false;

		var r = go.GetComponent<RectTransform>();
		r.SetParent(parent, false);
		r.anchorMin = new Vector2(0, pivotTop ? 1 : 0);
		r.anchorMax = new Vector2(1, pivotTop ? 1 : 0);
		r.pivot = new Vector2(0.5f, pivotTop ? 1 : 0);
		r.sizeDelta = new Vector2(0, 0);
		r.anchoredPosition = Vector2.zero;
		return r;
	}

	private void SetBarHeight(float h)
	{
		_top.sizeDelta = new Vector2(0, h);
		_bottom.sizeDelta = new Vector2(0, h);
	}

	public IEnumerator Open(float duration, float targetHeight)
	{
		_blocker.gameObject.SetActive(true);
		yield return Animate(to: targetHeight, duration);
	}

	public IEnumerator Close(float duration)
	{
		yield return Animate(to: 0f, duration);
		_blocker.gameObject.SetActive(false);
	}

	private IEnumerator Animate(float to, float duration)
	{
		float from = _top.sizeDelta.y;
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			float h = Mathf.Lerp(from, to, t / duration);
			SetBarHeight(h);
			yield return null;
		}
		SetBarHeight(to);
	}

	/// <summary>
	/// 열기(오버슈트): current → overshoot → settle (같은 '속도' 유지)
	/// 예: 0 → 10 → 9
	/// </summary>
	public IEnumerator OpenOvershoot(float settleHeight, float overshootHeight, float pixelsPerSecond)
	{
		_blocker.gameObject.SetActive(true);
		float start = _top.sizeDelta.y; // 보통 0
		yield return AnimateBySpeed(start, overshootHeight, pixelsPerSecond);
		yield return AnimateBySpeed(overshootHeight, settleHeight, pixelsPerSecond);
	}

	/// <summary>
	/// 닫기(오버슈트): current(=settle) → overshoot → 0 (같은 '속도' 유지)
	/// 예: 9 → 10 → 0
	/// </summary>
	public IEnumerator CloseOvershoot(float settleHeight, float overshootHeight, float pixelsPerSecond)
	{
		float start = _top.sizeDelta.y; // 보통 settleHeight
		yield return AnimateBySpeed(start, overshootHeight, pixelsPerSecond);
		yield return AnimateBySpeed(overshootHeight, 0f, pixelsPerSecond);
		_blocker.gameObject.SetActive(false);
	}

	private IEnumerator AnimateBySpeed(float from, float to, float pixelsPerSecond)
	{
		pixelsPerSecond = Mathf.Max(1e-3f, pixelsPerSecond);
		float dist = Mathf.Abs(to - from);
		float duration = dist / pixelsPerSecond;

		if (duration <= 0f)
		{
			SetBarHeight(to);
			yield break;
		}

		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			float h = Mathf.Lerp(from, to, t / duration);
			SetBarHeight(h);
			yield return null;
		}
		SetBarHeight(to);
	}
}
