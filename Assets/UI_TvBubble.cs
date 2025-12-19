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
		{
			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
			{
				if (Input.GetMouseButtonDown(0))
					Managers.Sound.Play("click_down", Sound.Effect);

				_typewriter.SkipTypewriter();
			}
			yield return null;
		}

		if (Input.GetMouseButton(0))
		{
			yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
			Managers.Sound.Play("click_up", Sound.Effect);
		}

		while (true)
		{
			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
			{
				if (Input.GetMouseButtonDown(0))
				{
					Managers.Sound.Play("click_down", Sound.Effect);
					yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
					Managers.Sound.Play("click_up", Sound.Effect);
				}
				break;
			}

			yield return null;
		}
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
