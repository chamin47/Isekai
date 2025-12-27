using UnityEngine;
using UnityEngine.EventSystems;

public class UIClickSoundHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private string _clickDownSound = "click_down";
    [SerializeField] private string _clickUpSound = "click_up";

    public void OnPointerDown(PointerEventData eventData)
    {
        Managers.Sound.Play(_clickDownSound, Sound.Effect);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Managers.Sound.Play(_clickUpSound, Sound.Effect);
    }
}