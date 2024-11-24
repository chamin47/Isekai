using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
	public int happinessThreshold = 100;
	public int happinessMin = 0;
	public int maxDialogueCount = 6; // �� �������� �� �ִ� ��ȭ ��

	private int currentDialogueCount = 0;

	void Start()
	{
		// �������� ���� �� �ʱ�ȭ
		currentDialogueCount = 0;
	}

	void Update()
	{
		float happiness = Managers.Happy.Happiness;

		if (happiness >= happinessThreshold)
		{
			ClearStage();
		}
		else if (happiness <= happinessMin || currentDialogueCount >= maxDialogueCount)
		{
			FailStage();
		}
	}

	public void OnDialogueSpawned()
	{
		currentDialogueCount++;
	}

	private void ClearStage()
	{
		// ��Ż ����
		GameObject portal = Managers.Resource.Instantiate("Prefabs/Portal", Managers.UI.Root.transform);
		Portal portalScript = portal.GetComponent<Portal>();
		portalScript.targetSceneName = GetNextSceneName();

		// Ŭ���� �˾� ǥ��
		//Managers.UI.ShowPopupUI<UI_ClearPopup>();
	}

	private string GetNextSceneName()
	{
		// ���� ���� ���� �� �̸��� ��ȯ�ϴ� ���� ����
		// ���÷� "NextStage" ��ȯ
		return "NextStage";
	}

	private void FailStage()
	{
		// ���� ���� �˾� ǥ��
		//Managers.UI.ShowPopupUI<UI_FailPopup>();
		// ���� ���� ó�� ���� �߰�
	}
}
