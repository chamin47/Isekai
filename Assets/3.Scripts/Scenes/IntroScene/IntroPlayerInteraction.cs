using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private Animator _ani;
    private bool _active = false;
    [SerializeField] private float _moveDuration = 3f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_active)
            return;

        if(collision.CompareTag("CurScene"))
        {
            _active = true;

            StartCoroutine(ProcessCutScene());
            
            _playerController.canMove = false;
        }
    }

    private IEnumerator ProcessCutScene()
    {
        yield return MoveTo(); 
        GameObject go = new GameObject { name = "CutSceneDirector" };
        go.AddComponent<CutSceneDirector>();
    }

    private IEnumerator MoveTo()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = new Vector3(_targetTransform.position.x, startPosition.y, startPosition.z);
        _ani.SetBool("CutScene", true);
        _ani.Play("CutSceneWalk");
        float elaspedTime = 0f;
        float footstepInterval = 0.71f;
        while (elaspedTime < _moveDuration)
        {
            // 일정 시간 마다 발자국 소리 재생
            if (elaspedTime > 0.002f && elaspedTime % footstepInterval < Time.deltaTime)
            {
                _playerController.PlayFootSound();
            }
            elaspedTime += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, targetPosition, elaspedTime / _moveDuration);
            yield return null;
        }

        _ani.SetBool("CutScene", false);
        transform.position = targetPosition;


        yield return null;
    }
}
