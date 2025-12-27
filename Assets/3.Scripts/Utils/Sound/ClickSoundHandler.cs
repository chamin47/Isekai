using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSoundHandler : MonoBehaviour
{
    [SerializeField] private string _clickDownSound = "click_down";
    [SerializeField] private string _clickUpSound = "click_up";

    private bool _pressedInside = false;
    private bool _isPointerOver = false;

    private void OnMouseDown()
    {
        if (enabled == false)
            return;

        _pressedInside = true;

        if (!string.IsNullOrEmpty(_clickDownSound))
        {
            Managers.Sound.Play(_clickDownSound, Sound.Effect);
            Debug.Log($"Play Sound: {_clickDownSound}");
        }
    }

    private void OnMouseUp()
    {
        if (enabled == false)
            return;

        if (_pressedInside && _isPointerOver)
        {
            if (!string.IsNullOrEmpty(_clickUpSound))
            {
                Managers.Sound.Play(_clickUpSound, Sound.Effect);
                Debug.Log($"Play Sound: {_clickUpSound}");
            }
        }

        _pressedInside = false;
    }

    private void OnMouseEnter()
    {
        _isPointerOver = true;
    }

    private void OnMouseExit()
    {
        _isPointerOver = false;
    }
}
