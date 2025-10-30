using System.Collections;
using UnityEngine;

/// <summary>
/// 2D(Orthographic) ���� ī�޶� ����
/// - ����(scale)�� '����(�ʱⰪ)' ��� ���� �����Դϴ�. (ex. 1.4 = 1.4�� ����)
/// - anchorKey:
///   * "base"   : �ʱ� ��ġ�� �̵�
///   * "player" : �÷��̾� Transform ��ġ�� �̵�
///   * "middle_2": middle2A�� middle2B�� �߰������� �̵�
///   * null/�� ���ڿ�/�� �� ���� Ű: ��ġ �̵� ����(���� ��ġ ����)
/// </summary>
public class CameraService : MonoBehaviour, ICameraService
{
	[SerializeField] private Camera _target;
	[SerializeField] private CameraController _cameraController;

    [Header("Anchors/Targets")]
	[SerializeField] private Transform _player;

	// "middle_2" ���� Ÿ�� �� ��
	[SerializeField] private Transform _middle2A;
	[SerializeField] private Transform _middle2B;

	[Header("Scale Clamp")]
	[SerializeField] private bool _clampScale = true;
	[SerializeField] private float _minScale = 0.25f;
	[SerializeField] private float _maxScale = 4f;

	// ���ذ�(�ʱ� ����)
	private float baseOrthoSize;
	private Vector3 basePos;
	private ActorDirectorSimple _actorDirector;

    void Awake()
	{
		if (_target == null)
			_target = Camera.main;

		if (_target == null) 
			return;

		// 2D ����
		_target.orthographic = true;

		baseOrthoSize = _target.orthographicSize;
		basePos = _target.transform.position;
	}

	public void Init(ActorDirectorSimple director)
	{
		_actorDirector = director;
    }

	/// <summary>���� ���¸� ���ذ����� �缳��</summary>
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

    /// <summary>middle_2�� Ÿ�� ����</summary>
    public void SetMiddle2Targets(Transform a, Transform b)
	{
		_middle2A = a;
		_middle2B = b;
	}

	// ���� �����ε�(��Ŀ ����)
	public IEnumerator ZoomTo(float targetScale, float duration)
		=> ZoomTo(targetScale, duration, null);

	public IEnumerator ZoomOutTo(float targetScale, float duration)
		=> ZoomOutTo(targetScale, duration, null);

	/// <summary>
	/// ���� ��� ���� ������ ��(��Ŀ �ɼ�)
	/// </summary>
	public IEnumerator ZoomTo(float targetScale, float duration, string anchorKey, EasingType easingType = EasingType.OutCubic)
	{
		if (_target == null) 
			yield break;

		// ���� Ŭ����/����
		if (_clampScale) targetScale = Mathf.Clamp(targetScale, _minScale, _maxScale);
		else targetScale = Mathf.Max(0.0001f, targetScale);

		float startSize = _target.orthographicSize;
		float endSize = baseOrthoSize / targetScale;

		Vector3 startPos = _target.transform.position;
		Vector3 endPos = startPos; // �⺻: ��ġ �̵� ����

		if (!string.IsNullOrWhiteSpace(anchorKey) && TryResolveAnchor(anchorKey, out var anchorPos))
			endPos = new Vector3(anchorPos.x, endPos.y, startPos.z); // z ����

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

		// ����
		_target.orthographicSize = endSize;
		_target.transform.position = endPos;
	}

	/// <summary>
	/// 1.0���� ��û + ��Ŀ ������ ��, �ڵ����� 'base' ��ġ�� ����
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
	/// ��Ŀ Ű �� ���� ��ǥ ��ȯ
	/// - �� �� ���� Ű�� false(��ġ �̵� ����)
	/// </summary>
	bool TryResolveAnchor(string key, out Vector3 pos)
	{
		key = key?.Trim();
		if (string.IsNullOrEmpty(key))
		{
			pos = default;
			return false;
		}

		// �ʱ� ��ġ
		if (string.Equals(key, "center", System.StringComparison.OrdinalIgnoreCase))
		{
			pos = basePos;
			return true;
		}

		// �÷��̾�
		if (string.Equals(key, "player", System.StringComparison.OrdinalIgnoreCase))
		{
			if (_player != null)
			{
				Debug.Log($"Player Position {_player.position}");
                pos = _player.position;
				return true;
			}
		}

		// middle_2 (�� Ÿ���� �߰���)
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
		
        // �� �� ���� Ű �� �̵� ����
        pos = default;
		return false;

		// ���� "�𸣴� Ű�� base�� �̵�"�� ���ϸ� �Ʒ�ó�� �ٲٸ� ��:
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
