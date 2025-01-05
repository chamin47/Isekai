using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LetterPopup : UI_Popup
{
	[SerializeField] private TMP_Text _letterText;
	[SerializeField] private Image _letterImage;

	private string _dialogues = "�� ���迡 ��ģ ���.\n�� ���迡 ������ ���� ���.\n�� ���踦 �����ϰ�\n �̼���� �������� ���\n\n�׷� ��ſ��� ��õ�մϴ�.\n\n'�̼��� ������'���� �ູ�ϰ� �Ƹ��ٿ� �̼��踦 ü���ϰ� �ູ�� ��ã�ƺ�����.";

	public override void Init()
	{
		base.Init();
		_letterText.text = "";
		StartCoroutine(typingEffectCo(_dialogues));
	}

	private IEnumerator typingEffectCo(string dialogue)
	{
		foreach (char c in dialogue)
		{
			_letterText.text += c;
			yield return new WaitForSeconds(0.05f); // Ÿ�� ġ�� �ӵ� ���� ����
		}

		yield return new WaitForSeconds(5.0f);
		Managers.UI.ShowPopupUI<UI_CutScene2Popup>();

		gameObject.SetActive(false);
	}
}
