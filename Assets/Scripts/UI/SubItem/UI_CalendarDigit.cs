using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UI_CalendarDigit : UI_Base
{
	[Header("UI")]
	[SerializeField] private Button _button;
	[SerializeField] private RectTransform _rollContainer;
	[SerializeField] private TMP_Text _digitTemplate;

	private int _index;
	private bool _locked;

	// 반드시 프리팹의 Digit 높이와 동일해야 함
	private const float DIGIT_HEIGHT = 40f;
	private const float ROLL_SPEED = 300f;

	public void SetIndex(int index)
	{
		_index = index;
		RefreshImmediate();
	}

	public override void Init()
	{
		_button.onClick.AddListener(OnClick);
	}

	public void Lock()
	{
		_locked = true;
		_button.interactable = false;
	}

	private void OnClick()
	{
		if (_locked)
			return;

		int from = CalendarInputModel.Digits[_index];
		int to = (from + 1) % 10;

		CalendarInputModel.Digits[_index] = to;
		StartCoroutine(CoRoll(from, to));
	}

	private IEnumerator CoRoll(int from, int to)
	{
		ClearRoll();

		// 슬롯 머신 시퀀스 생성 (ex: 3 → 4 → 5)
		List<int> sequence = new List<int>();
		int current = from;
		while (current != to)
		{
			sequence.Add(current);
			current = (current + 1) % 10;
		}
		sequence.Add(to);

		// 숫자 생성 (아래에서 위로 쌓음)
		for (int i = 0; i < sequence.Count; i++)
		{
			TMP_Text digit = Instantiate(_digitTemplate, _rollContainer);
			digit.gameObject.SetActive(true);
			digit.text = sequence[i].ToString();
			digit.rectTransform.anchoredPosition =
				new Vector2(0, -DIGIT_HEIGHT * i);
		}

		float targetMove = DIGIT_HEIGHT * (sequence.Count - 1);
		float moved = 0f;

		// RollContainer를 위로 이동
		while (moved < targetMove)
		{
			float delta = Time.deltaTime * ROLL_SPEED;
			moved += delta;
			_rollContainer.anchoredPosition += Vector2.up * delta;
			yield return null;
		}

		RefreshImmediate();
	}

	private void RefreshImmediate()
	{
		ClearRoll();

		TMP_Text digit = Instantiate(_digitTemplate, _rollContainer);
		digit.gameObject.SetActive(true);
		digit.text = CalendarInputModel.Digits[_index].ToString();
		digit.rectTransform.anchoredPosition = Vector2.zero;
	}

	private void ClearRoll()
	{
		_rollContainer.anchoredPosition = Vector2.zero;

		for (int i = _rollContainer.childCount - 1; i >= 0; i--)
		{
			Transform child = _rollContainer.GetChild(i);

			// 템플릿은 절대 삭제하지 않는다
			if (child == _digitTemplate.transform)
				continue;

			Destroy(child.gameObject);
		}
	}

}
