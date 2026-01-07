using UnityEngine;
using UnityEngine.EventSystems;

public class OutlinePointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	private PolygonOutline _outline;
	private LightToggleController _toggle;

	void Awake()
	{
		_outline = GetComponent<PolygonOutline>();
		_toggle = GetComponent<LightToggleController>();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		Debug.Log("Pointer Enter");
		_outline.Show();
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		_outline.Hide();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		_toggle.Toggle();
	}
}
