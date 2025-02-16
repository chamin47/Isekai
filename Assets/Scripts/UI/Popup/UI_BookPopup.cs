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
	[SerializeField] private Button _AnyClick;
	[SerializeField] Material[] _material;
	private MeshRenderer _meshRenderer;
	private GameObject _book;

	public override void Init()
	{
		base.Init();
		SetText();

		_meshRenderer = GameObject.Find("Quad").GetComponent<MeshRenderer>();
		_meshRenderer.material = _material[0];
		_book = GameObject.FindGameObjectWithTag("Book");
		_book.SetActive(false);
	}

	public void ClosePopup()
	{
		Managers.UI.ClosePopupUI(this);
	}

	private void SetText()
	{
		WorldType currentWorldType = Managers.World.CurrentWorldType;

		switch (currentWorldType)
		{
			case WorldType.Vinter:
				SetVinter();
				break;
			case WorldType.Chaumm:
				SetChaumm();
				break;
			case WorldType.Gang:
				SetGang();
				break;
			case WorldType.Pelmanus:
				SetPelmanus();
				break;
		}
	}

	private void SetVinter()
	{
		_TitleText.text = "Title\r\n\r\n���͹�Ʈ ����";
		_letterText.text = "� ���̿� ���� ģȭ���� �����ϸ� ���� �ɷ��� �����޾� �̸� �ñ⿡ �������� �̾�޾Ҵ�. ���� ������ \"���͹�Ʈ\"�� �� �������� �̸� ������ �پ ����� ������ �ڶ��Ѵ�.";
	}

	private void SetChaumm()
	{
		_TitleText.text = "Title\r\n\r\n������";
		_letterText.text = "�� ����� �ܸ�� ���� �Ƹ���ٴ� ǥ���� ������ ������ ������ �λ��� �����. ������� ���� ���� �������� ����� �����ϸ�, ���� �̸��� �� ���� �� ��ü�� �ǹ��Ѵ�.";
	}

	private void SetGang()
	{
		_TitleText.text = "Title\r\n\r\n���׸�";
		_letterText.text = "�ΰ��� ����� Ư���� ��� ���� ��Ư�� �����̴�. �ΰ��� ���õ� ������ ����� �پ �������� ����� ������ �䳻�� �� ���� ��â���� �ŷ��� �߻��Ѵ�.";
	}

	private void SetPelmanus()
	{
		_TitleText.text = "Title\r\n\r\n�縶����";
		_letterText.text = "�װ� �¾ ����, ������ ��ܽ����� �������. �״� �׾�� ������� ����� �� �ִ� �ź�ο� �ɷ��� ���ϰ� �־�, ������ �̵��� ���� �̸��� �ܿ�� �����ߴ�.";
	}

	private void OnDestroy()
	{
		_meshRenderer.material = _material[1];
		_book.SetActive(true);
	}
}
