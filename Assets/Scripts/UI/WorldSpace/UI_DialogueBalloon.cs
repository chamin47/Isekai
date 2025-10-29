using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Febucci.UI.Core;

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

		float preferWidth = Mathf.Clamp(_label.preferredWidth + 0.5f, 1f, 4.4f);
		float preferHeight = Mathf.Max(_label.preferredHeight * 1.1f, 0.8f);

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

		var baseWorld = cam.ScreenToWorldPoint(Vector3.zero);
		var offWorld = cam.ScreenToWorldPoint(
			new Vector3(_screenOffset.x, _screenOffset.y + _extraOffsetY, 0f)) - baseWorld;

		float bubbleHalfWidth = _image.rectTransform.sizeDelta.x * 0.5f;

		var pos = _anchor.position + offWorld;
		pos.x += bubbleHalfWidth * _root.lossyScale.x; // ������ �ݿ�
		pos.z = _anchor.position.z;

		_root.position = pos;
	}

	//private void LateUpdate()
	//{
	//	if (_anchor == null) 
	//		return;
	//	var cam = Camera.main; 
	//	if (cam == null) 
	//		return;

	//	var baseWorld = cam.ScreenToWorldPoint(Vector3.zero);
	//	var offWorld = cam.ScreenToWorldPoint(
	//		new Vector3(_screenOffset.x, _screenOffset.y + _extraOffsetY, 0f)) - baseWorld;

	//	var pos = _anchor.position + offWorld;
	//	pos.z = _anchor.position.z; // 2D�� z ����
	//	_root.position = pos;
	//}

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
		yield return _cg.FadeCanvas(1f, 0.15f);

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

	public override void Init() { }
}
