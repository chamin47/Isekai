using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LetterPopup : UI_Popup
{
	[SerializeField] private TMP_Text _letterText;
	[SerializeField] private Image _letterImage;

	public override void Init()
	{
		base.Init();

		GetComponent<Canvas>().worldCamera = Camera.main;

		StartCoroutine(WaitUntilProductionEnd());
	}

	private IEnumerator WaitUntilProductionEnd()
	{
		yield return WaitForSecondsCache.Get(5.0f);

		Managers.UI.ShowPopupUI<UI_CutScene2Popup>();
		gameObject.SetActive(false);
	}
}
