using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 화자와 해당 캐릭터/앵커 트랜스폼을 매핑하는 직렬화 가능한 바인딩 데이터.
/// 대화 시스템에서 화자명으로 애니/말풍선 앵커를 찾을 때 사용한다.
/// </summary>
[System.Serializable]
public class SpeakerBindingSimple
{
	public string speaker;                 // "사서", "Player" 등
	public ActorAnimatorSimple actor;      // 해당 캐릭터에 달린 애니메이터 제어용
	public Transform anchor;               // 말풍선 기준점(머리 위 등)
}

/// <summary>
/// 간단한 액터 디렉터.
/// 화자명으로 ActorAnimator/앵커를 해석해 애니메이션 재생 및 말풍선 앵커 제공을 담당한다.
/// </summary>
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

	// ActorAnimator만 필요할 때는 이것만
	ActorAnimatorSimple Find(string speaker) => FindBind(speaker)?.actor;

	public IEnumerator PlayAnim(string speaker, string animName, float? durationSec = null)
	{
		var a = Find(speaker);
		if (a == null || animName == "null" || string.IsNullOrEmpty(animName)) yield break;

		yield return a.PlayAnim(animName, durationSec);
	}

	// IResolveAnchor: 말풍선 앵커 위치 제공
	public Transform ResolveAnchor(string speaker)
	{
		var b = FindBind(speaker);
		if (b != null)
		{
			if (b.anchor != null) 
				return b.anchor;                 // 지정 앵커 우선
			if (b.actor != null) 
				return b.actor.transform;        // 없으면 배우 트랜스폼 폴백
		}
		return null; // 최후 폴백은 프레젠터 쪽에서 처리
	}

	public Transform GetTransform(string speaker)
	{
		var a = Find(speaker);
		if (a != null)
			return a.transform;
		return null;
    }
}
