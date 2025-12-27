using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtistController : NPCController
{
    [SerializeField] private Transform dialoguePos;
    [SerializeField] private UI_DialogueBalloon _dialogueBalloon;
    private string _dialogue;
    private void Start()
    {
        State = NPCState.Idle;
        _dialogueBalloon.gameObject.SetActive(false);
    }

    List<int> a;
    public override void ShowDialogue()
    {
        _dialogueBalloon.AppearAndFade(_dialogue);
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

    public override void OnEventEnd(int eventParam)
    {
        base.OnEventEnd(eventParam);
        State = NPCState.Idle;

        if(eventParam == 0)
        {
            _dialogue = "그래도 역시 우리 공작님이 훨씬 더 눈부시네요";
            _dialogueBalloon.Init(dialoguePos, _dialogue);
        }
        else
        {
            _dialogue = "대대손손 보관해주세요오오옹~~";
            _dialogueBalloon.Init(dialoguePos, _dialogue);
        }
    }
}
