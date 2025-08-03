using System;
using UnityEngine;

public class HappinessManager
{
	private float _happiness = 50;
	private readonly float _maxHappiness = 100;
	private readonly float _minHappiness = 0;

	public event Action<float> OnHappinessChanged;

	public float Happiness
	{
		get { return _happiness; }
		set
		{
			_happiness = Mathf.Clamp(value, _minHappiness, _maxHappiness);
			UpdateHappinessEffects();
            OnHappinessChanged?.Invoke(_happiness);
        }
	}
	public float MaxHappiness { get { return _maxHappiness; } }
	public float MinHappiness { get { return _minHappiness; } }

	public void Init()
	{
		UpdateHappinessEffects();
		OnHappinessChanged?.Invoke(_happiness); // �ʱ�ȭ �� �̺�Ʈ ȣ��
	}

	private void UpdateHappinessEffects()
	{
		// ��� ���� ������Ʈ �� �߰� ȿ��
		// Managers.UI.UpdateBackgroundColor(_happiness);
	}

	public void AddHappiness(float amount)
	{
		Happiness += amount;
	}
}
