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

	public IEnumerator PlayOnce(string speaker, string animName, float durationSec)
	{
		Debug.Log($"PlayAnim: {speaker} => {animName} for {durationSec}s");
		yield return new WaitForSeconds(durationSec);
	}
}
