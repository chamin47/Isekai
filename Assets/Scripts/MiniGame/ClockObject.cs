using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(OutlineSelectSprite))]
public class ClockObject : MonoBehaviour, IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		Managers.UI.ShowPopupUI<UI_ClockMiniGamePopup>();
	}
}
