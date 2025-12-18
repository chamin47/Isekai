using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarCreate : MonoBehaviour
{
	[SerializeField] private Button _createButton;

	private void Awake()
	{
		_createButton.onClick.AddListener(Create);
	}

	private void Create()
	{
		Managers.UI.ShowPopupUI<UI_CalendarMiniGamePopup>();
	}
}
