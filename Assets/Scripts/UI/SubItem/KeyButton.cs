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
        _isActive = false;
    }

    private void Update()
    {
        if (_isActive && Input.GetKeyDown(_keyCode)){
            Debug.Log($"Key Pressed : {_keyCode}");
            OnkeyPressedEvent();
            Destroy(gameObject);
        }
        else if (_isActive)
        {
            for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
            {
                if (key != _keyCode && Input.GetKeyDown(key))
                {
                    Debug.Log($"Key Missed : {key}, Correct Key : {_keyCode}");
                    OnKeyMissedEvent();
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }

    public void EnableKeyPress()
    { 
        _isActive = true;
    }

    public void DisableKeyPress()
    {
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
