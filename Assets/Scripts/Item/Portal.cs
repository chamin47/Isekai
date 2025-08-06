using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _fadeTime = 1f;

    private UI_Information _information;

    public event System.Func<IEnumerator> onEnterEvent;

    private Scene _nextScene = Scene.LoadingScene;

    private void Start()
    {
        FadeIn();
    }

    public void FadeIn()
    {
        StartCoroutine(_spriteRenderer.CoFadeOut(_fadeTime));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_information != null) return;

        if (collision.CompareTag("Player"))
        {
            _information = Managers.UI.ShowPopupUI<UI_Information>();
            _information.onYesEvent += OnEnterEvent;
        }
    }

    private void OnEnterEvent()
    {
        Managers.Scene.LoadScene(_nextScene);
        //StartCoroutine(CoOnEnterEvent());
    }

    // ���� �̺�Ʈ�� �߻������� ���� �������� �̺�Ʈ�� ������ ������ �� �̵�
    private IEnumerator CoOnEnterEvent()
    {
        if (onEnterEvent != null)
        {
            Delegate[] delegates = onEnterEvent.GetInvocationList();
            int count = delegates.Length;

            List<bool> isDoneList = new List<bool>();

            for(int i = 0; i < count; i++)
            {
                isDoneList.Add(false);

                int index = i;

                StartCoroutine(RunCoroutine((Func<IEnumerator>)delegates[i], () => 
                {
                    isDoneList[index] = true; 
                }));
            }

            while(isDoneList.Exists(done => done == false))
            {
                yield return null;
            }
        }

        // ��� �ڷ�ƾ�� ���� ��
        Managers.Scene.LoadScene(_nextScene);
    }

    private IEnumerator RunCoroutine(Func<IEnumerator> coroutineFunc, Action onComplete)
    {
        yield return coroutineFunc();
        onComplete?.Invoke();
    }

    public void SetPortalPosition(Scene targetScene)
    {
        _nextScene = targetScene;
    }
}
