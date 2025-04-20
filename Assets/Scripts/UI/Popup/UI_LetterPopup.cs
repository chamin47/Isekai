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
		StartCoroutine(typingEffectCo(""));
	}

	private IEnumerator typingEffectCo(string dialogue)
	{
		//foreach (char c in dialogue)
		//{
		//	_letterText.text += c;
		//	yield return new WaitForSeconds(0.05f); // 타자 치는 속도 조절 가능
		//}

		yield return new WaitForSeconds(5.0f);
		Managers.UI.ShowPopupUI<UI_CutScene2Popup>();

		gameObject.SetActive(false);
	}
}
