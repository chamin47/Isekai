using UnityEngine;
using UnityEngine.EventSystems;

public class ClickSoundHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Managers.Sound.Play("click_down", Sound.Effect);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Managers.Sound.Play("click_up", Sound.Effect);
    }
}