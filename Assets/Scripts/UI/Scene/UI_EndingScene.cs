using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_EndingScene : UI_Scene
{
	[SerializeField] private TMP_Text _newsText;       // ���� ��� �ؽ�Ʈ
	[SerializeField] private TMP_Text _finalText;      // ����ȭ�� �ؽ�Ʈ
    [SerializeField] private GameObject _bubbleImage;  // ��ǳ�� �̹���

    [SerializeField] private Image _fadeImage;

	[SerializeField] private AudioSource _effect;

    private EndingSceneData _sceneData;
	public override void Init()
	{
		base.Init();
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        _sceneData = Managers.DB.GetEndingSceneData();

        _newsText.text = "";       // ���� �ؽ�Ʈ �ʱ�ȭ
		_finalText.text = "";      // ���� �ؽ�Ʈ �ʱ�ȭ
        _bubbleImage.SetActive(false); // ��ǳ�� �̹��� ��Ȱ��ȭ

        StartCoroutine(PlayEndingSequence());
	}

	private IEnumerator PlayEndingSequence()
	{
		yield return new WaitForSeconds(0.5f);
        _bubbleImage.SetActive(true); // ��ǳ�� �̹��� Ȱ��ȭ

        _newsText.text = _sceneData.newsDialog[0];

        yield return WaitForSecondsCache.Get(1.5f); // 1�� ���
        // ���� �ؽ�Ʈ ���
        for (int i = 1; i < _sceneData.newsDialog.Count; i++)
        {
            yield return StartCoroutine(TypeEffect(_newsText, _sceneData.newsDialog[i], 0.1f));
        }

        // ȭ�� fadeOut
        yield return StartCoroutine(_fadeImage.CoFadeOut(1f));
        
        yield return WaitForSecondsCache.Get(1f); // 1�� ���

        // ���� ȭ����� �ؽ�Ʈ ���
        foreach (var finalDialogue in _sceneData.finalDialog)
        {
            yield return StartCoroutine(TypeEffect(_finalText, finalDialogue, 0.1f));
        }

        _finalText.text = "";

        yield return WaitForSecondsCache.Get(2f);

        // ���� ���� �������� ����

        // �ؽ�Ʈ size 90���� ����
        _finalText.fontSize = 90;
        yield return StartCoroutine(TypeEffect(_finalText, "�̼��� ���ı�", 0.1f));

        // ���� ȭ������ ��ȯ
        //Managers.Scene.LoadScene(Scene.TitleScene); // Main Title Scene���� ��ȯ
    }

    // �ؽ�Ʈ Ÿ���� ȿ��
    private IEnumerator TypeEffect(TMP_Text textComponent, string content, float typingSpeed)
    {
        textComponent.text = ""; // �ʱ�ȭ

        int stringIndex = 0;
        while (stringIndex < content.Length)
        {
            char c = content[stringIndex];

            if (c == '<') // Rich Text �±� ����
            {
                int closeIndex = content.IndexOf('>', stringIndex);
                if (closeIndex == -1) // �±װ� ���������� ������ ����
                {
                    textComponent.text += c;
                }
                else
                {
                    textComponent.text += content.Substring(stringIndex, closeIndex - stringIndex + 1);
                    stringIndex = closeIndex; // �±� ������ �ǳʶٱ�
                }
            }
            else if (c == '\n') // �ٹٲ� ó��
            {
                yield return new WaitForSeconds(1.0f); // �ٹٲ� ���
                yield return StartCoroutine(FadeText(textComponent, 1f, 0f, 1f));

                textComponent.text = ""; // �ؽ�Ʈ �ʱ�ȭ
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f);
            }
            else
            {
                textComponent.text += c;
            }

            stringIndex++;
            yield return new WaitForSeconds(typingSpeed);
        }

        yield return new WaitForSeconds(0.5f); // ȿ�� ������ �ð�
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
