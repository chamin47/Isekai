using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sound : MonoBehaviour
{
    [SerializeField] private Slider _masterSlider;
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _effectSlider;

    private void Start()
    {
        _masterSlider.value = Managers.Sound.GetVolume("MasterVolume");
        _bgmSlider.value = Managers.Sound.GetVolume("BgmVolume");
        _effectSlider.value = Managers.Sound.GetVolume("EffectVolume");

        _masterSlider.onValueChanged.AddListener(value => Managers.Sound.SetMasterVolume(value));
        _bgmSlider.onValueChanged.AddListener(value => Managers.Sound.SetBgmVolume(value));
        _effectSlider.onValueChanged.AddListener(value => Managers.Sound.SetEffectVolume(value));
    }

    private void OnDisable()
    {
        Managers.Sound.SaveVolumeSettings();
    }

    private void OnDestroy()
    {
        Managers.Sound.SaveVolumeSettings();
    }
}
