using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class TitleScene : BaseScene
{
	[SerializeField] private GameObject _endingBackGround;
	[SerializeField] private GameObject _commonBackGround;
	protected override void Init()
	{ 
		SceneType = Scene.TitleScene;

		if(Managers.Game.IsShowEnding)
		{
			_endingBackGround.SetActive(true);
            _commonBackGround.SetActive(false);
        }
		else
		{
            _endingBackGround.SetActive(false);
            _commonBackGround.SetActive(true);
        }

		Managers.World.CurrentWorldType = (WorldType)PlayerPrefs.GetInt("WorldType", 0);
    }

	public override void Clear()
	{
		Debug.Log("TitleScene Clear!");
	}
}
