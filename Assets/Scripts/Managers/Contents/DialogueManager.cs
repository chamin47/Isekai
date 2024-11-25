using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
	// ���������� ��� ����� �߰��� ������ �� �ֽ��ϴ�.
	private Dictionary<string, string[]> stageDialogues = new Dictionary<string, string[]>();

	private StageManager stageManager;

	void Start()
	{
		stageDialogues.Add("stage01", new string[] {
		"���� �� ������ ǰ���� ���� �Ϻ��� �ι��̼�",
		"���۴��� ����� ���� ������ �����̾�.",
		"���۴��� �ܸ�� ���� ������ ���� ��ǰ ����.",
		"���۴��� Ī���ϴ� �� �����ε� �����̾�!!",
		"���� ���� ��ü�� �� ������ ū �ູ����.",
		"��� ���ְ� ���۴��� �ܸ� ���� ������ �기����?"
	});

		// Stage3 ��� �߰�
		stageDialogues.Add("stage02", new string[] {
		"����ϱ� ¦�� ���ٸ鼭 ��� �������� �ϴ� �ž�?",
		"�� ��¥ ���簨�̶��� 1�� ����",
		"�ܸ� �ɷµ� Ư���� �� ���� ������� �� ����ϰھ�.",
		"���� ������ �����̳� ���� �� �˾�?",
		"�� ���ʿ� ������ ����̾�",
		"�ʴ� ��ü �� ���ϴ�?"
	});

		// Stage3 ��� �߰�
		stageDialogues.Add("stage03", new string[] {
		"���� ������ ��ġ�� �� ������ ����� �� �����ž�",
		"���ӿ� �������� ���� ���� ū �ŷ��̾�.",
		"�ʴ� ����� ���̿����� �ΰ��� �巯���� Ư���� �����.",
		"�Բ� ������ ��ſ�!!",
		"���� ������ �������� ������ ���� ���� ��",
		"�ʴ� �� ������ ������",
		"���� �������� ���� �����",
	});
		// Stage3 ��� �߰�
		stageDialogues.Add("stage04", new string[] {
		"�� �������δ� �� ���� �� ���� �ʾ�?",
		"��¥ �ƴ� �� ����",
		"�ʳ� �������� ������ ��� ���� �ϼ����ٵ�...",
		"�� �����߾�?",
		"�̰͵� ����?",
		"�� �ߵǶ�� �ϴ� �Ҹ���.",
		"�ð� ����� �װ�",
		"������ ����ó�� ��ž�?",
		"������ �ʹ� ū ��뿴����.",
		"�������� ���鷡?",
	});
		stageDialogues.Add("stage05", new string[] {
		"���� ���縸���ε� ������ �ູ���� �� ����",
		"�״� ����� �Ҿ�ִ� ���� ���� �����!!",
		"���� �翡�� �׻� ��ȭ�� ��ܽ��� ������.",
		"���� ���������� ������",
		"���� �ձ��� ������ ������",
		"�ٶ󺸱⸸ �ص� ���̷ο�",
		"���� ����� �ΰ����� �־� ���� ū �����̾�",
		"����� �ٶ󺸱⸸ �ص� �ູ��",
		"���� ��ó�� �ູ������ �;�",
	});
		// �ٸ� �������� �߰� ����
		stageManager = FindObjectOfType<StageManager>();
		if (stageManager == null)
		{
			Debug.LogError("StageManager�� ã�� �� �����ϴ�.");
		}


		StartCoroutine(SpawnDialogues());
		StartCoroutine(EndingCoroutine());

	}

	IEnumerator SpawnDialogues()
	{
		while (true)
		{
			yield return new WaitForSeconds(2f); // ��ȭâ ���� ����
			Debug.Log("SpawnDialogues: Coroutine ���� ��");

			string currentScene = Managers.Scene.CurrentScene.SceneType.ToString();
			Debug.Log($"Current Scene: {currentScene}");

			if (!stageDialogues.ContainsKey(currentScene))
			{
				Debug.LogWarning($"�������� '{currentScene}'�� ���� ��簡 �������� �ʽ��ϴ�.");
				continue;
			}

			string[] currentDialogues = stageDialogues[currentScene];
			int index = Random.Range(0, currentDialogues.Length);
			string dialogue = currentDialogues[index];

			// ���������� Ű ���� ���� (��: 4��)
			int keyCount = GetKeyCountForStage(currentScene);

			// �ʿ��� Ű ����Ʈ ����
			List<string> requiredKeys = GenerateRandomKeys(keyCount);

			// �˾� ����
			UI_DialogueWindowPopup dialoguePopup = Managers.UI.ShowPopupUI<UI_DialogueWindowPopup>();

			// �˾� ����
			dialoguePopup.Setup(
				dialogue,
				requiredKeys,
				OnDialogueSuccess,
				OnDialogueFail,
				limitTime: GetDialogueTimeLimit(currentScene),
				increase: GetHappinessIncrease(currentScene),
				decrease: GetHappinessDecrease(currentScene)
			);

			// ���� ��ȭ �� ����
			stageManager.OnDialogueSpawned();
		}
	}

	private List<string> GenerateRandomKeys(int keyCount)
	{
		string[] keys = { "Q", "W", "E", "R", "A", "S", "D", "F" };
		List<string> selectedKeys = new List<string>();

		while (selectedKeys.Count < keyCount)
		{
			string key = keys[Random.Range(0, keys.Length)];
			if (!selectedKeys.Contains(key))
			{
				selectedKeys.Add(key);
			}
		}

		return selectedKeys;
	}

	private int GetKeyCountForStage(string sceneName)
	{
		// ���������� �䱸�Ǵ� Ű ������ ��ȯ
		switch (sceneName)
		{
			case "stage01":
				return 5; // ��: 4���� Ű �䱸
			case "stage02":
				return 5;
			case "stage03":
				return 5;
			case "stage04":
				return 5;
			case "stage05":
				return 5;
			default:
				return 4;
		}
	}

	private float GetDialogueTimeLimit(string sceneName)
	{
		// ���������� ���� �ð� ����
		switch (sceneName)
		{
			case "stage01":
				return 3f;
			case "stage02":
				return 1.5f;
			case "stage03":
				return 2f;
			case "stage04":
				return 1f;
			case "stage05":
				return 2f;
			default:
				return 3f;
		}
	}

	private int GetHappinessIncrease(string sceneName)
	{
		// ���������� �ູ�� ������ ����
		switch (sceneName)
		{
			case "stage01":
				return 40;
			case "stage02":
				return 5;
			case "stage03":
				return 20;
			case "stage04":
				return 5;
			case "stage05":
				return 1;
			default:
				return 40;
		}
	}

	private int GetHappinessDecrease(string sceneName)
	{
		// ���������� �ູ�� ���ҷ� ����
		switch (sceneName)
		{
			case "stage01":
				return 5;
			case "stage02":
				return 20;
			case "stage03":
				return 5;
			case "stage04":
				return 10;
			case "stage05":
				return 5;
			default:
				return 10;
		}
	}

	private void OnDialogueSuccess(int happinessIncrease)
	{
		Managers.Happy.ChangeHappiness(happinessIncrease);
	}

	private void OnDialogueFail(int happinessDecrease)
	{
		Managers.Happy.ChangeHappiness(happinessDecrease);
	}

	private IEnumerator EndingCoroutine()
	{
		yield return new WaitForSeconds(10.0f);
		Managers.Scene.LoadScene(Scene.EndingScene);

	}
}
