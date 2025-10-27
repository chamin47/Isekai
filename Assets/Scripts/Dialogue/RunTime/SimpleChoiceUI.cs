using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 테스트용. 실제 구현 시 새로 만들어야 함.
/// </summary>
public class SimpleChoiceUI : MonoBehaviour, IChoiceUI
{
	[Serializable] public class ButtonBind { public Button button; public TMP_Text label; }

	public CanvasGroup group;
	public ButtonBind[] buttons; // 인스펙터에 2~4개 바인딩

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
