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

	private bool _isFlipped;

	public GameObject BackPageObject => backPage.gameObject;

	public event Action Finished;

	private void Awake()
	{
		SetFill(1f);
	}

	public void FlipOnce()
	{
		if (_isFlipped)
			return;

		_isFlipped = true;

		SetFill(0.287f);

		StartCoroutine(CoFlip());
	}

	private IEnumerator CoFlip()
	{
		float t = 0f;
		while (t < flipDuration) 
		{ 
			t += Time.deltaTime; 
			float fill = 0.287f - (t / flipDuration); 
			SetFill(fill); 
			yield return null; 
		}

		SetFill(0f);
		Finished?.Invoke();
	}

	private void SetFill(float value)
	{
		frontPage.material.SetFloat("_Fill", value);
	}
}
