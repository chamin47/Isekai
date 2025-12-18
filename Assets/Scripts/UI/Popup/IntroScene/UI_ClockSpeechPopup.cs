using System.Collections;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class UI_ClockSpeechPopup : UI_Popup
{
	[SerializeField] private RectTransform _popupRoot;
	[SerializeField] private TMP_Text _messageText;

	private Coroutine _autoCloseCoroutine;

	public override void Init()
	{
		base.Init();

		_popupRoot.localScale = Vector3.zero;
	}

	public IEnumerator Show(string message, float autoCloseTime = 4f)
	{
		yield return new WaitForSeconds(0.5f);

		_messageText.text = message;

		_popupRoot.localScale = Vector3.one * 0.7f;
		_popupRoot
			.DOScale(1f, 0.32f)
			.SetEase(Ease.OutBack);

		if (_autoCloseCoroutine != null)
			StopCoroutine(_autoCloseCoroutine);

		_autoCloseCoroutine = StartCoroutine(CoAutoClose(autoCloseTime));
	}

	private IEnumerator CoAutoClose(float delay)
	{
		yield return new WaitForSeconds(delay);
		CloseWithAnimation();
	}

	private void CloseWithAnimation()
	{
		_popupRoot
			.DOScale(0f, 0.25f)
			.SetEase(Ease.InBack)
			.OnComplete(() =>
			{
				Managers.UI.ClosePopupUI(this);
			});
	}
}
