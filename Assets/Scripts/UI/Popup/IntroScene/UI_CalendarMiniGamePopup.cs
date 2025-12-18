using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UI_CalendarMiniGamePopup : UI_Popup
{
	[Header("UI")]
	[SerializeField] private Button _checkButton;
	[SerializeField] private Button _closeButton;
	[SerializeField] private CanvasGroup _checkCanvas;
	[SerializeField] private Image _hintLeft;
	[SerializeField] private Image _hintRight;

	[SerializeField] private RectTransform _slotRoot;
	[SerializeField] private Image _slotBackground; // 색상 변경용

	[SerializeField] private Transform _yearRoot;   // yyyy
	[SerializeField] private Transform _monthRoot;  // mm
	[SerializeField] private Transform _dayRoot;    // dd

	private UI_CalendarDigit[] _digits = new UI_CalendarDigit[8];
	private CalendarHintController _hintController;
	private Coroutine _hintCoroutine;
	private Coroutine _failCoroutine;

	private Vector2 _originSlotPos;

	public override void Init()
	{
		base.Init();

		_originSlotPos = _slotRoot.anchoredPosition;

		// yyyy
		CreateSlots(
			root: _yearRoot,
			startIndex: 0,
			count: 4
		);

		// mm
		CreateSlots(
			root: _monthRoot,
			startIndex: 4,
			count: 2
		);

		// dd
		CreateSlots(
			root: _dayRoot,
			startIndex: 6,
			count: 2
		);

		_checkButton.onClick.AddListener(OnCheck);
		_closeButton.onClick.AddListener(OnCloseButton);

		_hintController = new CalendarHintController(_hintLeft, _hintRight);

		if (!CalendarInputModel.IsSolved)
			_hintCoroutine = StartCoroutine(_hintController.CoHintLoop());

		RestoreSolvedState();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Managers.UI.ClosePopupUI();
		}
	}

	/// <summary>
	/// SlotRoot 아래에 지정된 개수만큼 슬롯을 생성한다
	/// </summary>
	private void CreateSlots(Transform root, int startIndex, int count)
	{
		for (int i = 0; i < count; i++)
		{
			UI_CalendarDigit digit =
				Managers.UI.MakeSubItem<UI_CalendarDigit>(root);

			digit.transform.localScale = Vector3.one;

			int modelIndex = startIndex + i;
			digit.SetIndex(modelIndex);
			_digits[modelIndex] = digit;
		}
	}

	private void OnCheck()
	{
		if (CalendarInputModel.IsCorrect())
			StartCoroutine(CoSuccess());
		else
			StartCoroutine(CoFail());
	}

	private void OnCloseButton()
	{
		Managers.UI.ClosePopupUI(this);
	}

	private IEnumerator CoSuccess()
	{
		CalendarInputModel.IsSolved = true;

		yield return _checkCanvas.CoFadeIn(1f);
		Destroy(_checkButton.gameObject);

		foreach (var digit in _digits)
			digit.Lock();
	}

	private IEnumerator CoFail()
	{
		// 이미 흔들리는 중이면 중단
		if (_failCoroutine != null)
			yield break;

		_failCoroutine = StartCoroutine(CoShake());
	}

	private IEnumerator CoShake()
	{
		RectTransform rt = _slotRoot;

		float duration = 0.5f;  // 흔들림 전체 지속 시간 (초 단위)
		float strength = 10f;    // 흔들림의 최대 이동 거리(px)
		int vibrato = 4;        // 진동 횟수

		float elapsed = 0f;

		// 색상 변경 (노란색)
		Color originColor = _slotBackground.color;
		_slotBackground.color = new Color(1f, 0.92f, 0.6f);

		while (elapsed < duration)
		{
			float progress = elapsed / duration;
			float damp = 1f - progress;

			float offset =
				Mathf.Sin(progress * vibrato * Mathf.PI * 2f)
				* strength * damp;

			rt.anchoredPosition = _originSlotPos + new Vector2(offset, 0f);

			elapsed += Time.deltaTime;
			yield return null;
		}

		// 원래 상태 복구
		rt.anchoredPosition = _originSlotPos;
		_slotBackground.color = originColor;

		_failCoroutine = null;
	}

	private void RestoreSolvedState()
	{
		if (!CalendarInputModel.IsSolved)
			return;

		// 체크 버튼 제거
		if (_checkButton != null)
			Destroy(_checkButton.gameObject);

		// 체크 캔버스 표시
		_checkCanvas.alpha = 1f;
		_checkCanvas.blocksRaycasts = true;
		_checkCanvas.interactable = true;

		// 모든 Digit 잠금
		foreach (var digit in _digits)
			digit.Lock();
	}

	private void OnDisable()
	{
		if (_hintCoroutine != null)
			StopCoroutine(_hintCoroutine);

		_hintController.Reset();
	}
}
