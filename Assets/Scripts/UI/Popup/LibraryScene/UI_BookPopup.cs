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

	private LibraryBook _book;
	private LibraryScene _libraryScene;

    public override void Init()
	{
		base.Init();
		SetText();

        _libraryScene = Managers.Scene.CurrentScene as LibraryScene;
		_libraryScene.SetLightOff();
    }

	public void Init(LibraryBook book)
	{
		Managers.Sound.Play("book", Sound.Effect);
        _book = book;
    }

	public void ClosePopup()
	{
        _book.SetCanClicked();
		_book.StartFingerBlink();
        _libraryScene.SetLightOn();
        Managers.UI.ClosePopupUI(this);
	}

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
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
}
