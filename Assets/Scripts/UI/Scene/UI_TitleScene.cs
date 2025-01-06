using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
	private string dialogue1 = "�� ���迡�� ��ó���� ��� �̵鿡�� �����ϴ�.";
	private string dialogue2 = "�� ���迡�� ��� �̼���� ���Ѵٸ� \n�츮�� ���� �ູ���� �� �������?";
	private string dialogue3 = "����� �亯�� ��� �ǹ̴� �����ϱ��? \n�װ��� ã�� ���� ù������ ���۵˴ϴ�.";

	public override void Init()
	{
		base.Init();
		_dialogueText.text = "";
		_dialogueCanvasGroup.alpha = 1.0f;
		_inputCanvasGroup.alpha = 0f;
		_inputField.characterLimit = 50;
		_inputField.onSubmit.AddListener(OnInputSubmit);

		StartCoroutine(PlayIntroSequence());
	}

	private IEnumerator PlayIntroSequence()
	{
		// ��� 1
		yield return StartCoroutine(TypeAndWait(_dialogueText, dialogue1, 0.075f));
		yield return StartCoroutine(FadeOutText());

		yield return StartCoroutine(TypeAndWait(_dialogueText, dialogue2, 0.075f));
		yield return StartCoroutine(FadeOutText());

		// ��� 2
		yield return StartCoroutine(ShowInputField());
		// ���� �Է� ��� (OnInputSubmit �߻� ������ ���)
		while (_inputCanvasGroup.alpha > 0 && _inputField.gameObject.activeSelf)
		{
			yield return null;
		}

		// ��� 3
		// ���� �Է� �Ϸ� �� ������ ��� ���
		_dialogueText.text = "";
		yield return StartCoroutine(TypeAndWait(_dialogueText, dialogue3, 0.075f));
		yield return StartCoroutine(FadeOutText());

		// ���� ������ ���̵���
		Managers.Scene.LoadScene(Scene.IntroScene);
	}

	private IEnumerator TypeAndWait(TMP_Text textComponent, string content, float typingSpeed)
	{
		textComponent.text = "";
		// ��� Ÿ����
		yield return StartCoroutine(TypeEffect(textComponent, content, typingSpeed));
		// Ÿ���� �Ϸ� �� 3�� ���
		yield return new WaitForSeconds(4f);
	}

	// �ؽ�Ʈ Ÿ���� ȿ��
	private IEnumerator TypeEffect(TMP_Text textComponent, string content, float typingSpeed)
	{
		foreach (char c in content)
		{
			textComponent.text += c;
			yield return new WaitForSeconds(typingSpeed);
		}
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
