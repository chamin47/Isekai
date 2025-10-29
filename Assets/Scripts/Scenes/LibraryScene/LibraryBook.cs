using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// å hover�� �̺�Ʈ
/// å Ŭ������ �̺�Ʈ ����
/// </summary>
public class LibraryBook : MonoBehaviour
{
	[SerializeField] private GameObject _mouse;
    [SerializeField] private BoxCollider2D _collider;
	[SerializeField] private bool _isClicked = false;           // Ŭ���Ǿ����� ����
    [SerializeField] private float _fingerBlinkSpeed = 0.8f;    // �հ��� ���ڰŸ� ���ð�
	[SerializeField] private float _fadeDuration = 0.5f;   // ���̵��� ���� �ð�

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

    // å Ŭ�� �ʱ�ȭ
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

	// �հ��� ���ڰŸ�
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
