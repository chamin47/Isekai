using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_IntroBookPopup : UI_Popup
{
	[Header("UI 참조")]
	[SerializeField] private TMP_Text _nameTextA;
	[SerializeField] private Image _bookImage;
	[SerializeField] private Image _bookHit;
	[SerializeField] private Image _fadeImage;

	public CanvasGroup _canvasGroup;

	// 전용 머티리얼 인스턴스
	private Material _matInstance;
	private UI_EventHandler _evt;

	// 셰이더 프로퍼티 ID (성능 위해 캐시)
	private static readonly int ID_OutlineThickness = Shader.PropertyToID("_OutlineThickness");
	private static readonly int ID_OutlineColor = Shader.PropertyToID("_OutlineColor");

	// 호버 시 두께(px)
	[SerializeField] private float _hoverOutlinePx = 2f;

	private void Awake()
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
	}

	public override void Init()
	{
		base.Init();
		StartCoroutine(StartSequence());
	}

	private IEnumerator StartSequence()
	{
		// 이름 표시
		IntroRuntime.LoadIfEmptyFromPrefs();
		if (_nameTextA) _nameTextA.text = IntroRuntime.PlayerName;

		// 안전 체크
		if (_bookImage == null || _bookImage.sprite == null)
			yield break;

		// 이 Image만 사용하는 전용 머티리얼 인스턴스 생성
		if (_bookImage.material != null)
			_matInstance = new Material(_bookImage.material);
		else
			_matInstance = new Material(Shader.Find("UI/SpritePureOutline"));

		// 초기값: 아웃라인 OFF
		_matInstance.SetFloat(ID_OutlineThickness, 0f);
		_matInstance.SetColor(ID_OutlineColor, Color.white);
		_bookImage.material = _matInstance;

		// 이벤트 연결
		_evt = Util.GetOrAddComponent<UI_EventHandler>(_bookHit.gameObject);
		_evt.OnPointerEnterHandler += OnEnter;
		_evt.OnPointerExitHandler += OnExit;
		_evt.OnPointerUpHandler += OnClick;

		// 페이드 인
		if (_fadeImage != null)
			yield return _fadeImage.CoFadeIn(2f, 2f);
	}

	private void OnEnter(PointerEventData _)
	{
		var popup = FindAnyObjectByType<UI_ConfirmNamePopup>();
		if (popup != null)
			return;

		if (_matInstance)
			_matInstance.SetFloat(ID_OutlineThickness, _hoverOutlinePx); // e.g., 2 px
	}

	private void OnExit(PointerEventData _)
	{
		if (_matInstance)
			_matInstance.SetFloat(ID_OutlineThickness, 0f);
	}

	private void OnClick(PointerEventData _)
	{
		var popup = FindAnyObjectByType<UI_ConfirmNamePopup>();
		if (popup == null)
		{
			Managers.Sound.Play("click_down", Sound.Effect);
			Managers.UI.ShowPopupUI<UI_ConfirmNamePopup>();
		}
	}

	private void OnDestroy()
	{
		if (_evt != null)
		{
			_evt.OnPointerEnterHandler -= OnEnter;
			_evt.OnPointerExitHandler -= OnExit;
			_evt.OnPointerUpHandler -= OnClick;
		}
		if (_matInstance)
		{
#if UNITY_EDITOR
			DestroyImmediate(_matInstance);
#else
            Destroy(_matInstance);
#endif
			_matInstance = null;
		}
	}
}
