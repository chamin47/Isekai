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
    protected bool _canInteract = true;
    protected bool _isInteracted = false;
    public bool CanInteract
    {
        get => _canInteract;
    }
    public bool IsInteracted
    {
        get => _isInteracted;
    }
    [SerializeField] private string _startID;
    public string StartID => _startID;

    protected Animator _animator;
    private SpriteRenderer _sprite;
    private ActorDirectorSimple _actorDirector;
    public ActorDirectorSimple ActorDirector => _actorDirector;

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


    public void LookTarget(Vector3 targetPosition)
    {
        _sprite.flipX = targetPosition.x > transform.position.x;
    }

    protected abstract void UpdateAnimation();
    public virtual void ShowDialogue()
    {

    }
    public virtual void OnEventEnd()
    {

    }
}
