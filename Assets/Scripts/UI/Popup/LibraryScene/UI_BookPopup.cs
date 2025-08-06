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

	private LibraryBook _book;
	private LibraryScene _libraryScene;

    public override void Init()
	{
		base.Init();
		SetText();

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
		_book.StartFingerBlink();

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
}
