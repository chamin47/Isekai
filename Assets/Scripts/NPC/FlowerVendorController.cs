using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerVendorController : NPCController
{
    [Tooltip("패트롤할 지점을 넣어준다")]
    [SerializeField] private Transform[] _patrolPoints;
    [SerializeField] private float _patrolSpeed = 2f;

    private int _currentPatrolTargetIndex = 0;
    public bool CanMove { get => _canMove; set => _canMove = value; }
    [SerializeField] private bool _canMove = true;

    [SerializeField] private Transform dialoguePos;
    [SerializeField] private UI_DialogueBalloon _dialogueBalloon;

    private Coroutine _showDialogue; 
    private void Update()
    {
        if (_canMove == false || State == NPCState.Event) return;

        Patrol();
    }

    private void Start()
    {
        _dialogueBalloon.Init(dialoguePos, "<bounce a=0.2>꽃 사세요~!</>");
        _showDialogue = StartCoroutine(CoShowDialogue());
    }

    private IEnumerator CoShowDialogue()
    {
        while(true)
        {
            yield return new WaitForSeconds(2f);
            _dialogueBalloon.Appear("<bounce a=0.2>꽃 사세요~!</>");
            yield return new WaitForSeconds(3f);
            _dialogueBalloon.Disappear();
        }
    }

    public void Patrol()
    {
        Transform targetPoint = _patrolPoints[_currentPatrolTargetIndex];

        // 이동 
        State = NPCState.Move;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            _patrolSpeed * Time.deltaTime
            );
        
        LookTarget(targetPoint.position);

        if (Vector3.SqrMagnitude(transform.position - targetPoint.position) < 0.1f)
        {
            _currentPatrolTargetIndex = (_currentPatrolTargetIndex + 1) % _patrolPoints.Length;
        }
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
                _canInteract = false;
                if (_showDialogue != null)
                    StopCoroutine(_showDialogue);
                _dialogueBalloon.Disappear();
                break;
        }

        if(nextState != -1 && _prevState != nextState)
        {
            _animator.CrossFade(nextState, 0.1f);
        }

        _prevState = nextState;
    }

    public override void OnEventEnd()
    {
        State = NPCState.Idle;
        _showDialogue = StartCoroutine(CoShowDialogue());
    }
}
