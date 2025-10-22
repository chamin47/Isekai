using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimpleInputPrompt : MonoBehaviour, IInputPrompt
{
	public CanvasGroup group;
	public TMP_Text promptLabel;
	public TMP_InputField inputField;
	public Button confirmButton;

	public IEnumerator Prompt(string prompt, Action<string> onDone)
	{
		group.alpha = 1; group.interactable = true; group.blocksRaycasts = true;
		if (promptLabel) promptLabel.text = prompt ?? "";
		inputField.text = "";
		string captured = null;

		confirmButton.onClick.RemoveAllListeners();
		confirmButton.onClick.AddListener(() => captured = inputField.text);

		yield return new WaitUntil(() => captured != null);

		group.alpha = 0; group.interactable = false; group.blocksRaycasts = false;
		onDone?.Invoke(captured);
	}
}
