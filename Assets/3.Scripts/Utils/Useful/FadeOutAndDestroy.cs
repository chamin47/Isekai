using System.Collections;
using UnityEngine;

/// <summary>
/// 자신(또는 자식들)의 SpriteRenderer 알파를 서서히 낮춰 파괴.
/// Animator가 색을 덮어쓰는 문제를 피하려고 페이드 시작 시 Animator를 잠깐 끕니다.
/// </summary>
public class FadeOutAndDestroy : MonoBehaviour
{
	[SerializeField, Range(0.05f, 3f)] private float _duration = 0.6f;
	[SerializeField] private bool _disableAnimators = true;

	public void Play(float duration)
	{
		_duration = duration;
		StartCoroutine(CoFade());
	}

	private IEnumerator CoFade()
	{
		if (_disableAnimators)
		{
			var anims = GetComponentsInChildren<Animator>(true);
			for (int i = 0; i < anims.Length; i++) anims[i].enabled = false;
		}

		var srs = GetComponentsInChildren<SpriteRenderer>(true);
		if (srs.Length == 0)
		{
			Destroy(gameObject);
			yield break;
		}

		var start = new Color[srs.Length];
		for (int i = 0; i < srs.Length; i++) start[i] = srs[i].color;

		float t = 0f;
		while (t < _duration)
		{
			t += Time.deltaTime;
			float a = Mathf.Lerp(1f, 0f, t / _duration);

			for (int i = 0; i < srs.Length; i++)
			{
				var c = start[i];
				c.a = a;
				srs[i].color = c;
			}
			yield return null;
		}

		Destroy(gameObject);
	}
}
