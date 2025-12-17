using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Febucci.UI.Core;
using DG.Tweening;

public class UI_DialogueBalloon : UI_Base
{
	[SerializeField] private RectTransform _root;
	[SerializeField] private Image _image;
	[SerializeField] private TMP_Text _label;
	[SerializeField] private CanvasGroup _cg;
	[SerializeField] private Vector2 _screenOffset = new Vector2(0, 80f);

	[SerializeField] private TAnimCore _textAnimator;
	[SerializeField] private TypewriterCore _typewriter;

	private Transform _anchor;

	private float _extraOffsetY = 0f;

	Tween _offsetTween;
	Tween _popTween;

	private int _maxCharCount;
	private bool _isBrIncluded;

	// 글자 크기 리치 태그 스케일 저장
	private float _sizeScaleFactor = 1f;

	private string _pureText;
	public void AddStackOffset(float dy)
	{
		_extraOffsetY += dy;
		SetPosition();
	}

	public void Init(Transform anchor, string text)
	{
		_anchor = anchor;

		_cg.alpha = 0f;

		// 글자 알파 0 (기존 로직 유지)
		Color color = _label.color;
		color.a = 0f;
		_label.color = color;

		_pureText = text.RemoveRichTags();

		// 리치태그에서 size% 추출 후 캐싱
		ExtractSizeScale(text);

		_label.text = text.KeepOnlySizeTag();

		// 문자 수 계산
		_maxCharCount = CharCountCalculater(text);

		// 말풍선 크기 조절
		FixBubbleSize();

		// 위치 갱신
		SetPosition();
	}

	private void ExtractSizeScale(string text)
	{
		_sizeScaleFactor = 1f;

		int startIndex = text.IndexOf("<size=");
		if (startIndex < 0) return;

		int percentIndex = text.IndexOf('%', startIndex);
		if (percentIndex < 0) return;

		int numberStart = startIndex + "<size=".Length;
		string numStr = text.Substring(numberStart, percentIndex - numberStart);

		if (float.TryParse(numStr, out float value))
		{
			_sizeScaleFactor = Mathf.Pow(value / 100f, 0.75f);
		}
	}

	private int _lineCount;

	private int CharCountCalculater(string text)
	{
		string[] lines;

		if (text.Contains("<br>"))
		{
			_isBrIncluded = true;
			lines = text.Split(new string[] { "<br>" }, System.StringSplitOptions.None);

			_lineCount = lines.Length;
		}
		else
		{
			_isBrIncluded = false;
			lines = new string[] { text };
		}

		int maxCharCount = 0;
		foreach (string line in lines)
		{
			string pure = line.RemoveRichTags();
			int count = pure.Length;
			if (count > maxCharCount)
				maxCharCount = count;
		}

		return maxCharCount;
	}

	public void Appear(string text)
	{
		gameObject.SetActive(true);

		// 글자 알파 복구 (기존 Init의 0을 무조건 1로)
		Color c = _label.color;
		c.a = 1f;
		_label.color = c;

		_cg.alpha = 1f;
		_typewriter.ShowText(text);
	}

	private Coroutine _fadeCoroutine;
	public void AppearAndFade(string text)
	{
		if (gameObject.activeInHierarchy)
			return;

		gameObject.SetActive(true);

		Color c = _label.color;
		c.a = 1f;
		_label.color = c;

		_cg.alpha = 1f;
		_typewriter.ShowText(text);

		if (_fadeCoroutine != null)
			StopCoroutine(_fadeCoroutine);
		_fadeCoroutine = StartCoroutine(CoFade());
	}

	private IEnumerator CoFade()
	{
		float elapsed = 0f;
		float duration = 1f;
		float waitTime = 3f;

		yield return new WaitForSeconds(waitTime);

		while (elapsed < duration)
		{
			_cg.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
			elapsed += Time.deltaTime;
			yield return null;
		}

		Disappear();
	}

	public void Disappear()
	{
		_cg.alpha = 0f;
		gameObject.SetActive(false);
	}

