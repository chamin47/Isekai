using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeakerBindingSimple
{
	public string speaker;                 // "사서", "Player" 등
	public ActorAnimatorSimple actor;      // 해당 배우
	public Transform anchor;               // 말풍선 기준점(머리 위 등)
}

public class ActorDirectorSimple : MonoBehaviour, IActorDirector, ISpeakerAnchorResolver
{
	[SerializeField] List<SpeakerBindingSimple> bindings = new();

	// 전체 바인딩을 찾는 헬퍼 (앵커/배우 둘 다 필요할 때 사용)
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

	// 배우만 필요할 때는 이것만
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

	// IResolveAnchor: 말풍선 앵커 위치 제공
	public Transform ResolveAnchor(string speaker)
	{
		var b = FindBind(speaker);
		if (b != null)
		{
			if (b.anchor) return b.anchor;                 // 지정 앵커 우선
			if (b.actor) return b.actor.transform;        // 없으면 배우 트랜스폼 폴백
		}
		return null; // 최후 폴백은 프레젠터 쪽에서 처리
	}
}
