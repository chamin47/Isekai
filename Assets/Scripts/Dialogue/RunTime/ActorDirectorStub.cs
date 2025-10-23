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

	public float GetClipLength(string speaker, string animName)
	{
		// TODO: ���� ���������� Animator/DB���� Ŭ�� ���� ��ȯ
		return 1f; // ���� �⺻��
	}

	public IEnumerator PlayOnce(string speaker, string animName, float? durationSec = null)
	{
		float dur = durationSec ?? GetClipLength(speaker, animName);
		if (dur < 0f) dur = 0f;
		Debug.Log($"PlayAnim: {speaker} => {animName} for {dur}s");
		yield return new WaitForSeconds(dur);
	}
}
