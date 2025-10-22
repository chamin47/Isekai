using System.Collections;
using UnityEngine;

public class ActorDirectorStub : MonoBehaviour, IActorDirector
{
	public void SetPose(string speaker, string animName)
	{
		// ���� ������Ʈ���� �̸���Animator �����ؼ� state ����
		if (!string.IsNullOrEmpty(animName))
			Debug.Log($"Pose: {speaker} => {animName}");
	}

	public IEnumerator PlayOnce(string speaker, string animName, float durationSec)
	{
		Debug.Log($"PlayAnim: {speaker} => {animName} for {durationSec}s");
		yield return new WaitForSeconds(durationSec);
	}
}
