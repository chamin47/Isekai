using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(OutlineSelectSprite))]
public class ClockObject : MonoBehaviour
{
	private OutlineSelectSprite _outlineSelectSprite;

	private void Awake()
	{
		_outlineSelectSprite = GetComponent<OutlineSelectSprite>();
		_outlineSelectSprite.OnSelected += OnPointerClick;
	}

	public void OnPointerClick(int index)
	{
		Managers.UI.ShowPopupUI<UI_ClockMiniGamePopup>();
	}

	private void OnDestroy()
	{
		_outlineSelectSprite.OnSelected -= OnPointerClick;
	}
}
