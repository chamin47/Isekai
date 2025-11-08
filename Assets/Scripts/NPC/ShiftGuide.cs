using System.Collections;
using UnityEngine;

public class ShiftGuide : MonoBehaviour
{
	[Header("Shift Sprites (밝은색 / 어두운색)")]
	[SerializeField] private Sprite _brightSprite;   // 밝은 색 버전
	[SerializeField] private Sprite _darkSprite;     // 어두운 색 버전

	[Header("Settings")]
	[SerializeField] private float _blinkInterval = 2f;   // 교체 주기
	[SerializeField] private float _fadeOutDuration = 2f; // 사라지는 시간

	private SpriteRenderer _renderer;     
	private DialogueInteraction _dialogueInteraction; // TestScene에서 주입

	private Coroutine _blinkCoroutine;
	private bool _isActive = false;

	private void Start()
	{
		_renderer = GetComponent<SpriteRenderer>();

		if (_dialogueInteraction != null)
			_dialogueInteraction.OnDialogueStarted += HandleDialogueStarted;

		_isActive = true;
		_renderer.enabled = true;
		_blinkCoroutine = StartCoroutine(CoBlinkShift());
	}

	public void SetDialogueInteraction(DialogueInteraction dialogueInteraction)
	{
		_dialogueInteraction = dialogueInteraction;
	}

	private IEnumerator CoBlinkShift()
	{
		bool isBright = false;

		while (_isActive)
		{
			_renderer.sprite = isBright ? _brightSprite : _darkSprite;
			isBright = !isBright;

			yield return WaitForSecondsCache.Get(_blinkInterval / 2f);
		}
	}

	private void OnDestroy()
	{
		if (_dialogueInteraction != null)
			_dialogueInteraction.OnDialogueStarted -= HandleDialogueStarted;
	}

	private void HandleDialogueStarted()
	{
		if (!_isActive)
			return;

		_isActive = false;

		if (_blinkCoroutine != null)
			StopCoroutine(_blinkCoroutine);

		StartCoroutine(CoFadeOutShift());
	}

	private IEnumerator CoFadeOutShift()
	{
		Color c = _renderer.color;
		float startAlpha = c.a;
		float elapsed = 0f;

		while (elapsed < _fadeOutDuration)
		{
			elapsed += Time.deltaTime;
			c.a = Mathf.Lerp(startAlpha, 0f, elapsed / _fadeOutDuration);
			_renderer.color = c;
			yield return null;
		}

		c.a = 0f;
		_renderer.color = c;
		_renderer.enabled = false;
	}
}
