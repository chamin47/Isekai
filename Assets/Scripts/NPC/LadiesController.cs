using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadiesController : NPCController
{
    [SerializeField] private Transform[] dialoguePos = new Transform[3];
    [SerializeField] private UI_DialogueBalloon _dialogueBalloon;
    private void Start()
    {
        State = NPCState.Idle;
    }

    public override void ShowDialogue()
    {
        
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
