using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TwoKeyButton : KeyButton
{
    [SerializeField] protected KeyCode _secondKeyCode;
    [SerializeField] protected Image _secondImage;

    private bool _firstKeyPressed = false;
    private bool _secondKeyPressed = false;
    private float _firstKeyDownTime;
    private float _secondKeyDownTime;
    private float _allowableInterval = 3f / 60f; // 3������ (60������ ����)

    public override float Width
    {
        get
        {
            return base.Width + _secondImage.rectTransform.sizeDelta.x + 0.5f;
        }
    }

    public void Init(KeyCode keyCode, KeyCode secondKeyCode, Sprite sprite, Sprite sceondSprite)
    {
        base.Init(keyCode, sprite);
        _secondKeyCode = secondKeyCode;
        _secondImage.sprite = sceondSprite;
    }
    private void Update()
    {
        if (!_isActive) return;

        // ù ��° Ű �Է�
        if (!_firstKeyPressed && Input.GetKeyDown(_keyCode))
        {
            _firstKeyPressed = true;
            _firstKeyDownTime = Time.time;
        }
        // �� ��° Ű �Է�
        if (!_secondKeyPressed && Input.GetKeyDown(_secondKeyCode))
        {
            _secondKeyPressed = true;
            _secondKeyDownTime = Time.time;
        }

        // �� �� �������� �ð� �� üũ
        if (_firstKeyPressed && _secondKeyPressed)
        {
            float diff = Mathf.Abs(_firstKeyDownTime - _secondKeyDownTime);
            if (diff <= _allowableInterval)
            {
                OnkeyPressedEvent();
                Destroy(gameObject);
            }
            else
            {
                _firstKeyPressed = false;
                _secondKeyPressed = false;
            }
        }
        // �ٸ� Ű �߸� ���� ���
        else
        {
            for (KeyCode key = KeyCode.A; key <= KeyCode.Z; key++)
            {
                if ((key != _keyCode && key != _secondKeyCode) && Input.GetKeyDown(key))
                {
                    OnKeyMissedEvent();
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
}

