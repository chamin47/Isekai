using System.Collections;
using UnityEngine;

/// <summary>
/// 2D(Orthographic) 전용 카메라 서비스
/// - 배율(scale)은 '기준(초기값)' 대비 절대 배율입니다. (ex. 1.4 = 1.4배 줌인)
/// - anchorKey:
///   * "base"   : 초기 위치로 이동
///   * "player" : 플레이어 Transform 위치로 이동
///   * "middle_2": middle2A와 middle2B의 중간점으로 이동
///   * null/빈 문자열/알 수 없는 키: 위치 이동 없음(현재 위치 유지)
/// </summary>
public class CameraService : MonoBehaviour, ICameraService
{
	[Header("Target")]
	public Camera target;

	[Header("Anchors/Targets")]
	public Transform player;

	// "middle_2" 계산용 타깃 두 개
	public Transform middle2A;
	public Transform middle2B;

	[Header("Scale Clamp (optional)")]
	public bool clampScale = true;
	public float minScale = 0.25f;
	public float maxScale = 4f;

	// 기준값(초기 상태)
	float baseOrthoSize;
	Vector3 basePos;

	void Awake()
	{
		if (target == null) 
			target = Camera.main;

		if (target == null) 
			return;

		// 2D 전용
		target.orthographic = true;

		baseOrthoSize = target.orthographicSize;
		basePos = target.transform.position;
	}

	/// <summary>현재 상태를 기준값으로 재설정</summary>
	public void SetBaseFromCurrent()
	{
		if (!target) return;
		baseOrthoSize = target.orthographicSize;
		basePos = target.transform.position;
	}

	/// <summary>middle_2용 타깃 지정</summary>
	public void SetMiddle2Targets(Transform a, Transform b)
	{
		middle2A = a;
		middle2B = b;
	}

	// 간편 오버로드(앵커 없음)
	public IEnumerator ZoomTo(float targetScale, float duration)
		=> ZoomTo(targetScale, duration, null);

	public IEnumerator ZoomOutTo(float targetScale, float duration)
		=> ZoomOutTo(targetScale, duration, null);

	/// <summary>
	/// 기준 대비 절대 배율로 줌(앵커 옵션)
	/// </summary>
	public IEnumerator ZoomTo(float targetScale, float duration, string anchorKey)
	{
		if (!target) yield break;

		// 배율 클램프/보정
		if (clampScale) targetScale = Mathf.Clamp(targetScale, minScale, maxScale);
		else targetScale = Mathf.Max(0.0001f, targetScale);

		float startSize = target.orthographicSize;
		float endSize = baseOrthoSize / targetScale;

		Vector3 startPos = target.transform.position;
		Vector3 endPos = startPos; // 기본: 위치 이동 없음

		if (!string.IsNullOrWhiteSpace(anchorKey) && TryResolveAnchor(anchorKey, out var anchorPos))
			endPos = new Vector3(anchorPos.x, anchorPos.y, startPos.z); // z 고정

		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			float p = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;
			target.orthographicSize = Mathf.Lerp(startSize, endSize, p);
			target.transform.position = Vector3.Lerp(startPos, endPos, p);
			yield return null;
		}

		// 스냅
		target.orthographicSize = endSize;
		target.transform.position = endPos;
	}

	/// <summary>
	/// 1.0으로 요청 + 앵커 미지정 시, 자동으로 'base' 위치로 복귀
	/// </summary>
	public IEnumerator ZoomOutTo(float targetScale, float duration, string anchorKey)
	{
		if (string.IsNullOrWhiteSpace(anchorKey) && Mathf.Approximately(targetScale, 1f))
			anchorKey = "base";

		yield return ZoomTo(targetScale, duration, anchorKey);
	}

	public IEnumerator Shake(float magnitude, float duration)
	{
		if (!target) yield break;

		Vector3 origin = target.transform.localPosition;
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			target.transform.localPosition = origin + (Vector3)Random.insideUnitCircle * magnitude;
			yield return null;
		}
		target.transform.localPosition = origin;
	}

	/// <summary>
	/// 앵커 키 → 월드 좌표 변환
	/// - 알 수 없는 키면 false(위치 이동 없음)
	/// </summary>
	bool TryResolveAnchor(string key, out Vector3 pos)
	{
		key = key?.Trim();
		if (string.IsNullOrEmpty(key))
		{
			pos = default;
			return false;
		}

		// 초기 위치
		if (string.Equals(key, "base", System.StringComparison.OrdinalIgnoreCase))
		{
			pos = basePos;
			return true;
		}

		// 플레이어
		if (string.Equals(key, "player", System.StringComparison.OrdinalIgnoreCase))
		{
			if (player)
			{
				pos = player.position;
				return true;
			}
		}

		// middle_2 (두 타깃의 중간점)
		if (string.Equals(key, "middle_2", System.StringComparison.OrdinalIgnoreCase))
		{
			if (middle2A != null && middle2B != null)
			{
				pos = (middle2A.position + middle2B.position) * 0.5f;
				return true;
			}
		}

		// 알 수 없는 키 → 이동 없음
		pos = default;
		return false;

		// 만약 "모르는 키면 base로 이동"을 원하면 아래처럼 바꾸면 됨:
		// pos = basePos;
		// return true;
	}
}
