using UnityEngine.EventSystems;
using UnityEngine;

public class ClickSoundPlayer : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string _clickDownSound = "click_down";
    [SerializeField] private string _clickUpSound = "click_up";

    private bool _pressedInside = false;
    private bool _isPointerOver = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        _pressedInside = true;

        if (!string.IsNullOrEmpty(_clickDownSound))
            Managers.Sound.Play(_clickDownSound, Sound.Effect);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (_pressedInside && _isPointerOver)
        {
            if (!string.IsNullOrEmpty(_clickUpSound))
                Managers.Sound.Play(_clickUpSound, Sound.Effect);
        }

        _pressedInside = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _isPointerOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _isPointerOver = false;
    }
}