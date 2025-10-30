using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadiesController : NPCController
{
    [SerializeField] private Transform[] dialoguePos = new Transform[3];
    [SerializeField] private UI_DialogueBalloon _dialogueBalloon;
    [SerializeField] private List<SpriteRenderer> _ladiesSprites = new List<SpriteRenderer>();
    [SerializeField] private List<Animator> _ladiesAnimators = new List<Animator>();
    [SerializeField] private List<string> _dialogues = new List<string>();
    [SerializeField] private Click_Lady click;

    private int _selectedLadyIndex = -1;
    public int SelectedLadyIndex => _selectedLadyIndex;
    private void Start()
    {
        State = NPCState.Idle;
        click.OnClicked += OnLadySelected;
    }

    private void OnDestroy()
    {
        click.OnClicked -= OnLadySelected;
    }

    public override void ShowDialogue()
    {
        for(int i = 0; i < _ladiesSprites.Count; i++)
        {
            if (_ladiesSprites[i] != null && _ladiesSprites[i].gameObject.activeInHierarchy)
            {
                _dialogueBalloon.Init(dialoguePos[i], _dialogues[i]);
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

    private float _fadeDuration = 2.0f;
    // Fade �Ǹ鼭 ���ֵ��� ������� �Ѵ�
    private IEnumerator DisappearLady()
    {
        float elapsedTime = 0f;

        List<Color> startColors = new List<Color>();
        for (int i = 0; i < _ladiesSprites.Count; i++)
        {
            if (_ladiesSprites[i] != null)
                startColors.Add(_ladiesSprites[i].color);
            else
                startColors.Add(Color.clear); // null�� ��� ���� ������
        }

        while (elapsedTime < _fadeDuration)
        {
            elapsedTime += Time.deltaTime;
  
            float t = Mathf.Clamp01(elapsedTime / _fadeDuration);

            for (int i = 0; i < _ladiesSprites.Count; i++)
            {
                // ���õ� ���ִ� �ǳʶٱ�
                if (i == _selectedLadyIndex)
                    continue;

                // null�̰ų� ��Ȱ��ȭ�� ���ִ� �ǳʶٱ�
                if (_ladiesSprites[i] == null || !_ladiesSprites[i].gameObject.activeInHierarchy)
                    continue;

                // 3. Lerp�� ����� ���� ���Ŀ��� 0���� �ε巴�� ����
                float startAlpha = startColors[i].a;
                float currentAlpha = Mathf.Lerp(startAlpha, 0f, t);

                // ���� ����(RGB)�� �����ϰ� ����(A) ���� ����
                Color newColor = startColors[i];
                newColor.a = currentAlpha;
                _ladiesSprites[i].color = newColor;
            }

            yield return null; // ���� �����ӱ��� ���
        }

        // 4. ���̵� �Ϸ� �� Ȯ���ϰ� ��Ȱ��ȭ
        for (int i = 0; i < _ladiesSprites.Count; i++)
        {
            if (i == _selectedLadyIndex)
                continue;

            if (_ladiesSprites[i] != null)
            {
                // ���� �� 0���� Ȯ��
                Color finalColor = _ladiesSprites[i].color;
                finalColor.a = 0f;
                _ladiesSprites[i].color = finalColor;

                // ���� ������Ʈ ��Ȱ��ȭ
                _ladiesSprites[i].gameObject.SetActive(false);
            }
        }
    }

    private void OnLadySelected(int index)
    {
        _selectedLadyIndex = index;
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
                _isInteracted = true;
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

    public override void OnEventEnd()
    {
        State = NPCState.Idle;
    }
}
