using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �׽�Ʈ��. ���� ���� �� ���� ������ ��.
/// </summary>
public class SimpleChoiceUI : MonoBehaviour, IChoiceUI
{
	[Serializable] public class ButtonBind { public Button button; public TMP_Text label; }

	public CanvasGroup group;
	public ButtonBind[] buttons; // �ν����Ϳ� 2~4�� ���ε�

	public IEnumerator ShowChoices(ChoiceTable.ChoiceRow row, Action<int> onSelected)
	{
		int sel = -1;
		group.alpha = 1; group.interactable = true; group.blocksRaycasts = true;

		for (int i = 0; i < buttons.Length; i++)
		{
			bool active = row != null && i < row.options.Count;
			buttons[i].button.gameObject.SetActive(active);
			if (!active) continue;

			int idx = i;
			buttons[i].label.text = row.options[i].text;
			buttons[i].button.onClick.RemoveAllListeners();
			buttons[i].button.onClick.AddListener(() => sel = idx);
		}

		yield return new WaitUntil(() => sel >= 0);

		group.alpha = 0; group.interactable = false; group.blocksRaycasts = false;
		onSelected?.Invoke(sel);
	}
}
