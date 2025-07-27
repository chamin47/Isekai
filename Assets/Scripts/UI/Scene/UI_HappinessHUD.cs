using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HappinessHUD : UI_Base
{
	// 행복도에 따른 이미지 변경 요소
	[SerializeField] private Image _gaugeBarImage;
	[SerializeField] private Image _happyImage;

    [SerializeField] private List<HappinessLevel> _happinessLevels;

    [SerializeField] private Volume _volume;

    private ColorAdjustments _color;

	public override void Init()
	{
        _volume.profile.TryGet(out _color);

        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        _happinessLevels = Managers.DB.HappinessLevels;

        _gaugeBarImage.fillAmount = Managers.Happy.Happiness / Managers.Happy.MaxHappiness;
		UpdateHappinessUI(Managers.Happy.Happiness);		
	}

	private void OnEnable()
	{
		Managers.Happy.OnHappinessChanged += UpdateHappinessUI;
	}

	private void OnDisable()
	{
		Managers.Happy.OnHappinessChanged -= UpdateHappinessUI;
	}

    public void UpdateHappinessUI(float happiness)
    {
        float normalizedHappiness = happiness / Managers.Happy.MaxHappiness;

        UpdateGaugeBar(normalizedHappiness);
        UpdateEmotionSprite(normalizedHappiness); 
        UpdateScreenSaturation(normalizedHappiness);
    }
    private void UpdateGaugeBar(float normalizedValue)
    {
        _gaugeBarImage.fillAmount = normalizedValue;
    }

    private void UpdateEmotionSprite(float normalizedHappiness)
    {        
        _happyImage.sprite = _happinessLevels[_happinessLevels.Count - 1].sprite;
        foreach (var level in _happinessLevels)
        {
            if (normalizedHappiness >= level.threshold)
            {
                _happyImage.sprite = level.sprite;
                break;
            }
        }
    }

    private void UpdateScreenSaturation(float normalizedValue)
    {
        if (_color != null)
        {
            float unhappinessRatio = 1f - normalizedValue;
            _color.saturation.Override(-unhappinessRatio * 100f);
        }
    }

    public void ChangeHappiness(float amount)
	{
		Managers.Happy.AddHappiness(amount);
    }
}
