using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScholarController : NPCController
{
    [SerializeField] private Transform dialoguePos;
    [SerializeField] private UI_DialogueBalloon _dialogueBalloon;

    private void Start()
    {
        _dialogueBalloon.Init(dialoguePos, "{fade}역시!! 미천한 제 뇌로는 이해하지 못하겠네요.");
        State = NPCState.Idle;
    }

    public override void ShowDialogue()
    {
        _dialogueBalloon.AppearAndFade("{fade}역시!! 미천한 제 뇌로는 이해하지 못하겠네요.");
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
