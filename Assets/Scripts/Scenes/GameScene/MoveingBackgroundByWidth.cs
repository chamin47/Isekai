using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveingBackgroundByWidth : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private FlowCamera _flowCamera;

    [SerializeField] float _moveSpeed = 0.1f;
    void Start()
    {
        if(_flowCamera != null)
            _flowCamera.OnCameraMove += MoveBackground;

        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void MoveBackground(Vector2 dir, float moveSpeed)
    {
        Vector2 current = _spriteRenderer.size;
        current.x += moveSpeed * dir.x * Time.deltaTime * _moveSpeed;
        _spriteRenderer.size = current;
    }
}
