using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtistController : NPCController
{
    private void Start()
    {
        State = NPCState.Idle;
    }

    protected override void UpdateAnimation()
    {
        int nextState = -1;
        switch (State)
        {
            case NPCState.Idle:
                nextState = AnimatorHash.IDLE;
                break;
            case NPCState.Move:
                nextState = AnimatorHash.MOVE;
                break;
            case NPCState.Event:
                
                break;
        }

        if (_prevState != nextState)
        {
            _animator.CrossFade(nextState, 0.1f);
        }

        _prevState = nextState;
    }
}
