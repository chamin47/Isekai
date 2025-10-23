using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorAnimatorSimple : MonoBehaviour
{
	[SerializeField] private Animator animator;

	Dictionary<string, float> _clipLen;

	[SerializeField] List<string> _clipNames; // 인스펙터 확인용

	void Awake()
	{
		animator = GetComponentInChildren<Animator>();
		BuildClipTable();
	}

	void BuildClipTable()
	{
		_clipLen = new Dictionary<string, float>(32);
		if (animator && animator.runtimeAnimatorController)
		{
			foreach (var clip in animator.runtimeAnimatorController.animationClips)
			{
				_clipNames.Add(clip.name);

				if (clip && !_clipLen.ContainsKey(clip.name))
					_clipLen[clip.name] = clip.length;
			}
		}
	}

	float GetClipLength(string stateName)
	{
		if (string.IsNullOrEmpty(stateName)) 
			return 0f;

		if (_clipLen != null && _clipLen.TryGetValue(stateName, out var len)) 
			return len;

		BuildClipTable();

		return _clipLen.TryGetValue(stateName, out len) ? len : 0f;
	}

	public void SetPose(string stateName)
	{
		Debug.Log(stateName);
		if (!animator || string.IsNullOrEmpty(stateName)) 
			return;

		animator.CrossFade(stateName, 0.08f, 0, 0f);
	}

	public IEnumerator PlayOnce(string stateName, float? durationOverride = null)
	{
		Debug.Log(stateName);
		if (!animator || string.IsNullOrEmpty(stateName)) 
			yield break;

		animator.CrossFade(stateName, 0.05f, 0, 0f);
		float dur = durationOverride ?? GetClipLength(stateName);

		if (dur > 0f) 
			yield return new WaitForSeconds(dur);
	}
}