	[ContextMenu("FixBubbleSize")]
	private void FixBubbleSize()
	{
		_label.ForceMeshUpdate();

		int lineCount; 

		float baseHeight = 0.7f;
		float perLineIncrease = 0.23f;
		float padding = 0.1f;

		float preferWidth;
		if (_isBrIncluded)
		{
			lineCount = _lineCount;

			_label.enableWordWrapping = false;
			_label.ForceMeshUpdate();

			float charWidthUnit = 0.12f;
			float baseWidth = 1.1f;
			preferWidth = Mathf.Clamp((baseWidth + _maxCharCount * charWidthUnit) * _sizeScaleFactor, 1f, 4.4f);
		}
		else
		{
			lineCount = Mathf.Max(1, _label.textInfo.lineCount);

			string temp = _label.text;
			_label.text = _pureText;

			_label.enableWordWrapping = true;
			_label.ForceMeshUpdate();

			preferWidth = Mathf.Clamp((_label.preferredWidth + 0.5f) * _sizeScaleFactor, 1f, 4.4f);

			_label.text = temp;
		}

		float preferHeight = (baseHeight + padding + (lineCount - 1) * perLineIncrease)/* * _sizeScaleFactor*/;

		_image.rectTransform.sizeDelta = new Vector2(preferWidth, preferHeight);
	}

	[ContextMenu("SetPosition")]
	private void SetPosition()
	{
		if (_anchor == null) return;

		var cam = Camera.main;
		if (cam == null) return;

		_label.ForceMeshUpdate();
		int lineCount;

		if (_isBrIncluded)
		{
			lineCount = _lineCount;
		}
		else
		{
			lineCount = Mathf.Max(1, _label.textInfo.lineCount);
		}
		
		float lineOffsetY = (lineCount - 1) * 0.1f * _root.lossyScale.y;

		Vector3 anchorWorld = _anchor.position;

		Vector3 viewportOffset = new Vector3(
			_screenOffset.x,
			_screenOffset.y + _extraOffsetY,
			cam.nearClipPlane);

		Vector3 baseWorld = cam.ViewportToWorldPoint(Vector3.zero);
		Vector3 offsetWorld = cam.ViewportToWorldPoint(viewportOffset) - baseWorld;

		float bubbleHalfWidth = _image.rectTransform.sizeDelta.x * 0.5f * _root.lossyScale.x;

		Vector3 pos = anchorWorld + offsetWorld;
		pos.y += lineOffsetY;
		pos.x += bubbleHalfWidth;
		pos.z = anchorWorld.z;

		_root.position = pos;
	}

	public IEnumerator CoPresent(string text, float typeSpeed = 0.03f)
	{
		yield return _cg.FadeCanvas(1f, 0.15f);

		if (_typewriter && _textAnimator)
		{
			gameObject.SetActive(true);

			Color c = _label.color;
			c.a = 1f;
			_label.color = c;

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

		Destroy(gameObject);
	}

	public IEnumerator CoPresentStacked(string text, float typeSpeed = 0.03f)
	{
		_cg.alpha = 1f;

		PlayPopScale(0.6f, 0.2f);

		if (_typewriter && _textAnimator)
		{
			Color c = _label.color;
			c.a = 1f;
			_label.color = c;

			_typewriter.ShowText(text);
			while (_typewriter.isShowingText)
			{
				if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
					_typewriter.SkipTypewriter();
				yield return null;
			}
		}

		while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space))
			yield return null;
	}

	public Tween TweenStackOffset(float dyRatio, float dur = 0.12f, bool unscaled = false, Ease ease = Ease.InCubic)
	{
		_offsetTween?.Kill();
		float end = _extraOffsetY + dyRatio;
		_offsetTween = DOTween
			.To(() => _extraOffsetY, v => { _extraOffsetY = v; SetPosition(); }, end, dur)
			.SetEase(ease)
			.SetUpdate(unscaled)
			.SetTarget(this);
		return _offsetTween;
	}

	public Tween PlayPopScale(float from = 0.85f, float dur = 0.15f, bool unscaled = false, Ease ease = Ease.InCubic)
	{
		_popTween?.Kill();
		var s0 = _root.localScale;
		_root.localScale = s0 * from;
		_popTween = _root
			.DOScale(s0, dur)
			.SetEase(ease)
			.SetUpdate(unscaled)
			.SetTarget(this);
		return _popTween;
	}

	void OnDestroy() => DOTween.Kill(this);

	public override void Init() { }
}
