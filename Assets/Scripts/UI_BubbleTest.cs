using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_BubbleTest: MonoBehaviour
{
	[SerializeField] private TMP_Text _text;
	[SerializeField] private CanvasGroup _canvasGroup;
	[SerializeField] private RectTransform _rectTransformPosition;  // UI_BubbleTest
	[SerializeField] private RectTransform _rectTransformSize;		// bubbleImage
	[SerializeField] private bool _fadeCharacter = true;
	private DialogUnit _dialogUnit;

	public event System.Action<GameObject> OnDialogTriggered;

	private bool _isTyping = false;
	private bool _triggeredFadeOut = false;
	private bool _canTrigger = true;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (!_canTrigger) return;

        if (!collision.CompareTag("Player"))
			return;

		_canTrigger = false;

        Managers.Sound.Play("s1_say_impact3", Sound.Effect);
        TriggerDialog();
		Debug.Log("BubbleTest");
	}

	public void SetPosition(Vector3 worldPosition)
	{
		_rectTransformPosition.position = worldPosition + Vector3.up * 1.0f + Vector3.right * 0.2f;
	}

	public IEnumerator ShowText(float speed)
	{
        _canvasGroup.alpha = 1;
        yield return _text.CoTypingEffect(_dialogUnit.dialogs[0], speed, true);
    }

	public void SetDialogUnit(DialogUnit unit)
	{
        _canTrigger = true;
        _dialogUnit = unit;
		_fadeCharacter = unit.fadeCharacter;

		if (_rectTransformSize != null)
		{
			_rectTransformSize.sizeDelta = unit.bubbleSize;
		}
	}

	public IEnumerator FadeAll(float time)
	{
        yield return CoFadeOut(2f);
    }

	private void TriggerDialog()
	{
		Managers.Happy.ChangeHappiness(_dialogUnit.happinessDelta);
		Camera.main.GetComponent<CameraShake>().Shake();

		if (_isTyping)
		{
			_triggeredFadeOut = true; // 타이핑 중엔 나중에 FadeOut하도록 플래그만 설정
		}
		else
		{
			if (_fadeCharacter)
				OnDialogTriggered?.Invoke(_dialogUnit.characterObject);

			FadeOut(); // 타이핑 끝났으면 즉시 실행
		}
	}


	public IEnumerator CoTyping(List<string> lines)
	{
		_text.text = "";
		_canvasGroup.alpha = 1;
		_isTyping = true;
		_triggeredFadeOut = false;

		foreach (var line in lines)
		{
			foreach (char c in line)
			{
				_text.text += c;
				yield return new WaitForSeconds(0.05f);
			}
			_text.text += "\n";
			yield return new WaitForSeconds(0.5f);
		}

		_isTyping = false;

		if (_triggeredFadeOut)
		{
			if (_fadeCharacter)
			{
				OnDialogTriggered?.Invoke(_dialogUnit.characterObject);
			}

			FadeOut();
		}
	}

	public void FadeOut()
	{
		StartCoroutine(CoFadeOut());
	}

	private IEnumerator CoFadeOut(float time = 1f)
	{
		float t = 1f;
		while (t > 0f)
		{
			t -= Time.deltaTime / time;
            _canvasGroup.alpha = t;
			yield return null;
		}

		_text.text = "";
    }
}