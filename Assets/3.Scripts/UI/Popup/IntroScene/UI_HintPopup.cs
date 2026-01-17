using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum HintType
{
	CalendarHint,
	ClockHint
}

public class UI_HintPopup : UI_Popup
{
	[SerializeField] private Image _calendarHintImage;
	[SerializeField] private Image _clockHintImage;
	[SerializeField] private Button _closeButton;

	private CalendarHintController _hintController;

	// 이 팝업이 CalendarHint인지 ClockHint인지 여부
	private HintType _hintType;

	private void ClosePopupButton()
	{
		Managers.UI.ClosePopupUI();
		if (_hintType == HintType.CalendarHint)
		{
			_hintController?.Resume();
		}
	}

	public void Init(HintType hintType, CalendarHintController calendarHintController)
	{
		_hintController = calendarHintController;

		_closeButton.onClick.AddListener(ClosePopupButton);

		// 초기에는 두 이미지 모두 비활성화
		_calendarHintImage.gameObject.SetActive(false);
		_clockHintImage.gameObject.SetActive(false);

		_hintType = hintType;

		// 힌트 타입에 따라 이미지 활성화 설정
		if (_hintType == HintType.CalendarHint)
		{
			_calendarHintImage.gameObject.SetActive(true);
			_clockHintImage.gameObject.SetActive(false);
		}
		else if (_hintType == HintType.ClockHint)
		{
			_calendarHintImage.gameObject.SetActive(false);
			_clockHintImage.gameObject.SetActive(true);
		}

		StartCoroutine(GetEnabledImage().CoFadeOut(1f));	
	}

	private Image GetEnabledImage()
	{
		if (_calendarHintImage.gameObject.activeInHierarchy)
		{
			return _calendarHintImage;
		}
		else
		{
			return _clockHintImage;
		}
	}
}