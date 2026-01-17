using UnityEngine;

public class LightToggleController : MonoBehaviour
{
	[SerializeField] private SpriteRenderer _renderer;
	[SerializeField] private Sprite _bgLightOn;
	[SerializeField] private Sprite _bgLightOff;

	private bool _isOn;

	private void Awake()
	{
		_isOn = false;
		SetSprite();
	}

	public void Toggle()
	{
		_isOn = !_isOn;
		SetSprite();
		PlaySound();
	}

	private void SetSprite()
	{
		_renderer.sprite = _isOn ? _bgLightOn : _bgLightOff;
	}

	private void PlaySound()
	{
		if (_isOn)
		{
			Managers.Sound.Play("light_turn_on1", Sound.Effect);
		}
		else
		{
			Managers.Sound.Play("light_turn_off", Sound.Effect);
		}
	}
}
