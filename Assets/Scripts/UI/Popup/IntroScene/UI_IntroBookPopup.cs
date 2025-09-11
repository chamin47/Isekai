using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_IntroBookPopup : UI_Popup
{
	[SerializeField] private TMP_Text _nameTextA;
	[SerializeField] private SpriteRenderer _bookSprite;   // 반드시 할당
	[SerializeField] private PolygonOutline _outline;
	[SerializeField] private Image _fadeImage;

	private UI_EventHandler _evt;

	private void Awake()
	{
		Camera.main.gameObject.GetOrAddComponent<Physics2DRaycaster>();

		GetComponent<Canvas>().worldCamera = Camera.main;
	}

	public override void Init()
	{
		base.Init();

		StartCoroutine(StartSequence());	
	}

	private IEnumerator StartSequence()
	{
		IntroRuntime.LoadIfEmptyFromPrefs();
		if (_nameTextA) _nameTextA.text = IntroRuntime.PlayerName;

		yield return _fadeImage.CoFadeIn(2f, 2f);

		if (!_bookSprite)
		{
			Debug.LogError("[UI_IntroBookPopup] Book SpriteRenderer missing!");
			yield break;
		}

		if (!_outline)
			_outline = _bookSprite.GetComponent<PolygonOutline>();

		// 시작 시 외곽선 꺼두기
		if (_outline != null)
			_outline.Hide();

		// 이벤트 핸들러(책 오브젝트에 붙음)
		_evt = Util.GetOrAddComponent<UI_EventHandler>(_bookSprite.gameObject);
		_evt.OnPointerEnterHandler += OnEnter;
		_evt.OnPointerExitHandler += OnExit;
		_evt.OnPointerUpHandler += OnClick;
	}

	private void OnEnter(PointerEventData _)
	{
		if (_outline != null) _outline.Show();
		//Managers.Sound.Play("s1_glitter2", Sound.Effect);
	}

	private void OnExit(PointerEventData _)
	{
		if (_outline != null) _outline.Hide();
	}

	private void OnClick(PointerEventData _)
	{
		//Managers.UI.ClosePopupUI(this);

		var popup = FindAnyObjectByType<UI_ConfirmNamePopup>();

		if (popup == null)
		{
			Managers.Sound.Play("click_down", Sound.Effect);
			Managers.UI.ShowPopupUI<UI_ConfirmNamePopup>(); // 0004
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
	}
}
