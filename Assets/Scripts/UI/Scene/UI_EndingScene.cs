using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_EndingScene : UI_Scene
{
	[SerializeField] private TMP_Text _newsText;       // ���� ��� �ؽ�Ʈ
	[SerializeField] private TMP_Text _titleText;      // ���� �ؽ�Ʈ

	public override void Init()
	{
		base.Init();

		_newsText.text = "";       // ���� �ؽ�Ʈ �ʱ�ȭ
		_titleText.text = "";      // ���� �ؽ�Ʈ �ʱ�ȭ

		StartCoroutine(PlayEndingSequence());
	}

	private IEnumerator PlayEndingSequence()
	{
		// ���� ��� Ÿ���� ȿ��
		string newsDialogue = "�Ӻ��Դϴ�.\n�ֱ� '�̼���'�� ���� '�� ����'�� �ڽ��� �Ҿ�� ��ʰ� �����ϰ� �ִٴ� �����Դϴ�.\n���������� �̷��� ������ �ΰ� '�̼��� ���ı�'�̶�� �̸��� �ٿ����ϴ�.";
		yield return StartCoroutine(TypeEffect(_newsText, newsDialogue, 0.05f));

		// ��� ��� �Ϸ� �� 2�� ���
		yield return new WaitForSeconds(2f);
		_newsText.gameObject.SetActive(false);

		// ���� �ؽ�Ʈ ���̵� ��
		_titleText.text = "�̼��� ���ı�";
		yield return StartCoroutine(FadeText(_titleText, 0f, 1f, 2f));

		// ���� ȭ������ ��ȯ
		Managers.Scene.LoadScene(Scene.GameScene); // Main Title Scene���� ��ȯ
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

	// �ؽ�Ʈ ���̵� ȿ��
	private IEnumerator FadeText(TMP_Text textComponent, float startAlpha, float endAlpha, float duration)
	{
		float time = 0f;
		Color color = textComponent.color;
		color.a = startAlpha;
		textComponent.color = color;

		while (time < duration)
		{
			time += Time.deltaTime;
			float alpha = Mathf.Lerp(startAlpha, endAlpha, time / duration);
			color.a = alpha;
			textComponent.color = color;
			yield return null;
		}

		color.a = endAlpha;
		textComponent.color = color;
	}
}
