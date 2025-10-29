using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ��� ǥ�� ��� ��������.
/// ��ǳ�� ������ǥ��, ������ ��ǳ���� ���� ��ġ/����, ������ ��Ŀ �ؼ��� �����Ѵ�.
/// </summary>
public class DialogueTextPresenter : MonoBehaviour, ITextPresenter
{
	public float _charSpeed = 0.03f;

	public IActorDirector actor; // Runner���� ����

	public const float StackSpacingPx = 180f;

	readonly Dictionary<Transform, List<UI_DialogueBalloon>> _stacked = new();

	public IEnumerator ShowText(string speaker, string text, string animName)
	{
		if (actor != null && !string.IsNullOrEmpty(animName))
			StartCoroutine(actor.PlayAnim(speaker, animName));

		var anchor = ResolveAnchorSafe(speaker);

		var balloon = Managers.UI.MakeWorldSpaceUI<UI_DialogueBalloon>();
		balloon.Init(anchor, text);
		yield return balloon.CoPresent(text ?? "", _charSpeed);

		yield return new WaitForSeconds(0.5f);
	}

	public IEnumerator ShowTextStacked(string speaker, string text, string animName)
	{
		if (actor != null && !string.IsNullOrEmpty(animName))
			StartCoroutine(actor.PlayAnim(speaker, animName));

		var anchor = ResolveAnchorSafe(speaker);

		if (!_stacked.TryGetValue(anchor, out var list))
		{
			list = new List<UI_DialogueBalloon>();
			_stacked[anchor] = list;
		}

		for (int i = 0; i < list.Count; i++)
			if (list[i]) list[i].AddStackOffset(StackSpacingPx);

		var balloon = Managers.UI.MakeWorldSpaceUI<UI_DialogueBalloon>();
		balloon.Init(anchor, text);

		yield return balloon.CoPresentStacked(text ?? "", _charSpeed);

		list.Add(balloon);
	}

	public void AllStacksUp()
	{
		foreach (var kv in _stacked)
		{
			var list = kv.Value;
			for (int i = list.Count - 1; i >= 0; --i)
			{
				var balloon = list[i];
				if (balloon == null)
				{
					list.RemoveAt(i);
					continue;
				}
				balloon.AddStackOffset(StackSpacingPx);
			}
		}
	}

	public void ClearAllStacked()
	{
		foreach (var kv in _stacked)
		{
			var list = kv.Value;
			for (int i = 0; i < list.Count; i++)
				if (list[i]) 
					Destroy(list[i].gameObject);
		}
		_stacked.Clear();
	}

	private Transform ResolveAnchorSafe(string speaker)
	{		
		// acotr director�� �������� ���Ѵٸ� �װ� ���
		if (actor is ISpeakerAnchorResolver a2)
		{
			var transform = a2.ResolveAnchor(speaker);
			return transform;
		}

		return Camera.main ? Camera.main.transform : transform;
	}
}
