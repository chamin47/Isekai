using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Sound : MonoBehaviour
{
    [SerializeField] private Button _soundToggleButton;
    private bool _isSoundOn = false;

    [SerializeField] private Slider _masterSlider;
    //[SerializeField] private Slider _bgmSlider;
    //[SerializeField] private Slider _effectSlider;

    private void Start()
    {
        _soundToggleButton.onClick.AddListener(() =>
        {
            ToggleSoundSlider();
        });
        _masterSlider.value = Managers.Sound.GetVolume("MasterVolume");
        //_bgmSlider.value = Managers.Sound.GetVolume("BgmVolume");
        //_effectSlider.value = Managers.Sound.GetVolume("EffectVolume");

        _masterSlider.onValueChanged.AddListener(value => Managers.Sound.SetMasterVolume(value));
        //_bgmSlider.onValueChanged.AddListener(value => Managers.Sound.SetBgmVolume(value));
        //_effectSlider.onValueChanged.AddListener(value => Managers.Sound.SetEffectVolume(value));
        _masterSlider.gameObject.SetActive(false);

        DontDestroyOnLoad(gameObject);
    }

    private void ToggleSoundSlider()
    {
        if (_isSoundOn)
        {
            _masterSlider.gameObject.SetActive(false);
            //_bgmSlider.gameObject.SetActive(false);
            //_effectSlider.gameObject.SetActive(false);
        }
        else
        {
            _masterSlider.gameObject.SetActive(true);
            //_bgmSlider.gameObject.SetActive(true);
            //_effectSlider.gameObject.SetActive(true);
        }
        _isSoundOn = !_isSoundOn;
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
