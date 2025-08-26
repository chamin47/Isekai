using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_PrologueBookPopup : UI_Popup
{
	[Header("Toggle")]
	[SerializeField] private Sub_Toggle _historyToggle;

	[Header("Label")]
	[SerializeField, TextArea] private string _label = "지금까지의 이야기 보기";
	[SerializeField] private float _charInterval = 0.075f;

	[Header("FX")]
	[SerializeField] private Image _fadeImage;  // 화면 전체 페이드
	[SerializeField] private Sprite _onImage;  

	private bool _isNavigating;

	private void Awake()
	{
		GetComponent<Canvas>().worldCamera = Camera.main;
	}

	public override void Init()
	{
		base.Init();

		_historyToggle.Toggle.onValueChanged.AddListener(OnToggleChanged);
		_historyToggle.Toggle.SetIsOnWithoutNotify(false);
		_historyToggle.Toggle.interactable = false;
		_historyToggle.Toggle.onValueChanged.Invoke(false); 

		StartCoroutine(TypeLabelThenEnable());
	}

	private IEnumerator TypeLabelThenEnable()
	{
		//yield return WaitForSecondsCache.Get(1f);

		//// 라벨 타이핑(문자마다 효과음)
		//if (_historyToggle.Text != null)
		//{
		//	_historyToggle.Text.text = string.Empty;
		//	int hit = 0;
		//	foreach (char ch in _label)
		//	{
		//		_historyToggle.Text.text += ch;

		//		if (ch != ' ' && ch != '\n')
		//		{
		//			hit++;
		//			if (hit % 2 == 0)
		//				Managers.Sound.Play("intro_type_short", Sound.Effect);
		//		}
		//		yield return WaitForSecondsCache.Get(_charInterval);
		//	}
		//}

		yield return _fadeImage.CoFadeIn(2f, 2f);
		_historyToggle.Toggle.interactable = true;
		_historyToggle.BackGround.SetActive(true);
	}

	private void OnToggleChanged(bool on)
	{
		if (!on || _isNavigating) return;

		_isNavigating = true;
		_historyToggle.Toggle.interactable = false;
	
			_historyToggle.Toggle.image.sprite = _onImage;

		StartCoroutine(GoToIntro2());
	}

	private IEnumerator GoToIntro2()
	{
		if (_fadeImage) yield return _fadeImage.CoFadeOut(2f, 1f);

		yield return WaitForSecondsCache.Get(2f);

		Managers.UI.ShowSceneUI<UI_Intro2Video>();   // 영상 재생 UI
		Managers.UI.ClosePopupUI(this);
	}

	private void OnDestroy()
	{
		if (_historyToggle != null && _historyToggle.Toggle != null)
			_historyToggle.Toggle.onValueChanged.RemoveListener(OnToggleChanged);
	}
}
