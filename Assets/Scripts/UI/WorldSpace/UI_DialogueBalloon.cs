using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Febucci.UI.Core;
using DG.Tweening;
using System.Security.Cryptography;

/// <summary>
/// ���� �����̽� ��ȭ ��ǳ��.
/// ���Ÿ���� ����/��ŵ/Ŭ�� ���� ���̵� �Ρ��ƿ�, ��� ���� ��ġ�� �߰� �������� �����Ѵ�.
/// </summary>
public class UI_DialogueBalloon : UI_Base
{
	[SerializeField] private RectTransform _root;   // ��ǳ�� ��Ʈ
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

	public void AddStackOffset(float dy)
	{
		_extraOffsetY += dy;
		SetPosition(); 
	}

	public void Init(Transform anchor, string text)
	{
		_anchor = anchor;

		_cg.alpha = 0f;

		Color color = _label.color;
		color.a = 0f;           
		_label.color = color;

		_label.text = text.RemoveRichTags();

		FixBubbleSize();
		
		SetPosition();
	}

	public void Appear(string text)
	{
		this.gameObject.SetActive(true);
        Color color = _label.color;
        color.a = 1f;
        _label.color = color;
        _cg.alpha = 1f;
		_typewriter.ShowText(text);
    }

	private Coroutine _fadeCoroutine;
	public void AppearAndFade(string text)
	{
		if (this.gameObject.activeInHierarchy)
			return;

        this.gameObject.SetActive(true);
        Color color = _label.color;
        color.a = 1f;
        _label.color = color;
        _cg.alpha = 1f;
		_typewriter.ShowText(text);

        if (_fadeCoroutine != null)
			StopCoroutine(_fadeCoroutine);
		_fadeCoroutine = StartCoroutine(CoFade());
    }

	private IEnumerator CoFade()
	{
		float elapsed = 0f;
		float duration = 2f;
		float waitTime = 2f;
		yield return new WaitForSeconds(waitTime);

		while(elapsed < duration)
		{
			_cg.alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
			elapsed += Time.deltaTime;
        }

		Disappear();
    }

	public void Disappear()
	{
		_cg.alpha = 0f;
		this.gameObject.SetActive(false);
    }

	[ContextMenu("FixBubbleSize")]
	private void FixBubbleSize()
	{
		Debug.Log($"Length: {_label.text.Length} Width: {_label.preferredWidth} Height: {_label.preferredHeight}");

		_label.ForceMeshUpdate();

		int lineCount = Mathf.Max(1, _label.textInfo.lineCount);

		float baseHeight = 0.7f;      // 한 줄일 때 기본 높이
		float perLineIncrease = 0.23f; // 한 줄 추가될 때마다 증가하는 높이
		float padding = 0.1f;

		float preferHeight = baseHeight + padding + (lineCount - 1) * perLineIncrease;

		float preferWidth = Mathf.Clamp(_label.preferredWidth + 0.5f, 1f, 4.4f);
		//float preferHeight = Mathf.Max(_label.preferredHeight * 1.1f, 0.8f);


		Vector2 preferSize = new Vector2(preferWidth, preferHeight);

		_image.rectTransform.sizeDelta = preferSize;
	}

	private void SetPosition()
	{
		if (_anchor == null)
			return;
		var cam = Camera.main;
		if (cam == null)
			return;

		Debug.LogError(_label.textInfo.lineCount);

		_label.ForceMeshUpdate();
		int lineCount = Mathf.Max(1, _label.textInfo.lineCount);
		float lineOffsetY = (lineCount - 1) * 0.1f * _root.lossyScale.y;
		

		var baseWorld = cam.ScreenToWorldPoint(Vector3.zero);
		var offWorld = cam.ScreenToWorldPoint(
			new Vector3(_screenOffset.x, _screenOffset.y + _extraOffsetY, 0f)) - baseWorld;

		float bubbleHalfWidth = _image.rectTransform.sizeDelta.x * 0.5f;

		var pos = _anchor.position + offWorld;
		pos.y += lineOffsetY;
		pos.x += bubbleHalfWidth * _root.lossyScale.x; // ������ �ݿ�
		pos.z = _anchor.position.z;

		_root.position = pos;
	}

	public IEnumerator CoPresent(string text, float typeSpeed = 0.03f)
	{
		// ���̵� ��
		yield return _cg.FadeCanvas(1f, 0.15f);


		if (_typewriter && _textAnimator)
		{
			this.gameObject.SetActive(true);
            
			Color color = _label.color;
			color.a = 1f;           // ���ĸ� 1���� ����
			_label.color = color;

			_typewriter.ShowText(text); // Start Mode : OnShowText�� �����ص־� ��.

			// ���� �� Ŭ��/�����̽��� ��ŵ
			while (_typewriter.isShowingText)
			{		
				// �Է� ����
				if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
				{
					// ���콺 Ŭ�� �ÿ��� ���� ���
					if (Input.GetMouseButtonDown(0))
						Managers.Sound.Play("click_down", Sound.Effect);

					_typewriter.SkipTypewriter(); // ��� ���� ǥ��
				}

				yield return null;
			}

			// ���� ���� �� �� ������ ���� Up ����
			if (Input.GetMouseButton(0))
			{
				yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
				Managers.Sound.Play("click_up", Sound.Effect);
			}
		}

		// ��ü ��� �� Ŭ��/�����̽� ���
		while (true)
		{
			// �Է� ���� (Space �Ǵ� Mouse)
			if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
			{
				// ����� ���콺 Ŭ���� ����
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

		//// ���̵� �ƿ�
		//yield return _cg.FadeCanvas(0f, 0.12f);
		Destroy(gameObject);
	}

	public IEnumerator CoPresentStacked(string text, float typeSpeed = 0.03f)
	{
		//yield return _cg.FadeCanvas(1f, 0.15f);
		_cg.alpha = 1f;

		PlayPopScale(0.6f, 0.2f);

		if (_typewriter && _textAnimator)
		{
			Color color = _label.color;
			color.a = 1f;           // ���ĸ� 1���� ����
			_label.color = color;

			_typewriter.ShowText(text);
			while (_typewriter.isShowingText)
			{
				if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
					_typewriter.SkipTypewriter();
				yield return null;
			}
		}

		// ��ü ��� �� Ŭ��/Space ���
		while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space))
			yield return null;

		// ���⼭ ����: �ı����� �ʰ� ���ܵ�
	}

	/// 스택 오프셋을 dy만큼 dur초 동안 부드럽게 올리기(기존 벌룬 전용).
	public Tween TweenStackOffset(float dy, float dur = 0.12f, bool unscaled = false, Ease ease = Ease.InCubic)
	{
		_offsetTween?.Kill();
		float end = _extraOffsetY + dy;
		_offsetTween = DOTween
			.To(() => _extraOffsetY, v => { _extraOffsetY = v; SetPosition(); }, end, dur)
			.SetEase(ease)
			.SetUpdate(unscaled)
			.SetTarget(this);
		return _offsetTween;
	}

	/// 팝업(작게→원래 크기). 새로 생성된 스택 벌룬에서만 호출.
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
