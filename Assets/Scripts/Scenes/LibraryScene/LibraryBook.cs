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
	[SerializeField] private SpriteRenderer _mouseRenderer;
    [SerializeField] private BoxCollider2D _collider;
	[SerializeField] private bool _isClicked = false;           // Ŭ���Ǿ����� ����
    [SerializeField] private float _fingerBlinkSpeed = 0.8f;    // �հ��� ���ڰŸ� ���ð�

    // �ʱ� å ���� ����
    public void Init()
    {
        _mouseRenderer.enabled = false;
    }

    public void StartFingerBlink()
    {
        StartCoroutine(CoFingerBlink());
    }

    public void StopFingerBlink()
    {
        StopAllCoroutines();
        _mouseRenderer.enabled = false;
    }

    // å Ŭ�� �ʱ�ȭ
    public void SetCanClicked()
	{
        _isClicked = false;
        _collider.enabled = true;
    }

    // �հ��� ���ڰŸ�
    private IEnumerator CoFingerBlink()
    {
        while (true)
        {
            _mouseRenderer.enabled = !_mouseRenderer.enabled;

            yield return WaitForSecondsCache.Get(_fingerBlinkSpeed);
        }
    }

    public void OnMouseDown()
    {
        if (!_isClicked)
        {
            var ui = Managers.UI.MakeWorldSpaceUI<UI_BookSelectWorldSpace>();
            ui.Init(this);

            StopFingerBlink();
            _isClicked = true;
        }
    }

    // �ʱ�ȭ
    private void Reset()
    {
        _collider = GetComponent<BoxCollider2D>();
        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                _mouseRenderer = sr;
                break;
            }
        }
    }
}
