using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantController : NPCController
{
    [SerializeField] private Transform dialoguePos;
    [SerializeField] private UI_DialogueBalloon _dialogueBalloon;

    private void Start()
    {
        _dialogueBalloon.Init(dialoguePos, "{fade}그 분 덕분에 이 영지가 얼마나 평화로운지!!");
        State = NPCState.Idle;
    }

    public override void ShowDialogue()
    {
        _dialogueBalloon.AppearAndFade("{fade}그 분 덕분에 이 영지가 얼마나 평화로운지!!");
    }


    protected override void UpdateAnimation()
    {
        int nextState = -1;
        switch (State)
        {
            case NPCState.Idle:
                nextState = AnimatorHash.IDLE;
                break;
            case NPCState.Event:
                _isInteracted = true;
                break;
            default:
                nextState = AnimatorHash.IDLE;
                break;
        }

        if (nextState != -1 && _prevState != nextState)
        {
            _animator.CrossFade(nextState, 0.1f);
        }

        _prevState = nextState;
    }

    public override void OnEventEnd()
    {
        State = NPCState.Idle;
    }
}
