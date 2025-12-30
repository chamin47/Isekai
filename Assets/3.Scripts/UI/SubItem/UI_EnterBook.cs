using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EnterBook : UI_Base
{
    [SerializeField] private Animator _animater;
    [SerializeField] private RectTransform _uiRoot;
    private bool _canClick = false;
    private Vector2 _originAnchoredPos;

    [SerializeField] private  float _shakeDuration = 0.5f;
    [SerializeField] private float _shakeMagnitude = 5f;

    public override void Init()
    {
        DontDestroyOnLoad(gameObject);
        CloseBookEnter();
        _originAnchoredPos = _uiRoot.anchoredPosition;
    }
     
    private void Update()
    {
        if (_canClick == false) return;

        if(Input.anyKeyDown)
        {
            _canClick = false;
            StartCoroutine(CoOpenBookEnter());
        }
    }

    private IEnumerator CoOpenBookEnter()
    {
        Managers.Sound.Play("isekai_entry_door_knock", Sound.Effect);
        OpenBookEnter();
        yield return CoShakeUI();
        Managers.Scene.LoadScene(Scene.LoadingScene);
        yield return WaitForSecondsCache.Get(0.5f);
        Destroy(gameObject, 3f);
    }

    private IEnumerator CoShakeUI()
    {
        float elapsed = 0f;

        while (elapsed < _shakeDuration)
        {
            Vector2 offset = Random.insideUnitCircle * _shakeMagnitude;
            _uiRoot.anchoredPosition = _originAnchoredPos + offset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        _uiRoot.anchoredPosition = _originAnchoredPos;
    }

    [ContextMenu("Test")]
    public void Test()
    {
        StartCoroutine(CoShakeUI());
    }

    public void CloseBookEnter()
    {
        _animater.SetFloat("Speed", 1f);
        _animater.Play("EnterBook");
    }

    public void OpenBookEnter()
    {
        _animater.SetFloat("Speed", -1f);
        _animater.Play("EnterBook");
    }

    public void EnableClick()
    {
        _canClick = true;
    }
}
