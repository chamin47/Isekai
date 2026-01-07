using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Portrait : UI_Popup
{
	[SerializeField] private Image _blocker;

	private bool _canClose = false;
	private bool _isClosing = false;

	public override void Init()
	{
		base.Init();

		_blocker.gameObject.BindEvent(OnClickBlocker);
	}

	public IEnumerator ShowAndWait(float showTime)
	{
		yield return WaitForSecondsCache.Get(showTime); // 최소 노출 시간
		_canClose = true;

		// 플레이어 입력 대기
		while (!_isClosing)
			yield return null;
	}

	private void OnClickBlocker(PointerEventData data)
	{
		if (!_canClose || _isClosing)
			return;

		_isClosing = true;
		Destroy(gameObject);
	}

	//public IEnumerator Disapear(float time)
 //   {
 //       yield return WaitForSecondsCache.Get(time);
 //       Destroy(gameObject);
 //   }
}
