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
