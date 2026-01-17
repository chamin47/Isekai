using System;
using System.Collections;
using UnityEngine;

public class SimpleBookFlip : MonoBehaviour
{
	[Header("Pages")]
	[SerializeField] private SpriteRenderer frontPage;
	[SerializeField] private SpriteRenderer backPage;

	[Header("Setting")]
	[SerializeField] private float flipDuration = 1.2f;

	private MaterialPropertyBlock _mpb;

	private bool _isFlipped;

	public GameObject BackPageObject => backPage.gameObject;

	public event Action Finished;

	private void Awake()
	{
		_mpb = new MaterialPropertyBlock();
		SetFill(0.877f);
	}

	public void FlipOnce()
	{
		if (_isFlipped)
			return;

		_isFlipped = true;

		SetFill(0.877f);

		StartCoroutine(CoFlip());
	}

	private IEnumerator CoFlip()
	{
		float t = 0f;
		while (t < flipDuration)
		{
			t += Time.deltaTime;
			float fill = 0.877f - (t / flipDuration);
			SetFill(fill);
			yield return null;
		}

		SetFill(0f);
		Finished?.Invoke();
	}

	private void SetFill(float value)
	{
		frontPage.GetPropertyBlock(_mpb);
		_mpb.SetFloat("_Fill", value);
		frontPage.SetPropertyBlock(_mpb);
	}
}
