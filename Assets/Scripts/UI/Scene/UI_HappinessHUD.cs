using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HappinessHUD : UI_Scene
{
	
	// �ູ���� ���� �̹��� ���� ���
	[SerializeField] private Image _gaugeBarImage;
	[SerializeField] private Image _happyImage;
	[SerializeField] List<Sprite> _happySprites;


    [SerializeField] private Volume _volume;

    private ColorAdjustments _color;

	public override void Init()
	{
		base.Init();

        _volume.profile.TryGet(out _color);

        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;
		canvas.planeDistance = 1;

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
        _gaugeBarImage.fillAmount = happiness / Managers.Happy.MaxHappiness;

		

		if (happiness >= 80f)
		{
			_happyImage.sprite = _happySprites[0];
		}
		else if(happiness >= 60f)
        {
            _happyImage.sprite = _happySprites[1];
        }
        else if (happiness >= 40f)
        {
            _happyImage.sprite = _happySprites[2];
        }
        else if (happiness >= 20f)
        {
            _happyImage.sprite = _happySprites[3];
        }
        else
        {
            _happyImage.sprite = _happySprites[4];
        }

		// ���� ȸ�������� ����
		if (_color != null)
            _color.saturation.Override(-(1f - Managers.Happy.Happiness / Managers.Happy.MaxHappiness) * 100);

    }

    public void ChangeHappiness(float amount)
	{
		Managers.Happy.ChangeHappiness(amount);
    }
}
