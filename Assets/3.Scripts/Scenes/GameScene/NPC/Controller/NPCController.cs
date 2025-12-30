using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NPCState
{
    Idle,
    Move,
    Event
}

public static class AnimatorHash
{
    public static int IDLE = Animator.StringToHash("Idle");
    public static int MOVE = Animator.StringToHash("Move");
}

public abstract class NPCController : MonoBehaviour
{
    protected int _prevState = -1;
    private NPCState _state = NPCState.Idle;
    protected bool _canInteract = true; // 상호작용이 가능한지 여부
    protected bool _isInteracted = false; // 상호작용을 완료 했는지 여부
    public bool CanInteract
    {
        get => _canInteract;
    }
    public bool IsInteracted
    {
        get => _isInteracted;
    }
    [SerializeField] protected string _startID;
    public string StartID => _startID;

    protected Animator _animator;
    private SpriteRenderer _sprite;
    private ActorDirectorSimple _actorDirector;
    public ActorDirectorSimple ActorDirector => _actorDirector;

    public Transform LeftTalkPosition;
    public Transform RightTalkPostion;

    public Transform GetTalkPosition(Transform interactor)
    {
        if (LeftTalkPosition == null || RightTalkPostion == null)
            return null;

        var leftDistance = Vector3.SqrMagnitude(LeftTalkPosition.position - interactor.position);
        var rightDistance = Vector3.SqrMagnitude(RightTalkPostion.position - interactor.position);

        return leftDistance < rightDistance ? LeftTalkPosition : RightTalkPostion;
    }

    private void Awake()
    {
        if(_animator == null)
            _animator = GetComponentInChildren<Animator>();
        if(_sprite == null)
            _sprite = GetComponentInChildren<SpriteRenderer>();
        if(_actorDirector == null)
            _actorDirector = GetComponent<ActorDirectorSimple>();
    }

    public NPCState State
    {
        get => _state;
        set
        {
            _state = value;
            UpdateAnimation();
        }
    }


    public virtual void LookTarget(Vector3 targetPosition)
    {
        _sprite.flipX = targetPosition.x > transform.position.x;
    }

    protected abstract void UpdateAnimation();
    public abstract void ShowDialogue();
    
    public virtual void OnEventEnd(int eventParam)
    {
        _isInteracted = true;
    }
}
