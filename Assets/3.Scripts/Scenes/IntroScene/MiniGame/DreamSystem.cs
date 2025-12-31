using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DreamSystem : MonoBehaviour, IPointerClickHandler
{
    public enum DreamState { Phase1, Phase2, Phase3, WakeUp }
    
    [SerializeField] private Animator _dreamAnimator;
    [SerializeField] private SpriteRenderer _dreamRenderer;
    [SerializeField] private GameObject[] _masks;
    [SerializeField] private GameObject _mouse;
    [SerializeField] private Animator _bedAnimator;
    [SerializeField] private CanvasGroup _gameOverUI;
    [SerializeField] private GameObject _player;
    [SerializeField] private Image _fadeUI;
    [SerializeField] private BlinkObject _a;
    [SerializeField] private BlinkObject _b;

    public DreamState currentState = DreamState.Phase1;

    [SerializeField] private int[] _skipClickThresholds = { 5, 7, 10 }; 

    [Header("Settings")]
    public float comboWindow = 0.5f;     // 연타 인정 시간
    public float idleTimeout = 3.0f;     // 이전 단계로 돌아가는 시간
    public float gameOverTimeout = 10f;  // 1단계에서 게임오버까지 시간

    private int _currentClicks = 0;
    private float _lastClickTime;
    private float _idleTimer;

    private bool _doAction = false;
    private bool _canStartSystem = false;
    private bool _canStopSystem = false;

    private void Update()
    {
        if(_canStopSystem)
        {
            return;
        }

        if (_canStartSystem)
        {
            CheckIdleTime();
            return;
        }

        if(_dreamAnimator.GetCurrentAnimatorStateInfo(0).IsName("dream3"))
        {
            Managers.Sound.PlaySubEffect("dream_sink", 1f);
            _canStartSystem = true;
            _mouse.SetActive(true);
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if(_canStopSystem)
        {
            return;
        }

        if (_canStartSystem)
        {
            ProcessClick();
        }
    }

    public void ProcessClick()
    {
        float currentTime = Time.time;

        // 0.5초 이내 연타인지 확인
        if (currentTime - _lastClickTime <= comboWindow)
        {
            Managers.Sound.Play("dream_wake_up", Sound.Effect);
            _currentClicks++;
        }
        else
        {
            _currentClicks = 1; // 연타 끊기면 1부터 다시 시작
        }

        _lastClickTime = currentTime;
        _idleTimer = 0; // 클릭 시 타이머 초기화

        CheckStateTransition();
    }

    void CheckStateTransition()
    {
        if (currentState == DreamState.Phase1 && _currentClicks >= _skipClickThresholds[0])
        {
            _doAction = true;
            currentState = DreamState.Phase2;
            _currentClicks = 0;
            _mouse.SetActive(false);
            UpdateDreamVisual();
        }
        else if (currentState == DreamState.Phase2 && _currentClicks >= _skipClickThresholds[1])
        {
            currentState = DreamState.Phase3;
            _currentClicks = 0;
            UpdateDreamVisual();
        }
        else if (currentState == DreamState.Phase3 && _currentClicks >= _skipClickThresholds[2])
        {
            currentState = DreamState.WakeUp;
            _currentClicks = 0;
            TriggerWakeUp();
        }
    }

    [ContextMenu("Trigger Wake Up")]
    public void TriggerWakeUp()
    {
        _masks[0].SetActive(false);
        _masks[1].SetActive(false);
        _dreamAnimator.gameObject.SetActive(false);
        _canStopSystem = true;
        _bedAnimator.enabled = true;
        Managers.Sound.StopSubEffect();
        StartCoroutine(CoWakeUp());
    }

    private IEnumerator CoWakeUp()
    {
        yield return WaitForSecondsCache.Get(1f);
        _bedAnimator.gameObject.SetActive(false);        
        _player.SetActive(true);
        _a.gameObject.SetActive(true);
        _b.gameObject.SetActive(true);
        Managers.Sound.Play("dream_wake_up", Sound.Effect);
    }

    private void UpdateDreamVisual()
    {
        if(currentState == DreamState.Phase1)
        {
            _masks[0].SetActive(false);
            _masks[1].SetActive(false);
        }
        else if(currentState == DreamState.Phase2)
        {
            _masks[0].SetActive(true);
            _masks[1].SetActive(false);
        }
        else if(currentState == DreamState.Phase3)
        {
            _masks[0].SetActive(false);
            _masks[1].SetActive(true);
        }
    }

    void CheckIdleTime()
    {
        _idleTimer += Time.deltaTime;

        // 3초 동안 클릭이 없을 때
        if (_doAction && currentState != DreamState.Phase1 && _idleTimer >= idleTimeout)
        {
            RegressionState();
            _idleTimer = 0; // 다시 3초 카운트 시작
        }

        // 1단계에서 10초 방치 시 게임오버
        if (_doAction && currentState == DreamState.Phase1 && _idleTimer >= gameOverTimeout)
        {
            TriggerGameOver();
        }
    }

    private void TriggerGameOver()
    {
        Debug.Log("Game Over Triggered");
        _canStopSystem = true;
        Managers.Sound.StopSubEffect();
        StartCoroutine(CoGameOver());
    }

    private IEnumerator CoGameOver()
    {
        _masks[0].SetActive(false);
        _masks[1].SetActive(false);
        yield return WaitForSecondsCache.Get(1f);
        _dreamAnimator.gameObject.SetActive(false);
        Managers.Sound.Play("dream_failure", Sound.Effect);
        _gameOverUI.gameObject.SetActive(true);
        yield return _gameOverUI.CoFadeOut(1f);
    }

    void RegressionState()
    {
        Managers.Sound.Play("dream_deepen1", Sound.Effect);

        if (currentState == DreamState.Phase2) currentState = DreamState.Phase1;
        else if (currentState == DreamState.Phase3) currentState = DreamState.Phase2;
        // 1단계에서 3초 추가 방치 시 로직 추가 가능

        _currentClicks = 0; // 단계가 내려가면 연타 수도 초기화
        UpdateDreamVisual();
    }

    public IEnumerator StartSystem()
    {
        _canStopSystem = true;
        _fadeUI.gameObject.SetActive(true);
        yield return _fadeUI.CoFadeIn(3f);
        yield return WaitForSecondsCache.Get(2f);
        yield return _dreamRenderer.CoFadeOut(2f);
        _dreamAnimator.SetBool("StartDream", true);
        _canStopSystem = false;
        _fadeUI.gameObject.SetActive(false);
    }

}
