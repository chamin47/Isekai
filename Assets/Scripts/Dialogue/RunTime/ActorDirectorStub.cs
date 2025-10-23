using System.Collections;
using UnityEngine;

public class ActorDirectorStub : MonoBehaviour, IActorDirector
{
	public void SetPose(string speaker, string animName)
	{
		// 실제 프로젝트에선 이름→Animator 매핑해서 state 세팅
		if (!string.IsNullOrEmpty(animName))
			Debug.Log($"Pose: {speaker} => {animName}");
	}

	public float GetClipLength(string speaker, string animName)
	{
		// TODO: 실제 구현에서는 Animator/DB에서 클립 길이 반환
		return 1f; // 스텁 기본값
	}

	public IEnumerator PlayOnce(string speaker, string animName, float? durationSec = null)
	{
		float dur = durationSec ?? GetClipLength(speaker, animName);
		if (dur < 0f) dur = 0f;
		Debug.Log($"PlayAnim: {speaker} => {animName} for {dur}s");
		yield return new WaitForSeconds(dur);
	}
}
