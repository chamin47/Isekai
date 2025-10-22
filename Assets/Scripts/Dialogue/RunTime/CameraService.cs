using System.Collections;
using UnityEngine;

public class CameraService : MonoBehaviour, ICameraService
{
	public Camera target;

	// 최초 기준값(“scale = 1.0”이 의미하는 값)
	float baseFov;
	float baseOrthoSize;

	void Awake()
	{
		if (!target) target = Camera.main;
		if (target)
		{
			baseFov = target.fieldOfView;
			baseOrthoSize = target.orthographicSize;
		}
	}

	// targetScale: 기준값에 대한 "절대 배율" (예: 2.5면 기준에서 2.5배 줌인)
	public IEnumerator ZoomTo(float targetScale, float duration)
	{
		if (!target) yield break;

		bool ortho = target.orthographic;
		float start = ortho ? target.orthographicSize : target.fieldOfView;

		// 핵심 수정: 'start'가 아니라 '기준값'을 기준으로 목표 계산
		float end = (ortho ? baseOrthoSize : baseFov) / Mathf.Max(0.0001f, targetScale);

		float t = 0;
		while (t < duration)
		{
			t += Time.deltaTime;
			float v = Mathf.Lerp(start, end, Mathf.Clamp01(t / duration));
			if (ortho) target.orthographicSize = v; else target.fieldOfView = v;
			yield return null;
		}

		// 마무리 스냅(드리프트 방지)
		if (ortho) target.orthographicSize = end; else target.fieldOfView = end;
	}

	public IEnumerator ZoomOutTo(float targetScale, float duration)
	{
		// 관용적으로 targetScale = 1.0 이면 "기준값으로 복귀"
		yield return ZoomTo(targetScale, duration);
	}

	public IEnumerator Shake(float magnitude, float duration)
	{
		if (!target) yield break;
		Vector3 origin = target.transform.localPosition;
		float t = 0;
		while (t < duration)
		{
			t += Time.deltaTime;
			target.transform.localPosition = origin + (Vector3)Random.insideUnitCircle * magnitude;
			yield return null;
		}
		target.transform.localPosition = origin;
	}

	// 필요시 런타임 중 현 상태를 기준값으로 재설정하고 싶을 때 호출
	public void SetBaseFromCurrent()
	{
		if (!target) return;
		baseFov = target.fieldOfView;
		baseOrthoSize = target.orthographicSize;
	}
}
