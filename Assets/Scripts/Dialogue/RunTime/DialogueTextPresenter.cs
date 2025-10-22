using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTextPresenter : MonoBehaviour, ITextPresenter
{
	[Header("Febucci Typewriter")]
	public Febucci.UI.Core.TypewriterCore typewriter; // 없으면 null 허용

	[Header("Fallback UI")]
	public TMP_Text nameLabel;
	public TMP_Text textLabel;

	public UnityEvent onLineStart;    // 효과음/표시 등
	public UnityEvent onLineEnd;      // 클릭 가능 표시 등

	public IActorDirector actor;      // 선택(없으면 포즈 무시)

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

		// 아무 키/클릭 대기
		while (!Input.GetMouseButtonDown(0) && !Input.anyKeyDown) yield return null;
	}
}
