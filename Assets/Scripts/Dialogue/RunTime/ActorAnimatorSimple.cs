using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 단순 애니메이터 제어기.
/// Animator의 클립 길이 테이블을 빌드하고 지정 스테이트를 CrossFade 후 길이 기반으로 대기한다.
/// </summary>
public class ActorAnimatorSimple : MonoBehaviour
{
	[SerializeField] private Animator animator;

	private Dictionary<string, float> _clipLen;

	private void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		BuildClipTable();
	}

	private void BuildClipTable()
	{
		_clipLen = new Dictionary<string, float>(32);
		if (animator && animator.runtimeAnimatorController)
		{
			foreach (var clip in animator.runtimeAnimatorController.animationClips)
			{
				if (clip && !_clipLen.ContainsKey(clip.name))
					_clipLen[clip.name] = clip.length;
            }
		}
    }

	private float GetClipLength(string stateName)
	{
		if (string.IsNullOrEmpty(stateName)) 
			return 0f;

		if (_clipLen != null && _clipLen.TryGetValue(stateName, out var len)) 
			return len;

		BuildClipTable();

		return _clipLen.TryGetValue(stateName, out len) ? len : 0f;
	}

	public IEnumerator PlayAnim(string stateName, float? durationOverride = null)
	{
		if (!animator || string.IsNullOrEmpty(stateName)) 
			yield break;

        float clipLength = GetClipLength(stateName);
        float dur = durationOverride ?? clipLength;

        // 역 재생 처리
        if (dur == -1f)
        {
            animator.Play(stateName, 0, 1f);
            animator.SetFloat("Speed", -1f);

            yield return new WaitForSeconds(clipLength);

            animator.SetFloat("Speed", 1f);
            yield break;
        }

        // 현재 재생하고 있는 애니메이션과 동일하면 무시
        var currentState = animator.GetCurrentAnimatorStateInfo(0);
		if (currentState.IsName(stateName))
			yield break;

        animator.CrossFade(stateName, 0.05f, 0, 0f);

		if (dur > 0f) 
			yield return new WaitForSeconds(dur);
	}
}
