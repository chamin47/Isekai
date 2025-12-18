using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class DreamSystem : MonoBehaviour
{
    public enum DreamState { Phase1, Phase2, Phase3, WakeUp }

    [SerializeField] private Animator _dreamAnimator;
    [SerializeField] private GameObject[] _masks;

    public DreamState currentState = DreamState.Phase1;

    [SerializeField] private int[] _skipClickThresholds = { 5, 7, 10 }; 

    [Header("Settings")]
    public float comboWindow = 0.5f;     // 연타 인정 시간
    public float idleTimeout = 3.0f;     // 이전 단계로 돌아가는 시간
    public float gameOverTimeout = 10f;  // 1단계에서 게임오버까지 시간

    [Header("Click Counts")]
    private int currentClicks = 0;
    private float lastClickTime;
    private float idleTimer;

    private bool _canStartSystem = false;

    private void Update()
    {
        if (_canStartSystem)
        {
            if (Input.GetMouseButtonDown(0))
            {
                ProcessClick();
            }

            CheckIdleTime();
            return;
        }

        if(_dreamAnimator.GetCurrentAnimatorStateInfo(0).IsName("dream3"))
        {
            _canStartSystem = true;
        }
    }

    public void ProcessClick()
    {
        float currentTime = Time.time;

        // 0.5초 이내 연타인지 확인
        if (currentTime - lastClickTime <= comboWindow)
        {
            currentClicks++;
        }
        else
        {
            currentClicks = 1; // 연타 끊기면 1부터 다시 시작
        }

        lastClickTime = currentTime;
        idleTimer = 0; // 클릭 시 타이머 초기화

        CheckStateTransition();
    }

    void CheckStateTransition()
    {
        if (currentState == DreamState.Phase1 && currentClicks >= _skipClickThresholds[0])
        {
            currentState = DreamState.Phase2;
            currentClicks = 0;
            UpdateDreamVisual();
        }
        else if (currentState == DreamState.Phase2 && currentClicks >= _skipClickThresholds[1])
        {
            currentState = DreamState.Phase3;
            currentClicks = 0;
            UpdateDreamVisual();
        }
        else if (currentState == DreamState.Phase3 && currentClicks >= _skipClickThresholds[2])
        {
            currentState = DreamState.WakeUp;
            currentClicks = 0;
            //TriggerWakeUp();
        }
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
            _masks[0].SetActive(true);
            _masks[1].SetActive(true);
        }
    }

    void CheckIdleTime()
    {
        idleTimer += Time.deltaTime;

        // 3초 동안 클릭이 없을 때
        if (idleTimer >= idleTimeout)
        {
            RegressionState();
            idleTimer = 0; // 다시 3초 카운트 시작
        }

        // 1단계에서 10초 방치 시 게임오버
        if (currentState == DreamState.Phase1 && idleTimer >= gameOverTimeout)
        {
            //TriggerGameOver();
        }
    }

    void RegressionState()
    {
        if (currentState == DreamState.Phase2) currentState = DreamState.Phase1;
        else if (currentState == DreamState.Phase3) currentState = DreamState.Phase2;
        // 1단계에서 3초 추가 방치 시 로직 추가 가능

        currentClicks = 0; // 단계가 내려가면 연타 수도 초기화
        UpdateDreamVisual();
    }

    public void StartSystem()
    {
        
    }
}
