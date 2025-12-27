using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Timeline;

public class UI_Information : UI_Popup
{
    [SerializeField] private TextMeshProUGUI _yesText;
    [SerializeField] private TextMeshProUGUI _noText;

    private Color _initColor;

    [SerializeField] private bool _isYesFocused = false;
    [SerializeField] private bool _isNoFocused = false;
    [SerializeField] private bool _isKeyLocked = false;
    public event Action onYesEvent;

    private PlayerController _player;
    public PlayerController Player
    {
        get
        {
            if (_player == null)
                _player = FindObjectOfType<PlayerController>();
            return _player;
        }
    }

    private void OnEnable()
    {
        if(Player != null)
            Player.canMove = false;
    }
    private void OnDisable()
    {
        if (Player != null)
            Player.canMove = true;
    }
    public override void Init()
    {
        base.Init();

        _initColor = _yesText.color;

        // 마우스 이벤트 등록
        UI_EventHandler _yesTextEvent = _yesText.GetComponent<UI_EventHandler>();
        _yesTextEvent.OnPointerEnterHandler += (PointerEventData data) => { OnTextFocus(_yesText); OffTextFocus(_noText);
            _isYesFocused = true;
            _isNoFocused = false;
        };
        _yesTextEvent.OnPointerExitHandler += (PointerEventData data) => { OffTextFocus(_yesText); OffTextFocus(_noText);
            _isYesFocused = false;
            _isNoFocused = false;
        };
        _yesTextEvent.OnClickHandler += (PointerEventData data) => 
        {
            _isKeyLocked = true;
            //현실세계로 이동
            onYesEvent?.Invoke();
            Managers.UI.ClosePopupUI(this); 
        };

        UI_EventHandler _noTextEvent = _noText.GetComponent<UI_EventHandler>();
        _noTextEvent.OnPointerEnterHandler += (PointerEventData data) => { OnTextFocus(_noText); OffTextFocus(_yesText);
            _isNoFocused = true;
            _isYesFocused = false;
        };
        _noTextEvent.OnPointerExitHandler += (PointerEventData data) => { OffTextFocus(_noText);  OffTextFocus(_yesText);
            _isNoFocused = false;
            _isYesFocused = false;
        };
        _noTextEvent.OnClickHandler += (PointerEventData data) => { Managers.UI.ClosePopupUI(this); };
    }

    private void Update()
    {
        if(_isKeyLocked) return;
        // 키보드 이벤트 관리
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _isYesFocused = true;
            _isNoFocused = false;
            OnTextFocus(_yesText);
            OffTextFocus(_noText);
        }
        else if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            _isYesFocused = false;
            _isNoFocused = true;
            OnTextFocus(_noText);
            OffTextFocus(_yesText);
        }

        
        if(Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log("Enter Key Pressed");
            if (_isYesFocused)
            {
                _isKeyLocked = true;
                //현실세계로 이동
                onYesEvent?.Invoke();
                Managers.UI.ClosePopupUI(this);
            }
            else if(_isNoFocused)
            {
                Managers.UI.ClosePopupUI(this);
            }
        }
        
    }

    public void OnTextFocus(TextMeshProUGUI text)
    {
        text.color = Color.red;
    }

    public void OffTextFocus(TextMeshProUGUI text)
    {
        text.color = _initColor;
    }
}
