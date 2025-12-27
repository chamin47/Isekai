using TMPro;
using UnityEngine;

public class FixedTMPInputField : TMP_InputField
{
	protected override void LateUpdate()
	{
		base.LateUpdate(); 

		// 대신 Text의 위치를 완전히 고정
		if (textComponent != null)
			textComponent.rectTransform.localPosition = Vector3.zero;
	}
}
