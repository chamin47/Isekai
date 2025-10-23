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
	[Header("Target")]
	public Camera target;

	[Header("Anchors/Targets")]
	public Transform player;

	// "middle_2" ���� Ÿ�� �� ��
	public Transform middle2A;
	public Transform middle2B;

	[Header("Scale Clamp (optional)")]
	public bool clampScale = true;
	public float minScale = 0.25f;
	public float maxScale = 4f;

	// ���ذ�(�ʱ� ����)
	float baseOrthoSize;
	Vector3 basePos;

	void Awake()
	{
		if (target == null) 
			target = Camera.main;

		if (target == null) 
			return;

		// 2D ����
		target.orthographic = true;

		baseOrthoSize = target.orthographicSize;
		basePos = target.transform.position;
	}

	/// <summary>���� ���¸� ���ذ����� �缳��</summary>
	public void SetBaseFromCurrent()
	{
		if (!target) return;
		baseOrthoSize = target.orthographicSize;
		basePos = target.transform.position;
	}

	/// <summary>middle_2�� Ÿ�� ����</summary>
	public void SetMiddle2Targets(Transform a, Transform b)
	{
		middle2A = a;
		middle2B = b;
	}

	// ���� �����ε�(��Ŀ ����)
	public IEnumerator ZoomTo(float targetScale, float duration)
		=> ZoomTo(targetScale, duration, null);

	public IEnumerator ZoomOutTo(float targetScale, float duration)
		=> ZoomOutTo(targetScale, duration, null);

	/// <summary>
	/// ���� ��� ���� ������ ��(��Ŀ �ɼ�)
	/// </summary>
	public IEnumerator ZoomTo(float targetScale, float duration, string anchorKey)
	{
		if (!target) yield break;

		// ���� Ŭ����/����
		if (clampScale) targetScale = Mathf.Clamp(targetScale, minScale, maxScale);
		else targetScale = Mathf.Max(0.0001f, targetScale);

		float startSize = target.orthographicSize;
		float endSize = baseOrthoSize / targetScale;

		Vector3 startPos = target.transform.position;
		Vector3 endPos = startPos; // �⺻: ��ġ �̵� ����

		if (!string.IsNullOrWhiteSpace(anchorKey) && TryResolveAnchor(anchorKey, out var anchorPos))
			endPos = new Vector3(anchorPos.x, anchorPos.y, startPos.z); // z ����

		float t = 0f;
		while (t < duration)
		{
			t += Time.deltaTime;
			float p = duration > 0f ? Mathf.Clamp01(t / duration) : 1f;
			target.orthographicSize = Mathf.Lerp(startSize, endSize, p);
			target.transform.position = Vector3.Lerp(startPos, endPos, p);
			yield return null;
		}

		// ����
		target.orthographicSize = endSize;
		target.transform.position = endPos;
	}

	/// <summary>
	/// 1.0���� ��û + ��Ŀ ������ ��, �ڵ����� 'base' ��ġ�� ����
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
		if (string.Equals(key, "base", System.StringComparison.OrdinalIgnoreCase))
		{
			pos = basePos;
			return true;
		}

		// �÷��̾�
		if (string.Equals(key, "player", System.StringComparison.OrdinalIgnoreCase))
		{
			if (player)
			{
				pos = player.position;
				return true;
			}
		}

		// middle_2 (�� Ÿ���� �߰���)
		if (string.Equals(key, "middle_2", System.StringComparison.OrdinalIgnoreCase))
		{
			if (middle2A != null && middle2B != null)
			{
				pos = (middle2A.position + middle2B.position) * 0.5f;
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
}
