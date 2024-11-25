using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_DialogueWindowPopup : UI_Popup
{
	[SerializeField] private TMP_Text dialogueText;
	[SerializeField] private Image[] keyImages;
	[SerializeField] private Slider timeSlider;

	private List<string> requiredKeys;
	private float timeLimit;
	private Coroutine countdownCoroutine;

	private System.Action<int> onSuccess;
	private System.Action<int> onFail;

	private int happinessIncrease;
	private int happinessDecrease;

	public override void Init()
	{
		base.Init();
		// �߰� �ʱ�ȭ�� �ʿ��ϴٸ� ���⼭ ����
	}

	public void Setup(string dialogue, List<string> keys, System.Action<int> successCallback, System.Action<int> failCallback, float limitTime = 3f, int increase = 40, int decrease = 5)
	{
		dialogueText.text = dialogue;
		requiredKeys = keys;
		onSuccess = successCallback;
		onFail = failCallback;

		timeLimit = limitTime;
		timeSlider.maxValue = timeLimit;
		timeSlider.value = timeLimit;

		happinessIncrease = increase;
		happinessDecrease = decrease;

		// ��� keyImages�� ��Ȱ��ȭ
		foreach (var image in keyImages)
		{
			image.gameObject.SetActive(false);
		}

		// �ʿ��� Ű�� ������ŭ keyImages�� Ȱ��ȭ�ϰ� �̹��� ����
		for (int i = 0; i < requiredKeys.Count && i < keyImages.Length; i++)
		{
			keyImages[i].gameObject.SetActive(true);
			Sprite keySprite = Resources.Load<Sprite>($"KeyImages/{requiredKeys[i]}");
			if (keySprite != null)
			{
				keyImages[i].sprite = keySprite;
			}
			else
			{
				Debug.LogError($"Ű �̹��� �ε� ����: KeyImages/{requiredKeys[i]}");
			}
		}

		SetRandomPosition();

		countdownCoroutine = StartCoroutine(Countdown());
	}

	private IEnumerator Countdown() // �����׸� IEnumerator ���
	{
		float timer = timeLimit;
		while (timer > 0)
		{
			timer -= Time.deltaTime;
			timeSlider.value = timer;
			yield return null;
		}

		// �ð� �ʰ� �� ���� ó��
		Fail();
	}

	private void Update()
	{
		foreach (var key in requiredKeys)
		{
			if (Input.GetKeyDown(key.ToLower()))
			{
				Success();
				break;
			}
		}
	}

	/// <summary>
	/// ȭ���� ������ �Ǵ� ���ʿ� ������ ��ġ�� �˾� ��ġ
	/// </summary>
	private void SetRandomPosition()
	{
		RectTransform rectTransform = GetComponent<RectTransform>();
		if (rectTransform == null)
		{
			Debug.LogError("RectTransform�� �ʿ��մϴ�.");
			return;
		}

		// ȭ�� ũ�� ��������
		Canvas canvas = GetComponentInParent<Canvas>();
		if (canvas == null)
		{
			Debug.LogError("Canvas�� �ʿ��մϴ�.");
			return;
		}

		// anchor�� �����Ͽ� ĵ���� ��ü�� �������� ����
		rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
		rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
		rectTransform.pivot = new Vector2(0.5f, 0.5f);

		// Canvas�� ȭ�� �ʺ�� ����
		RectTransform canvasRect = canvas.GetComponent<RectTransform>();
		float canvasWidth = canvasRect.rect.width;
		float canvasHeight = canvasRect.rect.height;

		// ���� X ��ġ (���� �Ǵ� ������)
		float randomX = Random.value < 0.5f ? -canvasWidth / 4f : canvasWidth / 4f;

		// Y ��ġ�� �߾ӿ� ����� ���� ��
		float randomY = Random.Range(-canvasHeight / 4f, canvasHeight / 4f);

		// �˾��� ��ġ ����
		rectTransform.anchoredPosition = new Vector2(randomX, randomY);

		Debug.Log($"Canvas Width: {canvasRect.rect.width}, Canvas Height: {canvasRect.rect.height}");
		Debug.Log($"Random Position: X={randomX}, Y={randomY}");
		Debug.Log($"Final Anchored Position: {rectTransform.anchoredPosition}");
	}

	private void Success()
	{
		if (countdownCoroutine != null)
			StopCoroutine(countdownCoroutine);

		onSuccess?.Invoke(happinessIncrease);
		Managers.UI.ClosePopupUI();
	}

	private void Fail()
	{
		onFail?.Invoke(happinessDecrease);
		Managers.UI.ClosePopupUI();
	}
}
