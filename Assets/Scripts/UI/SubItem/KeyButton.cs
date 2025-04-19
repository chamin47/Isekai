using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 자신의 키에대한 입력을 기다린다.
/// </summary>
public class KeyButton : UI_Base
{
    public event Action OnKeyPressed;
    public event Action OnKeyMissed;

    [SerializeField] protected KeyCode _keyCode;
    [SerializeField] protected Image _image;

    protected bool _canPressKey = false;
    protected bool _isActive = false;
    public virtual float Width
    {
        get
        {
            return _image.rectTransform.sizeDelta.x;
        }
    }

    public void Init(KeyCode keyCode, Sprite sprite)
    {
        _keyCode = keyCode;
        _image.sprite = sprite;
        _canPressKey = false;
        _isActive = false;
    }

    private void Update()
    {
        if (_isActive && Input.GetKeyDown(_keyCode)){
            Debug.Log($"Key Pressed : {_keyCode}");
            if (_canPressKey)
            {
                OnkeyPressedEvent();
                Destroy(gameObject);
            }
            else
            {
                OnKeyMissedEvent();
            }
        }
    }

    public void EnableKeyPress()
    {
        _canPressKey = true;
        _isActive = true;
    }

    public void DisableKeyPress()
    {
        _canPressKey = false;
        _isActive = false;
    }

    protected void OnkeyPressedEvent()
    {
        Managers.Sound.Play("i_mini_key_correct2", Sound.Effect);
        OnKeyPressed?.Invoke();
    }

    protected void OnKeyMissedEvent()
    {
        Managers.Sound.Play("i_mini_key_miss2", Sound.Effect);
        OnKeyMissed?.Invoke();
    }

    public override void Init()
    {
           
    }
}
