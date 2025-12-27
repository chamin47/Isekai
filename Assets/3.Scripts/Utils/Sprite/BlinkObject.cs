using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkObject : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private float _blinkInterval = 0.5f;
    [SerializeField] private int _maxBlinkTime = 1000;

    private int _blinkCount = 0;
    private float _timer = 0f;

    public bool StopBlink { get; set; } = false;

    private void Update()
    {
        if(StopBlink)
        {
            _sprite.enabled = true;
            return;
        }

        _timer += Time.deltaTime;

        if (_timer >= _blinkInterval)
        {
            _sprite.enabled = !_sprite.enabled;
            _timer = 0f;
            _blinkCount++;
        }

        if(_blinkCount >= _maxBlinkTime)
        {
            StopBlink = true;
            _sprite.enabled = true;
        }
    }
}
