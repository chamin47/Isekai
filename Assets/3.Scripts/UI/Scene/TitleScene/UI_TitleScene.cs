using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class UI_TitleScene : UI_Scene
{
	[Header("Text")]
	[SerializeField] private TMP_Text _dialogueText;       // 대사 텍스트

	[Header("Fade")]
	[SerializeField] private CanvasGroup _dialogueCanvasGroup;  // 페이드용
	[SerializeField] private float _fadeDuration = 1.0f;

	[Header("Input")]
	[SerializeField] private TMP_InputField _inputField;    // 유저 입력용 (장면2)
	[SerializeField] private CanvasGroup _inputCanvasGroup; // 장면2 UI용

    private string _lastComposition = "";
    private string _lastInputText = "";
    [SerializeField] private float _soundDelay = 0.075f; // 타이핑 사운드 딜레이
    private float _soundTimer = 0f;

    private readonly List<string> _dialogue = new List<string>
	{
		"이 세계에서 상처받은 모든 이들에게 묻습니다.",
		"이 세계에서 벗어나 이세계로 향한다면 \n우리는 과연 행복해질 수 있을까요?",
	};

	private readonly string _lastDialogue = "당신의 답변에 담긴 의미는 무엇일까요? \n그것을 찾기 위한 첫걸음이 시작됩니다.";

    public override void Init()
	{
		base.Init();

		_dialogueText.text = "";
		_dialogueCanvasGroup.alpha = 1.0f;
		_inputCanvasGroup.alpha = 0f;
		_inputField.characterLimit = 20;

		_inputField.onSubmit.AddListener(OnInputSubmit);

		StartCoroutine(CoPlayIntroSequence());
	}


    
    void Update()
    {
        // 한글 감지를 위한 작업
        if (Input.compositionString != _lastComposition)
        {
            if (!string.IsNullOrEmpty(Input.compositionString))
            {
                OnInputFieldChanged();
            }
            _lastComposition = Input.compositionString;
        }

        // 2) 실제 입력 필드 텍스트 변경 감지
        if (_inputField.text != _lastInputText)
        {
            OnInputFieldChanged();
            _lastInputText = _inputField.text;
        }

        _soundTimer += Time.deltaTime;
    }

    private void OnInputFieldChanged()
    {
		if (_soundTimer < _soundDelay)
			return;

		_soundTimer = 0f;
		Managers.Sound.Play("intro_type_short", Sound.Effect);
    }

    private IEnumerator CoPlayIntroSequence()
	{
		// 초기 대사 출력
		for (int i = 0; i < _dialogue.Count; i++)
        {
            Managers.Sound.Play("intro_typing2", Sound.SubEffect);
            yield return _dialogueText.CoTypingEffect(_dialogue[i], 0.075f);
            Managers.Sound.PauseSubEffect();
            yield return WaitForSecondsCache.Get(2f);

            yield return _dialogueCanvasGroup.CoFadeIn(_fadeDuration, waitAfter: 0.5f);
            _dialogueText.text = "";
            yield return _dialogueCanvasGroup.CoFadeOut(_fadeDuration);
        }

		// 유저 입력 대기
		yield return StartCoroutine(ShowAndWaitInput());
		
        
        // 유저 입력 완료 후 마지막 대사 출력
        Managers.Sound.Play("intro_typing2", Sound.SubEffect);
        yield return _dialogueText.CoTypingEffect(_lastDialogue, 0.075f);
        Managers.Sound.PauseSubEffect();
        yield return WaitForSecondsCache.Get(2f);
        yield return _dialogueCanvasGroup.CoFadeIn(_fadeDuration, waitAfter: 0.5f);
		_dialogueText.text = "";
        yield return _dialogueCanvasGroup.CoFadeOut(_fadeDuration);

        // 게임 씬으로 페이드인
        Managers.Scene.LoadScene(Scene.IntroScene);
	}

	private IEnumerator ShowAndWaitInput()
	{
		// 입력 필드 표시
		_inputField.text = "";
		_inputField.gameObject.SetActive(true);

		yield return _inputCanvasGroup.CoFadeOut(_fadeDuration);

        _inputField.Select();
		_inputField.ActivateInputField();

        while (_inputCanvasGroup.alpha > 0 && _inputField.gameObject.activeSelf)
        {
            yield return null;
        }
    }

	private void OnInputSubmit(string userInput)
	{
		// 엔터 입력 시 입력 필드 페이드 아웃
		StartCoroutine(HideInputField());
	}

	private IEnumerator HideInputField()
	{
		yield return _inputCanvasGroup.CoFadeIn(_fadeDuration, waitAfter: 0.5f);
        _inputField.gameObject.SetActive(false);
	}
}
