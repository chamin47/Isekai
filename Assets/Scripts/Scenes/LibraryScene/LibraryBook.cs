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

    public void OnMouseDown()
    {
        if (!_isClicked)
        {
            _isClicked = true;

            var ui = Managers.UI.MakeWorldSpaceUI<UI_BookSelectWorldSpace>();
            ui.Init(this);

            StopFingerBlink();
        }
    }
}
