using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_ClockHand : UI_Base, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	[Header("UI")]
	[SerializeField] private RectTransform _handImage;
	[SerializeField] private Image _handSprite;

	[Header("Shader")]
	[SerializeField] private float _outlineHoverThickness = 1.5f;

	[Header("Sound")]
	[SerializeField] private float _soundAngleThreshold = 0.5f;
	[SerializeField] private float _soundStopDelay = 0.08f;

	private RectTransform _centerRoot;   // ClockRoot
	private float _handWidth;
	private float _handHeight;

	private bool _isHovered;     // 포인터가 위에 있는지
	private bool _isDragging;
	private bool _isSoundPlaying;

	private float _lastAngle;
	private float _lastFrameAngle;
	private float _soundStopTimer;

	private Material _runtimeMat;	

	public float CurrentAngle { get; private set; }

	public override void Init()
	{
		_runtimeMat = Instantiate(_handSprite.material);
		_handSprite.material = _runtimeMat;

		// 기본은 아웃라인 OFF
		_runtimeMat.SetFloat(ClockMiniGameModel.OutlineThicknessID, 0f);
	}

	public void SetCenterRoot(RectTransform centerRoot)
	{
		_centerRoot = centerRoot;
	}

	public void SetSprite(Sprite sprite)
	{
		_handSprite.sprite = sprite;
	}

	public void SetSize(float width, float height)
	{
		_handWidth = width;
		_handHeight = height;
		ApplyLayout();
	}

	public void SetAngle(float angle)
	{
		CurrentAngle = angle;
		_handImage.localRotation = Quaternion.Euler(0, 0, -angle);
	}

	public void SetPivot(Vector2 pivot)
	{
		_handImage.pivot = pivot;
	}

	private void ApplyLayout()
	{
		// 바늘 길이 조절
		_handImage.sizeDelta =
			new Vector2(_handWidth, _handHeight);
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (ClockMiniGameModel.IsSolved)
			return;

		_isHovered = true;
		UpdateOutline();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_isHovered = false;

		// 드래그 중이면 꺼지지 않음
		UpdateOutline();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (ClockMiniGameModel.IsSolved)
			return;

		if (_isDragging)
			return;

		_isDragging = true;
		UpdateRotation(eventData);
		UpdateOutline();

		_lastAngle = CurrentAngle;
		_lastFrameAngle = CurrentAngle;
		_soundStopTimer = 0f;
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (!_isDragging)
			return;

		UpdateRotation(eventData);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		_isDragging = false;

		if (!ClockMiniGameModel.HasTouchedHand)
			ClockMiniGameModel.HasTouchedHand = true;

		StopSound();

		UpdateOutline();
	}

	private void Update()
	{
		if (!_isDragging)
			return;

		float delta =
			Mathf.Abs(Mathf.DeltaAngle(_lastFrameAngle, CurrentAngle));

		if (delta > _soundAngleThreshold)
		{
			_soundStopTimer = 0f;

			if (!_isSoundPlaying)
				StartSound();
		}
		else
		{
			_soundStopTimer += Time.deltaTime;

			if (_soundStopTimer >= _soundStopDelay)
				StopSound();
		}

		_lastFrameAngle = CurrentAngle;
	}

	private void StartSound()
	{
		Managers.Sound.PlaySubEffect("mini_clock_spin", 1f);
		_isSoundPlaying = true;
	}

	private void StopSound()
	{
		if (!_isSoundPlaying)
			return;

		Managers.Sound.StopSubEffect();
		_isSoundPlaying = false;
	}

	private void UpdateRotation(PointerEventData eventData)
	{
		Vector2 center =
			RectTransformUtility.WorldToScreenPoint(
				eventData.pressEventCamera, _centerRoot.position);

		Vector2 mouse = eventData.position;
		Vector2 dir = mouse - center;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		angle = 90f - angle;
		if (angle < 0f)
			angle += 360f;

		CurrentAngle = angle;
		_handImage.localRotation = Quaternion.Euler(0, 0, -angle);
	}

	private void UpdateOutline()
	{
		if (_runtimeMat == null)
			return;

		// Hover 중이거나 Drag 중이면 아웃라인 ON
		bool showOutline = _isHovered || _isDragging;

		_runtimeMat.SetFloat(
			ClockMiniGameModel.OutlineThicknessID,
			showOutline ? _outlineHoverThickness : 0f
		);
	}

	public void Lock()
	{
		_isDragging = false;
		_isHovered = false;
		UpdateOutline();

		enabled = false;
	}
}
