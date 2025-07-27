using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Tilemaps.TilemapRenderer;

public class UIManager
{
	private Stack<UI_Popup> _popupStack = new Stack<UI_Popup>(); 
	private UI_Scene _sceneUI = null;

	private int _order = 10;                                        // popup ����
    public UI_Scene SceneUI { get { return _sceneUI; } }

	private GameObject _uiRoot = null;								
    public GameObject Root                                          // UI �θ�
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
	/// sort = true �� ���, �����Ǵ� UI�� �տ� ��ġ�� �ش�.
	/// </summary>
	/// <param name="go">������ ���� ������Ʈ</param>
	/// <param name="sort">ȭ�� ����</param>
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
    /// Ŭ���� �̸��� ���� Prefab �̸��� ��Ī�� �־�� �ȴ�
    /// �ش� �̸��� Prefab�� ���� �����̽��� �����Ѵ�.
    /// </summary>
    /// <returns>�ش� Ŭ����</returns>
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
	/// Sub UI�� �����Ѵ�. �ַ� Toast�������� ������ UI�� ���� ������ UI�� �����Ѵ�.
	/// </summary>
	/// <returns>�ش� Ŭ����</returns>
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
	/// �ش� Scene�� �׻� �����ϴ� UI�� �����Ѵ�.
	/// </summary>
	/// <returns>�ش� Ŭ����</returns>
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
			Debug.LogWarning("���� �˾��� �����ϴ�.");
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
