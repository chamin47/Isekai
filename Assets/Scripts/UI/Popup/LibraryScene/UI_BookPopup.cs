using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_BookPopup : UI_Popup
{
    [SerializeField] private TMP_Text _letterText;
	[SerializeField] private TMP_Text _TitleText;
	[SerializeField] private Button _AnyClick;

    [Header("0:Vinter, 1:Chaumm, 2:Gang, 3:Pelmanus")]
    [SerializeField] private List<Sprite> _icon;
    [SerializeField] private Image innerPotrait;

	private LibraryBook _book;
	private LibraryScene _libraryScene;

    public override void Init()
	{
		base.Init();
        //SetText();
        SetImage();

        if (Managers.Scene.CurrentScene is LibraryScene libraryScene)
        {
            _libraryScene = libraryScene;
            _libraryScene.SetLightOff();
        }
    }

	public void Init(LibraryBook book)
	{
		Managers.Sound.Play("book", Sound.Effect);
        _book = book;
    }

	public void ClosePopup()
	{
        _libraryScene.SetLightOn();
        _book.EnableClick();
        //_book.StartFingerBlink();
        _book.EnableFinger();

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

        _TitleText.text = Managers.DB.GetLibrarySceneData(currentWorldType).bookData.title;
        _letterText.text = Managers.DB.GetLibrarySceneData(currentWorldType).bookData.content;
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
}
