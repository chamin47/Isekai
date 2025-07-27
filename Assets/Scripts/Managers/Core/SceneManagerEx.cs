using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
	private BaseScene _currentScene;
	public BaseScene CurrentScene { 
		get
		{
			if(_currentScene == null)
			{
				_currentScene = GameObject.FindObjectOfType<BaseScene>();
            }

            return _currentScene;
        }
	}
	public Scene prevSceneType { get; private set; } = Scene.Unknown;

    public void LoadScene(Scene type)
	{
		Managers.Clear();

		prevSceneType = CurrentScene?.SceneType ?? Scene.Unknown;
        _currentScene = null;

        Managers.DB.ResetPlayerData();
        SceneManager.LoadScene(GetSceneName(type));
	}

	string GetSceneName(Scene type)
	{
		string name = System.Enum.GetName(typeof(Scene), type);
		return name;
    }

	public void Clear()
	{
		CurrentScene?.Clear();
	}
}
