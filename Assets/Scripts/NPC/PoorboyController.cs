using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoorboyController : NPCController
{
    [SerializeField] private Transform dialoguePos;
    [SerializeField] private UI_DialogueBalloon _dialogueBalloon;

    private void Start()
    {
        _dialogueBalloon.Init(dialoguePos, "{fade}저도 언젠가 꼭 공작님처럼 훌륭한 어른이 될 거예요…!!");
        _dialogueBalloon.gameObject.SetActive(false);
        State = NPCState.Idle;
    }

    public override void ShowDialogue()
    {
        _dialogueBalloon.AppearAndFade("{fade}저도 언젠가 꼭 공작님처럼 훌륭한 어른이 될 거예요…!!");
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
    }
}
