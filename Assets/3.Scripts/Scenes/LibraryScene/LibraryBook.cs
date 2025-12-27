using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 책 hover시 이벤트
/// 책 클릭유도 이벤트 정의
/// </summary>
public class LibraryBook : MonoBehaviour
{
	[SerializeField] private GameObject _mouse;
    [SerializeField] private BoxCollider2D _collider;
	[SerializeField] private bool _isClicked = false;           // 클릭되었는지 여부
    [SerializeField] private float _fingerBlinkSpeed = 0.8f;    // 손가락 깜박거림 대기시간
	[SerializeField] private float _fadeDuration = 0.5f;   // 페이드인 지속 시간

	private SpriteRenderer _mouseRenderer;

	private void Awake()
	{
		if (_mouse != null)
			_mouseRenderer = _mouse.GetComponent<SpriteRenderer>();
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

	public void DisableFinger()
	{
		_mouse.SetActive(false);
	}

	// 손가락 깜박거림
	private IEnumerator CoFingerBlink()
    {
        while (true)
        {
            if(_mouse.activeSelf)
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
        if (!_isClicked)
        {
            _isClicked = true;

            var ui = Managers.UI.MakeWorldSpaceUI<UI_BookSelectWorldSpace>();
            ui.Init(this);

            DisableFinger();
		}
    }
}
