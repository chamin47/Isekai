using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtistController : NPCController
{
    [SerializeField] private Transform dialoguePos;
    [SerializeField] private UI_DialogueBalloon _dialogueBalloon;

    private void Start()
    {
        _dialogueBalloon.Init(dialoguePos, "대대손손 보관해주세요오오옹~~");
        State = NPCState.Idle;
    }

    public override void ShowDialogue()
    {
        _dialogueBalloon.AppearAndFade("대대손손 보관해주세요오오옹~~");
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
        base.OnEventEnd();
        State = NPCState.Idle;
    }
}
