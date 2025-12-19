using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_ClockSpeechPopup : UI_Popup
{
	[SerializeField] private Image _blocker;
	[SerializeField] private RectTransform _popupRoot;
	[SerializeField] private TMP_Text _messageText;

	public override void Init()
	{
		base.Init();

		_popupRoot.localScale = Vector3.zero;

		_blocker.gameObject.BindEvent(OnClickBlocker);
	}

	public IEnumerator Show(string message, float autoCloseTime = 4f)
	{
		yield return new WaitForSeconds(0.5f);

		_messageText.text = message;

		_popupRoot.localScale = Vector3.one * 0.7f;
		_popupRoot
			.DOScale(1f, 0.32f)
			.SetEase(Ease.OutBack);
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

	private void OnClickBlocker(PointerEventData data)
	{
		CloseWithAnimation();
	}
}
