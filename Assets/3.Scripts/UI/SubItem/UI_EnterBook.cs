using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_EnterBook : UI_Base
{
    [SerializeField] private Animator _animater;
    [SerializeField] private RectTransform _uiRoot;
    private bool _canClick = false;
    private Vector2 _originAnchoredPos;

    [SerializeField] private  float _shakeDuration = 0.15f;
    [SerializeField] private float _shakeMagnitude = 3f;

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
        yield return CoShakeUI();
        Managers.Sound.Play("isekai_entry_door_knock", Sound.Effect);
        yield return CoShakeUI();
        Managers.Sound.Play("isekai_entry_door_knock", Sound.Effect);
        yield return CoShakeUI();
        yield return WaitForSecondsCache.Get(1f);
        Managers.Scene.LoadScene(Scene.LoadingScene);
        OpenBookEnter();
        yield return WaitForSecondsCache.Get(0.5f);
        Managers.Sound.Play("isekai_entry_warp", Sound.Effect);
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
        Managers.Sound.Play("isekai_entry_door_close", Sound.Effect);
        _animater.Play("EnterBook");
    }

    public void OpenBookEnter()
    {
        Managers.Sound.Play("isekai_entry_door_open", Sound.Effect);
        _animater.Play("REnterBook");
    }

    public void EnableClick()
    {
        _canClick = true;
    }
}
