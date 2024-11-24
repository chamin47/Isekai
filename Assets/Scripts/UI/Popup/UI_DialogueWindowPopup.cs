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

	private void Success()
	{
		if (countdownCoroutine != null)
			StopCoroutine(countdownCoroutine);

		onSuccess?.Invoke(happinessIncrease);
		ClosePopupUI();
	}

	private void Fail()
	{
		onFail?.Invoke(happinessDecrease);
		ClosePopupUI();
	}
}
