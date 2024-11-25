using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_BookPopup : UI_Popup
{
    [SerializeField] private Image _letterImage;
    [SerializeField] private TMP_Text _letterText;
	[SerializeField] private TMP_Text _TitleText;
    [SerializeField] private Button _nextButton;

	public override void Init()
	{
		base.Init();
		if (_nextButton == null)
		{
			Debug.LogError("_nextButton�� null�Դϴ�.");
			return;
		}
		_nextButton.onClick.RemoveAllListeners(); // ���� �̺�Ʈ ����
		_nextButton.onClick.AddListener(OnClickNextButton);
		Debug.Log("Next ��ư �̺�Ʈ�� ����Ǿ����ϴ�.");
	}

	public void OnClickNextButton()
	{
		Managers.Resource.Instantiate("UI/Popup/UI_NoticePopup");
		Debug.Log("����ü");
		Destroy(gameObject);
	}
}
