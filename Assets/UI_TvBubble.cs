using System.Collections;
using UnityEngine;
using TMPro;
using Febucci.UI.Core;

public class UI_TvBubble : MonoBehaviour
{
	[SerializeField] private CanvasGroup _cg;

	[Header("Febucci")]
	[SerializeField] private TAnimCore _textAnimator;
	[SerializeField] private TypewriterCore _typewriter;

	public IEnumerator Show(string text)
	{
		_typewriter.ShowText(text);

		while (_typewriter.isShowingText)
			yield return null;

		yield return new WaitForSeconds(0.4f);
	}

	public void FadeOutAndDestroy()
	{
		StartCoroutine(CoFadeOut());
	}

	private IEnumerator CoFadeOut()
	{
		float t = 0f;
		const float dur = 0.25f;

		while (t < dur)
		{
			t += Time.deltaTime;
			_cg.alpha = 1f - (t / dur);
			yield return null;
		}

		Destroy(gameObject);
	}
}
