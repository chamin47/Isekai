using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeakerBindingSimple
{
	public string speaker;                 // "�缭", "Player" ��
	public ActorAnimatorSimple actor;      // �ش� ���
	public Transform anchor;               // ��ǳ�� ������(�Ӹ� �� ��)
}

public class ActorDirectorSimple : MonoBehaviour, IActorDirector, ISpeakerAnchorResolver
{
	[SerializeField] List<SpeakerBindingSimple> bindings = new();

	// ��ü ���ε��� ã�� ���� (��Ŀ/��� �� �� �ʿ��� �� ���)
	SpeakerBindingSimple FindBind(string speaker)
	{
		if (string.IsNullOrEmpty(speaker)) return null;
		foreach (var b in bindings)
		{
			if (b != null && b.actor != null && b.speaker == speaker)
				return b;
		}
		return null;
	}

	// ��츸 �ʿ��� ���� �̰͸�
	ActorAnimatorSimple Find(string speaker) => FindBind(speaker)?.actor;

	public void SetPose(string speaker, string animName)
	{
		var a = Find(speaker);
		if (a == null || string.IsNullOrEmpty(animName)) return;
		a.SetPose(animName);
	}

	public IEnumerator PlayOnce(string speaker, string animName, float? durationSec = null)
	{
		var a = Find(speaker);
		if (a == null || string.IsNullOrEmpty(animName)) yield break;

		var n = animName.ToLowerInvariant();
		if (n.Contains("idle") || n.Contains("walk") || n.Contains("loop"))
		{
			a.SetPose(animName);
			yield break;
		}

		yield return a.PlayOnce(animName, durationSec);
	}

	// IResolveAnchor: ��ǳ�� ��Ŀ ��ġ ����
	public Transform ResolveAnchor(string speaker)
	{
		var b = FindBind(speaker);
		if (b != null)
		{
			if (b.anchor) return b.anchor;                 // ���� ��Ŀ �켱
			if (b.actor) return b.actor.transform;        // ������ ��� Ʈ������ ����
		}
		return null; // ���� ������ �������� �ʿ��� ó��
	}
}
