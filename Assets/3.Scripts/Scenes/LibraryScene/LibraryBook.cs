using System.Collections;
using UnityEngine;

/// <summary>
/// 책 hover시 이벤트
/// 책 클릭유도 이벤트 정의
/// </summary>
public class LibraryBook : MonoBehaviour
{
	[SerializeField] private GameObject _mouse;
	[SerializeField] private BoxCollider2D _collider;
	[SerializeField] private SpriteRenderer _twinkleRenderer;
	[SerializeField] private GameObject _hud;
	[SerializeField] private WorldType _worldType;
	[SerializeField] private bool _isClicked = false;           // 클릭되었는지 여부
	[SerializeField] private float _fingerBlinkSpeed = 0.8f;    // 손가락 깜박거림 대기시간
	[SerializeField] private float _fadeDuration = 0.5f;   // 페이드인 지속 시간
	[SerializeField] private float _dimAlpha = 0.35f;

	private SpriteRenderer _bookRenderer;
	private SpriteRenderer _mouseRenderer;

	private void Awake()
	{
		if (_mouse != null)
			_mouseRenderer = _mouse.GetComponent<SpriteRenderer>();

		_bookRenderer = GetComponent<SpriteRenderer>();
	}

	public void StartFingerBlink()
	{
		StartCoroutine(CoFingerBlink());
	}

	public void StopFingerBlink()
	{
		StopAllCoroutines();
		_mouse.SetActive(false);
	}

	// 책 클릭 초기화
	public void EnableClick()
	{
		_isClicked = false;
		_collider.enabled = true;
	}

	public void EnableFinger()
	{
		_mouse.SetActive(true);
	}

	public void DisableClick()
	{
		_collider.enabled = false;
	}

	public void DisableFinger()
	{
		_mouse.SetActive(false);
	}

	// 손가락 깜박거림
	private IEnumerator CoFingerBlink()
	{
		while (true)
		{
			if (_mouse.activeSelf)
			{
				_mouse.SetActive(false);
			}
			else
			{
				_mouse.SetActive(true);
			}

			yield return WaitForSecondsCache.Get(_fingerBlinkSpeed);
		}
	}

	public void SetHighlight(bool highlight)
	{
		if (highlight)
		{
			SetAlpha(1f);
		}
		else
		{
			SetAlpha(_dimAlpha);
			DisableTwincle();
		}
	}

	private void SetAlpha(float alpha)
	{
		Color bookColor = _bookRenderer.color;
		bookColor.a = alpha;
		_bookRenderer.color = bookColor;
	}

	private void DisableTwincle()
	{
		_twinkleRenderer.gameObject.SetActive(false);
	}

	public IEnumerator FadeInMouse()
	{
		if (_mouseRenderer == null)
			yield break;

		_mouse.SetActive(true);
		Managers.Sound.Play("library_mouse", Sound.Effect);

		Color color = _mouseRenderer.color;
		color.a = 0f;
		_mouseRenderer.color = color;

		float t = 0f;
		while (t < _fadeDuration)
		{
			t += Time.deltaTime;
			float alpha = Mathf.Clamp01(t / _fadeDuration);
			color.a = alpha;
			_mouseRenderer.color = color;
			yield return null;
		}

		color.a = 1f;
		_mouseRenderer.color = color;
	}

	public void OnMouseDown()
	{
		if (!IsUnlocked())
		{
			if (UI_LockedBookNotice.IsShowing)
				return;

			var ui = Managers.UI.ShowPopupUI<UI_LockedBookNotice>();
			ui.Init();
			return;
		}

		if (!_isClicked)
		{
			_isClicked = true;

			_hud.SetActive(false);
			var ui = Managers.UI.ShowPopupUI<UI_BookPopup>();
			ui.Init(this, _hud);

			DisableFinger();
		}
	}

	private bool IsUnlocked()
	{
		Debug.Log(Managers.World.CurrentWorldType);
		return _worldType == Managers.World.CurrentWorldType;
	}
}
