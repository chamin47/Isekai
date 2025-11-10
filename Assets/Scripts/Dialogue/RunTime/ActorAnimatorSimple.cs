using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 단순 애니메이터 제어기.
/// 1. Animator의 클립 길이 테이블을 빌드 
/// 2. 지정 스테이트를 CrossFade 후 길이 기반으로 대기한다.
/// </summary>

public class ActorAnimatorSimple : MonoBehaviour
{
	[SerializeField] private Animator _animator;

	private Dictionary<string, float> _clipLen;

	private void Awake()
	{
		_animator = GetComponentInChildren<Animator>();
		BuildClipTable();
	}

	private void BuildClipTable()
	{
		_clipLen = new Dictionary<string, float>(8);
		if (_animator && _animator.runtimeAnimatorController)
		{
			foreach (var clip in _animator.runtimeAnimatorController.animationClips)
			{
				if (clip != null && !_clipLen.ContainsKey(clip.name))
					_clipLen[clip.name] = clip.length;
            }
		}
    }

	/// <summary>
	/// 존재하지 않는 상태 return 0f
	/// </summary>
	private float GetClipLength(string stateName)
	{
		if (string.IsNullOrWhiteSpace(stateName)) 
			return 0f;

		if (_clipLen != null && _clipLen.TryGetValue(stateName, out var len)) 
			return len;

		BuildClipTable();

		return _clipLen.TryGetValue(stateName, out len) ? len : 0f;
	}

    /// <summary>
    /// 역재생: durationOverride에 -1f 전달
	/// 역재생 처리를 위해서는 Speed 파라미터를 지정해 줘야된다.
    /// </summary>
    public IEnumerator PlayAnim(string stateName, float? durationOverride = null)
	{
		if (!_animator || string.IsNullOrWhiteSpace(stateName)) 
			yield break;

        float clipLength = GetClipLength(stateName);
        float dur = durationOverride ?? clipLength;

        // 역 재생 처리
        if (dur == -1f)
        {
            _animator.Play(stateName, 0, 1f);
            _animator.SetFloat("Speed", -1f);

            yield return WaitForSecondsCache.Get(clipLength);

            _animator.SetFloat("Speed", 1f);
            yield break;
        }

        // 현재 재생하고 있는 애니메이션과 동일하면 무시
        var currentState = _animator.GetCurrentAnimatorStateInfo(0);
		if (currentState.IsName(stateName))
			yield break;

        _animator.CrossFade(stateName, 0.05f, 0, 0f);

		if (dur > 0f) 
			yield return WaitForSecondsCache.Get(dur);
	}
}
