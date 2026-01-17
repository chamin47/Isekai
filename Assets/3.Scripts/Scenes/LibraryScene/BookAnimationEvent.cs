using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookAnimationEvent : MonoBehaviour
{
	[SerializeField] private SimpleBookFlip _bookFlip;
	[SerializeField] private UI_BookPopup _bookPopup;
	public void OnBookOpenAnimationEnd()
	{
		_bookPopup.BookClickArea.raycastPadding = new Vector4(-407.41f, 0, 0, 0);
		_bookPopup.NextButton.SetActive(true);
		_bookFlip.BackPageObject.SetActive(true);
	}
}
