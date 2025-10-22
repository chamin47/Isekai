using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTextPresenter : MonoBehaviour, ITextPresenter
{
	[Header("Febucci Typewriter")]
	public Febucci.UI.Core.TypewriterCore typewriter; // ������ null ���

	[Header("Fallback UI")]
	public TMP_Text nameLabel;
	public TMP_Text textLabel;

	public UnityEvent onLineStart;    // ȿ����/ǥ�� ��
	public UnityEvent onLineEnd;      // Ŭ�� ���� ǥ�� ��

	public IActorDirector actor;      // ����(������ ���� ����)

	public IEnumerator ShowText(string speaker, string text, string animName)
	{
		if (actor != null && !string.IsNullOrEmpty(animName))
			actor.SetPose(speaker, animName);

		if (nameLabel) nameLabel.text = speaker ?? "";

		onLineStart?.Invoke();

		if (typewriter != null)
		{
			bool shown = false;
			typewriter.onTextShowed.RemoveAllListeners();
			typewriter.onTextShowed.AddListener(() => shown = true);
			typewriter.ShowText(text ?? "");
			yield return new WaitUntil(() => shown);
		}
		else
		{
			if (textLabel) textLabel.text = text ?? "";
		}

		onLineEnd?.Invoke();

		// �ƹ� Ű/Ŭ�� ���
		while (!Input.GetMouseButtonDown(0) && !Input.anyKeyDown) yield return null;
	}
}
