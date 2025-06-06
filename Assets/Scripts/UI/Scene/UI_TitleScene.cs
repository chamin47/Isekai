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

	// ����
	private readonly List<string> dialogue = new List<string>
	{
		"�� ���迡�� ��ó���� ��� �̵鿡�� �����ϴ�.",
		"�� ���迡�� ��� �̼���� ���Ѵٸ� \n�츮�� ���� �ູ���� �� �������?",
	};

	private readonly string lastDialogue = "����� �亯�� ��� �ǹ̴� �����ϱ��? \n�װ��� ã�� ���� ù������ ���۵˴ϴ�.";

    public override void Init()
	{
		base.Init();
		_dialogueText.text = "";
		_dialogueCanvasGroup.alpha = 1.0f;
		_inputCanvasGroup.alpha = 0f;
		_inputField.characterLimit = 20;
		//_inputField.onValueChanged.AddListener(OnInputFieldChanged);
		_inputField.onSubmit.AddListener(OnInputSubmit);

		StartCoroutine(PlayIntroSequence());
	}


    private string _lastComposition = "";
    private string _lastInputText = "";
	[SerializeField] private float _soundDelay = 0.075f; // Ÿ���� ���� ������
	private float _soundTimer = 0f;
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

    private IEnumerator PlayIntroSequence()
	{
		for (int i = 0; i < dialogue.Count; i++)
        {
            yield return StartCoroutine(TypeAndWait(_dialogueText, dialogue[i], 0.075f));
        }

		yield return StartCoroutine(ShowInputField());
		// ���� �Է� ���
		while (_inputCanvasGroup.alpha > 0 && _inputField.gameObject.activeSelf)
		{
			yield return null;
		}

		// ��� 3
		// ���� �Է� �Ϸ� �� ������ ��� ���

		yield return StartCoroutine(TypeAndWait(_dialogueText, lastDialogue, 0.075f));

		// ���� ������ ���̵���
		Managers.Scene.LoadScene(Scene.IntroScene);
	}

	private IEnumerator TypeAndWait(TMP_Text textComponent, string content, float typingSpeed)
	{
        _dialogueText.text = "";
        Managers.Sound.Play("intro_typing2", Sound.SubEffect);
        yield return _dialogueText.CoTypingEffect(content, 0.075f);
        Managers.Sound.PauseSubEffect();
        yield return WaitForSecondsCache.Get(2f);

        yield return StartCoroutine(FadeOutText());
    }

	private IEnumerator FadeOutText()
	{
		float startAlpha = _dialogueCanvasGroup.alpha;
		float elapsed = 0f;
		while (elapsed < _fadeDuration)
		{
			elapsed += Time.deltaTime;
			_dialogueCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / _fadeDuration);
			yield return null;
		}

		_dialogueCanvasGroup.alpha = 0f;
		// �ؽ�Ʈ ����
		_dialogueText.text = "";
		// ���̵� �ƿ� �� ��� ���
		yield return new WaitForSeconds(0.5f);

		// �ٽ� ���̵� �� (���� ��� ����)
		elapsed = 0f;
		while (elapsed < _fadeDuration)
		{
			elapsed += Time.deltaTime;
			_dialogueCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / _fadeDuration);
			yield return null;
		}

		_dialogueCanvasGroup.alpha = 1f;
	}

	private IEnumerator ShowInputField()
	{
		// �Է� �ʵ� ǥ��
		_inputField.text = "";
		_inputField.gameObject.SetActive(true);

		float elapsed = 0f;
		float startAlpha = _inputCanvasGroup.alpha;
		while (elapsed < _fadeDuration)
		{
			elapsed += Time.deltaTime;
			_inputCanvasGroup.alpha = Mathf.Lerp(startAlpha, 1f, elapsed/ _fadeDuration);
			yield return null;
		}
		_inputCanvasGroup.alpha = 1f;

		_inputField.Select();
		_inputField.ActivateInputField();
	}

	private void OnInputSubmit(string userInput)
	{
		// ���� �Է� �� �Է� �ʵ� ���̵� �ƿ�
		StartCoroutine(HideInputField());
	}

	private IEnumerator HideInputField()
	{
		float elapsed = 0f;
		float startAlpha = _inputCanvasGroup.alpha;
		while (elapsed < _fadeDuration)
		{
			elapsed += Time.deltaTime;
			_inputCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed/ _fadeDuration);
			yield return null;
		}
		_inputCanvasGroup.alpha = 0f;
		_inputField.gameObject.SetActive(false);

	}
}
