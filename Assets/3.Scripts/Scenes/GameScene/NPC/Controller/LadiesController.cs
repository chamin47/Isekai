using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadiesController : NPCController
{
    [SerializeField] private Transform[] dialoguePos = new Transform[3];
    [SerializeField] private UI_DialogueBalloon _dialogueBalloon;
    [SerializeField] private List<SpriteRenderer> _ladiesSprites = new List<SpriteRenderer>();
    [SerializeField] private List<GameObject> _ladiesHearts = new List<GameObject>();
    [SerializeField] private List<Animator> _ladiesAnimators = new List<Animator>();
    [SerializeField] private List<string> _dialogues = new List<string>();
    [SerializeField] private Click_Lady click;

    private int _selectedLadyIndex = -1;
    public int SelectedLadyIndex => _selectedLadyIndex;
    private void Start()
    {
        State = NPCState.Idle;
        click.OnClicked += OnLadySelected;
        _dialogueBalloon.Init(dialoguePos[0], _dialogues[0]);
        _dialogueBalloon.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        click.OnClicked -= OnLadySelected;
    }

    public override void ShowDialogue()
    {
        for(int i = 0; i < _ladiesSprites.Count; i++)
        {
            if(_selectedLadyIndex != -1 && i != _selectedLadyIndex)
            {
                continue;
            }

            if (_ladiesSprites[i] != null && _ladiesSprites[i].gameObject.activeInHierarchy)
            {
                _dialogueBalloon.AppearAndFade(_dialogues[i]);
                return;
            }
        }
    }

    public override void LookTarget(Vector3 targetPosition)
    {
        for(int i = 0; i < _ladiesSprites.Count; i++)
        {
            if (_ladiesSprites[i] == null || !_ladiesSprites[i].gameObject.activeInHierarchy)
                continue;                

            if (targetPosition.x > transform.position.x)
                _ladiesSprites[i].flipX = true;
            else
                _ladiesSprites[i].flipX = false;

            if(i == 0)
            {
                _ladiesSprites[i].flipX = !_ladiesSprites[i].flipX;
            }
        }
    }

    private void OnLadySelected(int index)
    {
        _selectedLadyIndex = index;
        _dialogueBalloon.Init(dialoguePos[index], _dialogues[index]);
        _ladiesHearts[index].SetActive(true);
    }

    public IEnumerator ClickLady(System.Action<int> onClicked)
    {
        yield return click.ClickLady(onClicked);
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
            for(int i = 0; i < _ladiesAnimators.Count; i++)
            {
                if (_ladiesAnimators[i] != null && _ladiesAnimators[i].gameObject.activeInHierarchy)
                    _ladiesAnimators[i].CrossFade(nextState, 0.1f);
            }
        }

        _prevState = nextState;
    }

    public override void OnEventEnd(int eventParam)
    {
        base.OnEventEnd(eventParam);
        State = NPCState.Idle;
        if(eventParam >= 1000)
        {
            _isInteracted = false;
            _startID = eventParam.ToString();
            for(int i = 0; i < _ladiesSprites.Count; i++)
            {
                _ladiesSprites[i].flipX = false;
            }
        }
    }
}
