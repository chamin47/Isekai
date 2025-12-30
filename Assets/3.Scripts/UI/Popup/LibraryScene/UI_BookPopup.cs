using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_BookPopup : UI_Popup
{
	[SerializeField] private TMP_Text _letterText;
	[SerializeField] private TMP_Text _TitleText;
	[SerializeField] private Button _AnyClick;
	[SerializeField] private Image _selectImage;

	[Header("0:Vinter, 1:Chaumm, 2:Gang, 3:Pelmanus")]
	[SerializeField] private List<Sprite> _icon;
	[SerializeField] private Image innerPotrait;

	private LibraryBook _book;
	private Color defaultColor;
	private GameObject _hud;

	[SerializeField] private GameObject _decisionHintBubble;
	[SerializeField] private TMP_Text _decisionHintText;
	[SerializeField] private CanvasGroup _canvasGroup;
	private const float _decisionFadeTime = 1f;


	public override void Init()
	{
		base.Init();

		SetImage();

		_selectImage.gameObject.BindEvent(OnClickdecision);
		_selectImage.gameObject.BindEvent(OnPointerBookEnter, UIEvent.Enter);
		_selectImage.gameObject.BindEvent(OnPointerBookExit, UIEvent.Exit);

		defaultColor = _selectImage.color;

		_decisionHintBubble.SetActive(false);
		_decisionHintText.text = "결정하면 아드리안의 세계로 향합니다.";
	}

	public void Init(LibraryBook book, GameObject hud)
	{
		Managers.Sound.Play("book", Sound.Effect);
		_book = book;
		_hud = hud;
	}

	public void ClosePopup()
	{
		_book.EnableClick();

		_book.EnableFinger();

		Managers.UI.ClosePopupUI(this);
		_hud.SetActive(true);
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			ClosePopup();
		}
	}

	private void SetImage()
	{
		WorldType currentWorldType = Managers.World.CurrentWorldType;

		switch (currentWorldType)
		{
			case WorldType.Vinter:
				innerPotrait.sprite = _icon[0];
				break;
			case WorldType.Chaumm:
				innerPotrait.sprite = _icon[1];
				break;
			case WorldType.Gang:
				innerPotrait.sprite = _icon[2];
				break;
			case WorldType.Pelmanus:
				innerPotrait.sprite = _icon[3];
				break;
		}
	}

	private void OnClickdecision(PointerEventData eventData)
	{
		StartCoroutine(CoDecisionFadeSequence());
	}

	public void OnPointerBookEnter(PointerEventData eventData)
	{
		if (eventData.pointerEnter == _selectImage.gameObject)
		{
			_selectImage.color = Color.yellow;
			_decisionHintBubble.SetActive(true);
		}
	}

	public void OnPointerBookExit(PointerEventData eventData)
	{
		if (eventData.pointerEnter == _selectImage.gameObject)
		{
			_selectImage.color = defaultColor;
			_decisionHintBubble.SetActive(false);
		}
	}

	private IEnumerator CoDecisionFadeSequence()
	{
		// 중복 클릭 방지
		_selectImage.raycastTarget = false;

		Managers.Sound.FadeOutBGM(_decisionFadeTime);

		// UI Fade Out (1s)
		float t = 0f;
		while (t < _decisionFadeTime)
		{
			t += Time.deltaTime;
			_canvasGroup.alpha = 1f - (t / _decisionFadeTime);
			yield return null;
		}
		_canvasGroup.alpha = 0f;

		Managers.UI.ClosePopupUI(this);

		var ui = NoticePopupFactory.CreatePopup(Managers.World.CurrentWorldType);
		ui.Init(_book);
	}
}
