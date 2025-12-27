using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkSprite : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _sprite;
    private int Length => _sprite.Length;
    private int _currentIndex = 0;

    [SerializeField] private float _blinkInterval = 0.5f;
    private float _timer = 0f;
    public bool StopBlinking { get; set; } = false;

    public void Update()
    {
        if(StopBlinking)
        {
            return;
        }

        _timer += Time.deltaTime;

        if(_timer >= _blinkInterval)
        {
            _timer = 0f;
            _sprite[_currentIndex].enabled = false;
            _currentIndex = (_currentIndex + 1) % Length;
            _sprite[_currentIndex].enabled = true;
        }
    }

}
