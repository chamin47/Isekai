using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DoorController : MonoBehaviour
{
	[SerializeField] private Animator _animator;
	[SerializeField] private OutlineSelectSprite _outlineSelectSprite;
    [SerializeField] private Image _fadeUI;
    [SerializeField] private PlayerController _player;
    [SerializeField] private Transform _targetPosition;
    [SerializeField] private Transform _playerTragetPosition;
    [SerializeField] private BoxCollider2D _changedCollider;
    [SerializeField] private IntroCameraController _introCameraController;
    [SerializeField] private GameObject _happyUI;

    private void Awake()
	{
		_animator = GetComponent<Animator>();
	}

    private void Start()
    {
        _outlineSelectSprite.OnSelected += MoveToIntro;
        if (HomeSystem.IsDoorOpen)
        {
            Debug.Log("Door is Open");
            _animator.CrossFade("door_Clip", 0.1f);
            _outlineSelectSprite.enabled = true;
        }
        else
        {
            Debug.Log("Door is Closed");
            _outlineSelectSprite.enabled = false;
        }
    }

    private void MoveToIntro(int index)
    {
        _outlineSelectSprite.enabled = false;
        StartCoroutine(FadeAndMove());
    }

    private IEnumerator FadeAndMove()
    {
        _player.canMove = false;
        _introCameraController.DisableCameraUpdate();
        _introCameraController.ChangeBoxBounds(_changedCollider);
        _fadeUI.gameObject.SetActive(true);
        yield return _fadeUI.CoFadeOut(2f);
        _introCameraController.MoveTo(_targetPosition.position);
        _player.transform.position = _playerTragetPosition.position;
        _player.transform.localScale = new Vector3(1, 1, 1);
        _happyUI.SetActive(true);
        yield return _fadeUI.CoFadeIn(2f);
        _fadeUI.gameObject.SetActive(false);
        //_introCameraController.EnableCameraUpdate();
        _player.canMove = true;
    }

    [ContextMenu("Open Door")]
    public void Open()
	{
		if(HomeSystem.IsDoorOpen)
			return;

        _outlineSelectSprite.enabled = true;
        _animator.CrossFade("door_Clip", 0.1f);
		HomeSystem.IsDoorOpen = true;
    }

	
}
