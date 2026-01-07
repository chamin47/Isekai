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
	}

	private void SetSprite()
	{
		_renderer.sprite = _isOn ? _bgLightOn : _bgLightOff;
	}
}
