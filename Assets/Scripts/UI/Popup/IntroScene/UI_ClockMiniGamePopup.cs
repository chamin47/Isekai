using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_ClockMiniGamePopup : UI_Popup
{
	[Header("Clock Root")]
	[SerializeField] private RectTransform _clockRoot;  // Center
	[SerializeField] private RectTransform _hourHandRoot;
	[SerializeField] private RectTransform _minuteHandRoot;
	[SerializeField] private CanvasGroup _rootCanvasGroup;

	[Header("Hour Hand")]
	[SerializeField] private Sprite _hourSprite;
	private const float _hourWidth = 23.3585f;
	private const float _hourHeight = 119.0532f;

	[Header("Minute Hand")]
	[SerializeField] private Sprite _minuteSprite;
	private const float _minuteWidth = 56.8893f;
	private const float _minuteHeight = 195.9099f;

	[SerializeField] private Button _centerButton;
	[SerializeField] private Button _closeButton;

	[SerializeField] private CanvasGroup _blackOverlay;

	[SerializeField] private Image _centerImage;
	private Material _centerMat;

	private UI_ClockHand _hourHand;
	private UI_ClockHand _minuteHand;

	private Coroutine _uiShakeCoroutine;
	private Vector2 _originAnchoredPos;

	public override void Init()
	{
		base.Init();

		_originAnchoredPos = _clockRoot.anchoredPosition;

		_minuteHand =
			Managers.UI.MakeSubItem<UI_ClockHand>(_minuteHandRoot);
		AlignSubItemToParent(_minuteHand);

		_hourHand =
			Managers.UI.MakeSubItem<UI_ClockHand>(_hourHandRoot);
		AlignSubItemToParent(_hourHand);

		// 회전 기준점(시계 중앙)
		_hourHand.SetCenterRoot(_clockRoot);
		_minuteHand.SetCenterRoot(_clockRoot);

		// 차이점은 팝업에서 주입
		_hourHand.SetSprite(_hourSprite);
		_hourHand.SetSize(_hourWidth, _hourHeight);

		_minuteHand.SetSprite(_minuteSprite);
		_minuteHand.SetSize(_minuteWidth, _minuteHeight);

		// 스프라이트의 어느 지점을 중심으로 회전할지 결정한다.
		_hourHand.SetPivot(new Vector2(0.5f, 0.04f));
		_minuteHand.SetPivot(new Vector2(0.5f, 0.085f));

		_hourHand.SetAngle(ClockMiniGameModel.HourAngle);
		_minuteHand.SetAngle(ClockMiniGameModel.MinuteAngle);

		if (ClockMiniGameModel.IsSolved)
		{
			_hourHand.Lock();
			_minuteHand.Lock();
		}

		_centerButton.onClick.AddListener(CheckAnswer);
		_closeButton.onClick.AddListener(OnCloseButton);

		_centerMat = Instantiate(_centerImage.material);
		_centerImage.material = _centerMat;
		_centerMat.SetFloat(ClockMiniGameModel.OutlineThicknessID, 0f);


		if (!ClockMiniGameModel.HasSeenIntro)
			StartCoroutine(CoPlayIntro());
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Managers.UI.ClosePopupUI();
		}

		if (ClockMiniGameModel.IsSolved)
			return;


		if (ClockMiniGameModel.HasTouchedHand && !ClockMiniGameModel.HasClickedCenter)
		{
			_centerMat.SetFloat(ClockMiniGameModel.OutlineThicknessID, 2f);
		}

		ClockMiniGameModel.HourAngle = _hourHand.CurrentAngle;
		ClockMiniGameModel.MinuteAngle = _minuteHand.CurrentAngle;
	}

	private void AlignSubItemToParent(UI_ClockHand clockHand)
	{
		RectTransform rectTransform = clockHand.GetComponent<RectTransform>();
		rectTransform.anchoredPosition = Vector2.zero;
		rectTransform.localScale = Vector3.one;
		rectTransform.localRotation = Quaternion.identity;
	}

	private void CheckAnswer()
	{
		if (ClockMiniGameModel.IsSolved)
			return;

		ClockMiniGameModel.HasClickedCenter = true;
		_centerMat.SetFloat(ClockMiniGameModel.OutlineThicknessID, 0f);

		bool hourOk =
			Mathf.Abs(Mathf.DeltaAngle(_hourHand.CurrentAngle, 157.4f)) <= 5f;

		bool minuteOk =
			Mathf.Abs(Mathf.DeltaAngle(_minuteHand.CurrentAngle, 0f)) <= 5f;

		if (hourOk && minuteOk)
		{
			OnSuccess();
		}
		else
		{
			StartCoroutine(CoFail());
		}
	}

	private void OnCloseButton()
	{
		Managers.UI.ClosePopupUI(this);

		if (ClockMiniGameModel.IsSolved && CalendarInputModel.IsSolved)
		{
			DoorController door = FindAnyObjectByType<DoorController>();
			door.Open();
		}
	}

	private void OnSuccess()
	{
		if (ClockMiniGameModel.IsSolved)
			return;

		ClockMiniGameModel.IsSolved = true;
		ClockMiniGameModel.HourAngle = _hourHand.CurrentAngle;
		ClockMiniGameModel.MinuteAngle = _minuteHand.CurrentAngle;

		Managers.Sound.Play("mini_answer_correct",Sound.Effect);

		_hourHand.Lock();
		_minuteHand.Lock();
		Debug.Log("성공!");
	}

	private IEnumerator CoPlayIntro()
	{
		ClockMiniGameModel.HasSeenIntro = true;

		var introPopup =
			Managers.UI.ShowPopupUI<UI_ClockSpeechPopup>();

		yield return introPopup.Show(
			"어딘가 시간이 어긋나 있는 것 같다.\n" +
			"올바른 시간을 맞춘 뒤, <b>시계의 중앙</b>을 클릭하자."
		);

		//yield return new WaitForSeconds(4f);
		// 자동으로 닫힘 (팝업 내부에서 처리)
	}


	private IEnumerator CoFail()
	{
		ClockMiniGameModel.InputLocked = true;

		SetInputLock(ClockMiniGameModel.InputLocked);

		Managers.Sound.Play("mini_answer_wrong", Sound.Effect);

		// 이전 쉐이크 중단
		if (_uiShakeCoroutine != null)
		{
			StopCoroutine(_uiShakeCoroutine);
			_clockRoot.anchoredPosition = _originAnchoredPos;
		}

		_uiShakeCoroutine = StartCoroutine(CoShakeUI(0.5f, 5f));

		// 화면 어두워짐
		yield return StartCoroutine(_blackOverlay.FadeCanvas(0.35f, 0.25f));

		// 복구
		yield return StartCoroutine(_blackOverlay.FadeCanvas(0f, 0.25f));

		ClockMiniGameModel.InputLocked = false;

		SetInputLock(ClockMiniGameModel.InputLocked);
	}

	private void SetInputLock(bool inputLocked)
	{
		//_rootCanvasGroup.interactable = !inputLocked;
		_rootCanvasGroup.blocksRaycasts = !inputLocked;
	}

	private IEnumerator CoShakeUI(float duration, float magnitude)
	{
		float elapsed = 0f;

		while (elapsed < duration)
		{
			Vector2 offset = Random.insideUnitCircle * magnitude;
			_clockRoot.anchoredPosition = _originAnchoredPos + offset;

			elapsed += Time.deltaTime;
			yield return null;
		}

		_clockRoot.anchoredPosition = _originAnchoredPos;
	}
}
