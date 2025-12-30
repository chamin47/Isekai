using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_ClockHand : UI_Base, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
	[Header("UI")]
	[SerializeField] private RectTransform _handImage;
	[SerializeField] private Image _handSprite;

	[Header("Outline")]
	[SerializeField] private RectTransform _outlineRoot; 
	[SerializeField] private Image[] _outlineImages;       // OL_Up ~ OL_DownLeft (8개)
	[SerializeField] private float _outlineThickness = 4f; 

	[Header("Sound")]
	[SerializeField] private float _soundAngleThreshold = 0.5f;
	[SerializeField] private float _soundStopDelay = 0.08f;

	private RectTransform _centerRoot;   // ClockRoot
	private float _handWidth;
	private float _handHeight;

	private bool _isHovered;     // 포인터가 위에 있는지
	private bool _isDragging;
	private bool _isSoundPlaying;

	private float _lastFrameAngle;
	private float _soundStopTimer;

	public float CurrentAngle { get; private set; }

	/// <summary>
	/// Inspector 배열 순서와 반드시 일치
	/// 0: Up, 1: Down, 2: Left, 3: Right,
	/// 4: UpRight, 5: UpLeft, 6: DownRight, 7: DownLeft
	/// </summary>
	private static readonly Vector2[] DIR_8 =
	{
		new( 0,  1),  // Up
        new( 0, -1),  // Down
        new(-1,  0),  // Left
        new( 1,  0),  // Right
        new( 1,  1),  // UpRight
        new(-1,  1),  // UpLeft
        new( 1, -1),  // DownRight
        new(-1, -1),  // DownLeft
    };

	public override void Init()
	{
		SetOutlineVisible(false);
	}

	public void SetCenterRoot(RectTransform centerRoot)
	{
		_centerRoot = centerRoot;
	}

	public void SetAngle(float angle)
	{
		CurrentAngle = angle;
		_handImage.localRotation = Quaternion.Euler(0, 0, -angle);
		_outlineRoot.localRotation = Quaternion.Euler(0, 0, -angle);
	}

	public void Setup(Sprite sprite, float width, float height, Vector2 pivot, float outlineThickness)
	{
		_handSprite.sprite = sprite;
		_handWidth = width;
		_handHeight = height;
		_outlineThickness = outlineThickness;

		_handImage.pivot = pivot;
		ApplyLayout();

		// Outline들도 동일하게 세팅
		foreach (var img in _outlineImages)
		{
			img.sprite = sprite;
			img.rectTransform.sizeDelta = new Vector2(width, height);
			img.rectTransform.pivot = pivot;
		}

		ApplyOutlineOffset();
	}

	private void ApplyLayout()
	{
		// 바늘 길이 조절
		_handImage.sizeDelta = new Vector2(_handWidth, _handHeight);
	}

	private void ApplyOutlineOffset()
	{
		for (int i = 0; i < _outlineImages.Length; i++)
		{
			RectTransform rt = _outlineImages[i].rectTransform;
			Vector2 dir = DIR_8[i];

			float offset =
				(Mathf.Abs(dir.x) + Mathf.Abs(dir.y) > 1.1f)
				? _outlineThickness * 0.707f
				: _outlineThickness;

			rt.anchoredPosition = dir.normalized * offset;
		}
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

		SetAngle(angle);
	}

	private void UpdateOutline()
	{
		// Hover 중이거나 Drag 중이면 아웃라인 ON
		bool showOutline = _isHovered || _isDragging;

		SetOutlineVisible(showOutline);
	}

	private void SetOutlineVisible(bool visible)
	{
		_outlineRoot.gameObject.SetActive(visible);
	}

	public void Lock()
	{
		_isDragging = false;
		_isHovered = false;
		SetOutlineVisible(false);

		enabled = false;
	}
}
