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


    private void Update()
    {
        if (_canMove == false) return;

        Patrol();
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
                
                break;
        }

        if(_prevState != nextState)
        {
            _animator.CrossFade(nextState, 0.1f);
        }

        _prevState = nextState;
    }
}
