using System.Collections;
using UnityEngine;

public class CameraService : MonoBehaviour, ICameraService
{
	public Camera target;

	// ���� ���ذ�(��scale = 1.0���� �ǹ��ϴ� ��)
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

	// targetScale: ���ذ��� ���� "���� ����" (��: 2.5�� ���ؿ��� 2.5�� ����)
	public IEnumerator ZoomTo(float targetScale, float duration)
	{
		if (!target) yield break;

		bool ortho = target.orthographic;
		float start = ortho ? target.orthographicSize : target.fieldOfView;

		// �ٽ� ����: 'start'�� �ƴ϶� '���ذ�'�� �������� ��ǥ ���
		float end = (ortho ? baseOrthoSize : baseFov) / Mathf.Max(0.0001f, targetScale);

		float t = 0;
		while (t < duration)
		{
			t += Time.deltaTime;
			float v = Mathf.Lerp(start, end, Mathf.Clamp01(t / duration));
			if (ortho) target.orthographicSize = v; else target.fieldOfView = v;
			yield return null;
		}

		// ������ ����(�帮��Ʈ ����)
		if (ortho) target.orthographicSize = end; else target.fieldOfView = end;
	}

	public IEnumerator ZoomOutTo(float targetScale, float duration)
	{
		// ���������� targetScale = 1.0 �̸� "���ذ����� ����"
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

	// �ʿ�� ��Ÿ�� �� �� ���¸� ���ذ����� �缳���ϰ� ���� �� ȣ��
	public void SetBaseFromCurrent()
	{
		if (!target) return;
		baseFov = target.fieldOfView;
		baseOrthoSize = target.orthographicSize;
	}
}
