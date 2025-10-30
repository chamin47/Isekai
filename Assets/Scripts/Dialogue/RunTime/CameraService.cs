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
	[SerializeField] private Camera _target;
	[SerializeField] private CameraController _cameraController;

    [Header("Anchors/Targets")]
	[SerializeField] private Transform _player;

	// "middle_2" 계산용 타깃 두 개
	[SerializeField] private Transform _middle2A;
	[SerializeField] private Transform _middle2B;

	[Header("Scale Clamp")]
	[SerializeField] private bool _clampScale = true;
	[SerializeField] private float _minScale = 0.25f;
	[SerializeField] private float _maxScale = 4f;

	// 기준값(초기 상태)
	private float baseOrthoSize;
	private Vector3 basePos;
	private ActorDirectorSimple _actorDirector;

    void Awake()
	{
		if (_target == null)
			_target = Camera.main;

		if (_target == null) 
			return;

		// 2D 전용
		_target.orthographic = true;

		baseOrthoSize = _target.orthographicSize;
		basePos = _target.transform.position;
	}

	public void Init(ActorDirectorSimple director)
	{
		_actorDirector = director;
    }

	/// <summary>현재 상태를 기준값으로 재설정</summary>
	public void SetBaseFromCurrent()
	{
		if (_target == null) 
			return;
		baseOrthoSize = _target.orthographicSize;
		basePos = _target.transform.position;
	}

	public void EnableCameraUpdate()
	{
		_cameraController?.EnableCameraUpdate();
    }

	public void DisableCameraUpdate()
    {
		_cameraController?.DisableCameraUpdate();
    }

    /// <summary>middle_2용 타깃 지정</summary>
    public void SetMiddle2Targets(Transform a, Transform b)
	{
		_middle2A = a;
		_middle2B = b;
	}

	// 간편 오버로드(앵커 없음)
	public IEnumerator ZoomTo(float targetScale, float duration)
		=> ZoomTo(targetScale, duration, null);

	public IEnumerator ZoomOutTo(float targetScale, float duration)
		=> ZoomOutTo(targetScale, duration, null);

	/// <summary>
	/// 기준 대비 절대 배율로 줌(앵커 옵션)
	/// </summary>
	public IEnumerator ZoomTo(float targetScale, float duration, string anchorKey, EasingType easingType = EasingType.OutCubic)
	{
		if (_target == null) 
			yield break;

		// 배율 클램프/보정
		if (_clampScale) targetScale = Mathf.Clamp(targetScale, _minScale, _maxScale);
		else targetScale = Mathf.Max(0.0001f, targetScale);

		float startSize = _target.orthographicSize;
		float endSize = baseOrthoSize / targetScale;

		Vector3 startPos = _target.transform.position;
		Vector3 endPos = startPos; // 기본: 위치 이동 없음

		if (!string.IsNullOrWhiteSpace(anchorKey) && TryResolveAnchor(anchorKey, out var anchorPos))
			endPos = new Vector3(anchorPos.x, endPos.y, startPos.z); // z 고정

		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			float p = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;

			float eased = EvaluateEasing(easingType, p);

			_target.orthographicSize = Mathf.Lerp(startSize, endSize, eased);
			_target.transform.position = Vector3.Lerp(startPos, endPos, eased);
			yield return null;
		}

		// 스냅
		_target.orthographicSize = endSize;
		_target.transform.position = endPos;
	}

	/// <summary>
	/// 1.0으로 요청 + 앵커 미지정 시, 자동으로 'base' 위치로 복귀
	/// </summary>
	public IEnumerator ZoomOutTo(float targetScale, float duration, string anchorKey, EasingType easingType = EasingType.OutCubic)
	{
		if (string.IsNullOrWhiteSpace(anchorKey) && Mathf.Approximately(targetScale, 1f))
			anchorKey = "center";

		yield return ZoomTo(targetScale, duration, anchorKey, easingType);
	}

	public IEnumerator Shake(float magnitude, float duration)
	{
		if (_target == null) 
			yield break;

		Vector3 origin = _target.transform.localPosition;
		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			_target.transform.localPosition = origin + (Vector3)Random.insideUnitCircle * magnitude;
			yield return null;
		}
		_target.transform.localPosition = origin;
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
		if (string.Equals(key, "center", System.StringComparison.OrdinalIgnoreCase))
		{
			pos = basePos;
			return true;
		}

		// 플레이어
		if (string.Equals(key, "player", System.StringComparison.OrdinalIgnoreCase))
		{
			if (_player != null)
			{
				Debug.Log($"Player Position {_player.position}");
                pos = _player.position;
				return true;
			}
		}

		// middle_2 (두 타깃의 중간점)
		if (string.Equals(key, "middle_2", System.StringComparison.OrdinalIgnoreCase))
		{
			if (_middle2A != null && _middle2B != null)
			{
				pos = (_middle2A.position + _middle2B.position) * 0.5f;
				return true;
			}
		}

		if(_actorDirector != null)
		{
			var actorPos = _actorDirector.GetTransform(key);
			if (actorPos != null)
			{
				Debug.Log($"Actor Position {actorPos.position}");
				pos = actorPos.position;
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

	private float EvaluateEasing(EasingType type, float t)
	{
		switch (type)
		{
			case EasingType.InCubic: return t * t * t;
			case EasingType.OutCubic: return 1f - Mathf.Pow(1f - t, 3f);
			case EasingType.InOutCubic:
				return t < 0.5f
					? 4f * t * t * t
					: 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
			default: return t; // Linear
		}
	}
}
