using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SpeechBubble : UI_Base
{
	enum GameObjects
	{
		KeyImage,
		TimeGauge
	}

	enum Texts
	{
		DialogueText
	}

	[Header("Settings")]
	public bool isUpsideDown = false;      // �̹��� ���� ���� ����

	[Header("References")]
	public Image KeyImage;
	public Image TimeGauge;

	private string requiredKey;            // �ʿ��� Ű �Է�
	private float timeLimit;               // ���� �ð�
	private System.Action OnSuccess;
	private System.Action OnFail;

	private float timer;
	private bool isSuccess = false;

	private string[] keys = { "Q", "W", "E", "R", "A", "S", "D", "F" }; // �⺻ Ű �迭

	public override void Init()
	{
		
	}

	public void Configure(string dialogue, int keyCount, float limit, System.Action onSuccess, System.Action onFail)
	{
		Bind<GameObject>(typeof(GameObjects));
		Bind<TMP_Text>(typeof(Texts));

		// ��� ����
		SetDialogueText(dialogue);

		// Ű �Է� ����
		requiredKey = GenerateRandomKey(keyCount);
		SetKeyImage(requiredKey);

		// ���� �ð� ����
		timeLimit = limit;
		timer = timeLimit;

		// ���� �� ���� �ݹ� ����
		OnSuccess = onSuccess;
		OnFail = onFail;

		// �̹��� ���� ���� ó��
		if (isUpsideDown)
		{
			GetImage((int)GameObjects.KeyImage).rectTransform.localScale = new Vector3(1, -1, 1);
		}

		// Ű �Է� �̺�Ʈ ���
		//Managers.Happy.OnHappinessChanged += UpdateTimeGauge;
	}

	private void Update()
	{
		if (isSuccess) return;

		timer -= Time.deltaTime;
		UpdateTimeGauge();

		if (timer <= 0f)
		{
			Fail();
		}
		else if (Input.GetKeyDown(requiredKey.ToLower()))
		{
			Success();
		}
	}

	private void UpdateTimeGauge()
	{
		if (TimeGauge != null)
		{
			float fillAmount = timer / timeLimit;
			TimeGauge.fillAmount = fillAmount;
		}
	}

	private string GenerateRandomKey(int keyCount)
	{
		List<string> selectedKeys = new List<string>();
		while (selectedKeys.Count < keyCount)
		{
			string key = keys[Random.Range(0, keys.Length)];
			if (!selectedKeys.Contains(key))
			{
				selectedKeys.Add(key);
			}
		}
		return selectedKeys[Random.Range(0, selectedKeys.Count)];
	}

	private void SetKeyImage(string key)
	{
		if (KeyImage != null)
		{
			Sprite keySprite = Resources.Load<Sprite>($"KeyImages/{key}");
			if (keySprite != null)
			{
				KeyImage.sprite = keySprite;
			}
			else
			{
				Debug.LogError($"Key sprite not found: KeyImages/{key}");
			}
		}
	}

	private void SetDialogueText(string dialogue)
	{
		TMP_Text dialogueText = Get<TMP_Text>((int)Texts.DialogueText);
		if (dialogueText != null)
		{
			dialogueText.text = dialogue;
		}
	}

	private void Success()
	{
		if (isSuccess) return;
		isSuccess = true;
		OnSuccess?.Invoke();
		Managers.Resource.Destroy(gameObject);
	}

	private void Fail()
	{
		if (isSuccess) return;
		isSuccess = true;
		OnFail?.Invoke();
		Managers.Resource.Destroy(gameObject);
	}

	private void OnDestroy()
	{
		//Managers.Happy.OnHappinessChanged -= UpdateTimeGauge;
	}
}
