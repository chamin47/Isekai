using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_BlamePopup : UI_Popup
{
	[SerializeField] private Image[] _talkBalloonImages;
	[SerializeField] private TMP_Text[] _dialogueTexts;

	private string[] _dialogues = {
		"�ʸ� �������� �ִ� �� �ƴϾ�?",
		"�� ģ���� ���� �����ߴٴ���",
		"�� �� ������ ���ϴ�?",
		"�ٵ� �� ��ٴ���, �ʴ� ��ȹ �־�?",
		"�� ��¥ ȥ�ڸ� �ٸ��� ���."
	};

	private float _glitchDuration = 1f; // �۸�ġ ȿ�� ���� �ð�

	public override void Init()
	{
		base.Init();
		// ��� ��ȭ ǳ�� �̹����� ��Ȱ��ȭ�ϰ� �ؽ�Ʈ�� �ʱ�ȭ�մϴ�.
		for (int i = 0; i < _talkBalloonImages.Length; i++)
		{
			_talkBalloonImages[i].gameObject.SetActive(false);
			_dialogueTexts[i].text = "";
		}

		// ��� ��� �ڷ�ƾ ����
		StartCoroutine(ShowDialogues());
	}

	private IEnumerator ShowDialogues()
	{

		yield return WaitForSecondsCache.Get(2f); // �ణ�� ��� �ð�

        for (int i = 0; i < _dialogues.Length; i++)
		{
			// �ش� ��ȭ ǳ�� �̹����� Ȱ��ȭ�մϴ�.
			_talkBalloonImages[i].gameObject.SetActive(true);

			// Ÿ�� ġ�� ȿ���� ��� ���
			yield return StartCoroutine(TypeEffect(_dialogueTexts[i], _dialogues[i], true, "intro_type_short"));

			// �ູ ������ ����
			Managers.Happy.ChangeHappiness(-10f);

			Managers.Sound.Play("s1_say_impact2", Sound.Effect);
			// ī�޶� ��鸲 ȿ�� ���� (�� ����������)
			ShakeCamera(duration: 0.5f, magnitude: 0.3f);

			// ���� ������ 1�� ���
			yield return new WaitForSeconds(1f);
		}

		// ��� ��簡 ���� �� 2�� ���
		yield return new WaitForSeconds(2f);

		// �۸�ġ ȿ���� ���̵� �ƿ��� ���������� ����
		StartCoroutine(GlitchEffect());
		yield return StartCoroutine(FadeOutDialogues());

		// ��� ó���� ���� �� �˾� �ݱ� �Ǵ� ���� �ൿ ����
		//ClosePopupUI();
	}

	private IEnumerator TypeEffect(TMP_Text textComponent, string dialogue, bool playSound = false, string soundKey = "")
	{
        int typingCount = 0;

        textComponent.text = "";
		foreach (char c in dialogue)
		{
			textComponent.text += c;
            if (playSound && (c != ' ' || c != '\n'))
            {
				typingCount += 1;
				if(typingCount % 2 == 0)
				{
                    Managers.Sound.Play(soundKey, Sound.Effect);
				}
            }
			yield return WaitForSecondsCache.Get(0.05f); // Ÿ�� ġ�� �ӵ� ���� ����
		}
	}

	private void ShakeCamera(float duration = 0.5f, float magnitude = 0.3f)
	{
		StartCoroutine(ShakeCoroutine(duration, magnitude));
	}

	private IEnumerator ShakeCoroutine(float duration, float magnitude)
	{
		Transform cameraTransform = Camera.main.transform;
		Vector3 originalPos = cameraTransform.localPosition;
		float elapsed = 0.0f;

		while (elapsed < duration)
		{
			float x = Mathf.Sin(Time.time * 50f) * magnitude * (1f - (elapsed / duration));
			float y = Mathf.Cos(Time.time * 60f) * magnitude * (1f - (elapsed / duration));

			cameraTransform.localPosition = originalPos + new Vector3(x, y, 0f);

			elapsed += Time.deltaTime;

			yield return null;
		}

		cameraTransform.localPosition = originalPos;
	}

	private IEnumerator GlitchEffect()
	{
		float elapsed = 0f;
		float interval = 0.05f; // �۸�ġ ȿ���� ������Ʈ ����
		while (elapsed < _glitchDuration)
		{
			// �� ��ȭ ǳ�� �̹����� �ؽ�Ʈ�� �۸�ġ ȿ�� ����
			for (int i = 0; i < _talkBalloonImages.Length; i++)
			{
				if (_talkBalloonImages[i].gameObject.activeSelf)
				{
					// ��ġ ����
					RectTransform rectTransform = _talkBalloonImages[i].rectTransform;
					Vector3 originalPosition = rectTransform.anchoredPosition;
					rectTransform.anchoredPosition = originalPosition + new Vector3(Random.Range(-5f, 5f), Random.Range(-5f, 5f), 0f);

					// ���� ����
					_talkBalloonImages[i].color = new Color(Random.value, Random.value, Random.value);

					// �ؽ�Ʈ ����
					_dialogueTexts[i].color = new Color(Random.value, Random.value, Random.value);
					_dialogueTexts[i].fontSize = Random.Range(20, 30);
				}
			}

			yield return new WaitForSeconds(interval);

			// ������ �� ����
			for (int i = 0; i < _talkBalloonImages.Length; i++)
			{
				if (_talkBalloonImages[i].gameObject.activeSelf)
				{
					// ��ġ ����
					RectTransform rectTransform = _talkBalloonImages[i].rectTransform;
					rectTransform.anchoredPosition3D = rectTransform.anchoredPosition3D;

					// ���� ����
					_talkBalloonImages[i].color = Color.white;
					_dialogueTexts[i].color = Color.black;
					_dialogueTexts[i].fontSize = 24; // ���� ��Ʈ ũ��� ����
				}
			}

			elapsed += interval;
		}
	}

	// ���̵� �ƿ� �ڷ�ƾ �߰�
	private IEnumerator FadeOutDialogues()
	{
		float duration = 1f;
		float elapsed = 0f;

		// �� ��ȭ ǳ�� �̹����� �ؽ�Ʈ�� �ʱ� ���� ����
		Color[] balloonColors = new Color[_talkBalloonImages.Length];
		Color[] textColors = new Color[_dialogueTexts.Length];

		for (int i = 0; i < _talkBalloonImages.Length; i++)
		{
			balloonColors[i] = _talkBalloonImages[i].color;
			textColors[i] = _dialogueTexts[i].color;
		}
		Managers.Sound.Play("s1_glitch2", Sound.Effect);

		while (elapsed < duration)
		{
			float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

			// ���� �� ���� ����
			for (int i = 0; i < _talkBalloonImages.Length; i++)
			{
				if (_talkBalloonImages[i].gameObject.activeSelf)
				{
					Color balloonColor = balloonColors[i];
					balloonColor.a = alpha;
					_talkBalloonImages[i].color = balloonColor;

					Color textColor = textColors[i];
					textColor.a = alpha;
					_dialogueTexts[i].color = textColor;
				}
			}

			elapsed += Time.deltaTime;
			yield return null;
		}


        // ���������� ���� ���� 0���� �����ϰ� ������Ʈ ��Ȱ��ȭ
        for (int i = 0; i < _talkBalloonImages.Length; i++)
		{
			if (_talkBalloonImages[i].gameObject.activeSelf)
			{
				Color balloonColor = _talkBalloonImages[i].color;
				balloonColor.a = 0f;
				_talkBalloonImages[i].color = balloonColor;

				Color textColor = _dialogueTexts[i].color;
				textColor.a = 0f;
				_dialogueTexts[i].color = textColor;

				_talkBalloonImages[i].gameObject.SetActive(false);
			}
		}

		GameObject go = new GameObject { name = "CutSceneDirector" };
		go.AddComponent<CutSceneDirector>();		
	}
}
