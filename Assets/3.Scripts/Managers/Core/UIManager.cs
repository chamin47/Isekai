using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class UIManager
{
	private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>(); 
	private UI_Scene _sceneUI = null;

	private int _order = 10;                                        // popup 정렬
    public UI_Scene SceneUI { get { return _sceneUI; } }

	private GameObject _uiRoot = null;								
    public GameObject Root                                          // UI 부모
    {
		get
		{
			if(_uiRoot == null)
			{
				_uiRoot = new GameObject { name = "@UI_Root" };
            }
			return _uiRoot;
		}
	}

	/// <summary>
	/// sort = true 일 경우, 생성되는 UI를 앞에 배치해 준다.
	/// </summary>
	/// <param name="go">생성할 게임 오브젝트</param>
	/// <param name="sort">화면 순서</param>
	public void SetCanvas(GameObject go, bool sort)
	{
		var canvas = Util.GetOrAddComponent<Canvas>(go);		
        var canvasScaler = Util.GetOrAddComponent<CanvasScaler>(go);

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1920, 1080);

        if (sort)
        {
            canvas.sortingOrder = _order;
            _order++;
        }
    }

    /// <summary>
    /// 클래스 이름과 실제 Prefab 이름을 매칭해 주어야 된다
    /// 해당 이름의 Prefab을 월드 스페이스에 생성한다.
    /// </summary>
    /// <returns>해당 클래스</returns>
    public T MakeWorldSpaceUI<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/WorldSpace/{name}");

		if (parent != null)
			go.transform.SetParent(parent);

		Canvas canvas = go.GetOrAddComponent<Canvas>();
		canvas.renderMode = RenderMode.WorldSpace;
		canvas.worldCamera = Camera.main;

		return Util.GetOrAddComponent<T>(go);
	}

	/// <summary>
	/// Sub UI를 생성한다. 주로 Toast형식으로 생성될 UI나 서브 목적의 UI를 생성한다.
	/// </summary>
	/// <returns>해당 클래스</returns>
	public T MakeSubItem<T>(Transform parent = null, string name = null) where T : UI_Base
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/SubItem/{name}");

		if (parent != null)
			go.transform.SetParent(parent);

		return Util.GetOrAddComponent<T>(go);
	}

	/// <summary>
	/// 해당 Scene에 항상 존재하는 UI를 생성한다.
	/// </summary>
	/// <returns>해당 클래스</returns>
	public T ShowSceneUI<T>(string name = null) where T : UI_Scene
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Scene/{name}");

		T sceneUI = Util.GetOrAddComponent<T>(go);
		_sceneUI = sceneUI;

		go.transform.SetParent(Root.transform);

		return sceneUI;
	}

	public T ShowPopupUI<T>(string name = null) where T : UI_Popup
	{
		if (string.IsNullOrEmpty(name))
			name = typeof(T).Name;

		GameObject go = Managers.Resource.Instantiate($"UI/Popup/{name}");
		T popup = Util.GetOrAddComponent<T>(go);
		_popupStack.Push(popup);

		go.transform.SetParent(Root.transform);

		return popup;
	}

    public void ClosePopupUI(UI_Popup popup)
	{
		if (_popupStack.Count == 0)
		{
			Debug.LogWarning("닫을 팝업이 없습니다.");
			return;
		}

		if (_popupStack.Peek() != popup)
		{
			Debug.Log("Close Popup Failed!");
			return;
		}

		ClosePopupUI();
	}

	public void ClosePopupUI()
	{
		if (_popupStack.Count == 0)
			return;

		UI_Popup popup = _popupStack.Pop();
		Managers.Resource.Destroy(popup.gameObject);

		popup = null;
        _order--;
    }

	public void CloseAllPopupUI()
	{
		while (_popupStack.Count > 0)
			ClosePopupUI();
	}

	public void Clear()
	{
		CloseAllPopupUI();
		_sceneUI = null;
	}
}
