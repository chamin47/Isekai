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
	[SerializeField] private TMP_Text _dialogueText;       // ��� �ؽ�Ʈ

	[Header("Fade")]
	[SerializeField] private CanvasGroup _dialogueCanvasGroup;  // ���̵��
	[SerializeField] private float _fadeDuration = 1.0f;

	[Header("Input")]
	[SerializeField] private TMP_InputField _inputField;    // ���� �Է¿� (���2)
	[SerializeField] private CanvasGroup _inputCanvasGroup; // ���2 UI��

    private string _lastComposition = "";
    private string _lastInputText = "";
    [SerializeField] private float _soundDelay = 0.075f; // Ÿ���� ���� ������
    private float _soundTimer = 0f;

    private readonly List<string> _dialogue = new List<string>
	{
		"�� ���迡�� ��ó���� ��� �̵鿡�� �����ϴ�.",
		"�� ���迡�� ��� �̼���� ���Ѵٸ� \n�츮�� ���� �ູ���� �� �������?",
	};

	private readonly string _lastDialogue = "����� �亯�� ��� �ǹ̴� �����ϱ��? \n�װ��� ã�� ���� ù������ ���۵˴ϴ�.";

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
        // �ѱ� ������ ���� �۾�
        if (Input.compositionString != _lastComposition)
        {
            if (!string.IsNullOrEmpty(Input.compositionString))
            {
                OnInputFieldChanged();
            }
            _lastComposition = Input.compositionString;
        }

        // 2) ���� �Է� �ʵ� �ؽ�Ʈ ���� ����
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
		// �ʱ� ��� ���
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

		// ���� �Է� ���
		yield return StartCoroutine(ShowAndWaitInput());
		
        
        // ���� �Է� �Ϸ� �� ������ ��� ���
        Managers.Sound.Play("intro_typing2", Sound.SubEffect);
        yield return _dialogueText.CoTypingEffect(_lastDialogue, 0.075f);
        Managers.Sound.PauseSubEffect();
        yield return WaitForSecondsCache.Get(2f);
        yield return _dialogueCanvasGroup.CoFadeIn(_fadeDuration, waitAfter: 0.5f);
		_dialogueText.text = "";
        yield return _dialogueCanvasGroup.CoFadeOut(_fadeDuration);

        // ���� ������ ���̵���
        Managers.Scene.LoadScene(Scene.IntroScene);
	}

	private IEnumerator ShowAndWaitInput()
	{
		// �Է� �ʵ� ǥ��
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
		// ���� �Է� �� �Է� �ʵ� ���̵� �ƿ�
		StartCoroutine(HideInputField());
	}

	private IEnumerator HideInputField()
	{
		yield return _inputCanvasGroup.CoFadeIn(_fadeDuration, waitAfter: 0.5f);
        _inputField.gameObject.SetActive(false);
	}
}
