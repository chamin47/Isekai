using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;



public class BlackHole : MonoBehaviour
{
    [SerializeField] private float _fadeDuration = 2f;
    private SpriteRenderer _sprite;
    public bool IsPlayerInside = false;

    private Coroutine _showCoroutine;
    private Coroutine _hideCoroutine;

    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
    }


    public void Show()
    {
        if (_hideCoroutine != null)
        {
            StopCoroutine(_hideCoroutine);
            _hideCoroutine = null;
        }

        if (this.gameObject.activeInHierarchy == false)
            return;

        _showCoroutine = StartCoroutine(_sprite.CoFadeOut(_fadeDuration));
    }

    public void Hide()
    {
        if (_showCoroutine != null)
        {
            StopCoroutine(_showCoroutine);
            _showCoroutine = null;
        }

        if (this.gameObject.activeInHierarchy == false)
            return;

        _hideCoroutine = StartCoroutine(_sprite.CoFadeIn(_fadeDuration));

    }
}
