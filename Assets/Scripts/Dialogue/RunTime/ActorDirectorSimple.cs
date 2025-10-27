using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ȭ�ڿ� �ش� ĳ����/��Ŀ Ʈ�������� �����ϴ� ����ȭ ������ ���ε� ������.
/// ��ȭ �ý��ۿ��� ȭ�ڸ����� �ִ�/��ǳ�� ��Ŀ�� ã�� �� ����Ѵ�.
/// </summary>
[System.Serializable]
public class SpeakerBindingSimple
{
	public string speaker;                 // "�缭", "Player" ��
	public ActorAnimatorSimple actor;      // �ش� ĳ���Ϳ� �޸� �ִϸ����� �����
	public Transform anchor;               // ��ǳ�� ������(�Ӹ� �� ��)
}

/// <summary>
/// ������ ���� ����.
/// ȭ�ڸ����� ActorAnimator/��Ŀ�� �ؼ��� �ִϸ��̼� ��� �� ��ǳ�� ��Ŀ ������ ����Ѵ�.
/// </summary>
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

	// ActorAnimator�� �ʿ��� ���� �̰͸�
	ActorAnimatorSimple Find(string speaker) => FindBind(speaker)?.actor;

	public IEnumerator PlayAnim(string speaker, string animName, float? durationSec = null)
	{
		var a = Find(speaker);
		if (a == null || string.IsNullOrEmpty(animName)) yield break;

		yield return a.PlayAnim(animName, durationSec);
	}

	// IResolveAnchor: ��ǳ�� ��Ŀ ��ġ ����
	public Transform ResolveAnchor(string speaker)
	{
		var b = FindBind(speaker);
		if (b != null)
		{
			if (b.anchor != null) 
				return b.anchor;                 // ���� ��Ŀ �켱
			if (b.actor != null) 
				return b.actor.transform;        // ������ ��� Ʈ������ ����
		}
		return null; // ���� ������ �������� �ʿ��� ó��
	}
}
