using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(OutlineSelectSprite))]
public class CalendarObject : MonoBehaviour, IPointerClickHandler
{
	public void OnPointerClick(PointerEventData eventData)
	{
		if (TvBubbleRunner.InputLocked)
			return;

		Managers.UI.ShowPopupUI<UI_CalendarMiniGamePopup>();
	}
}
