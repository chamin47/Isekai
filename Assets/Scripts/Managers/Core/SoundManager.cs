using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager
{
    AudioSource _bgmAudio;
    AudioSource[] _effectAudio;
    AudioSource _subEffectAudio;

    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();

	private GameObject _root = null;

    private const int MAX_EFFECT_COUNT = 10;
    public void Init()
	{
		if(_root != null)
		{
			return;
		}

		_root = GameObject.Find("@SoundRoot");
		if (_root == null)
		{
            _root = new GameObject { name = "@SoundRoot" };
			UnityEngine.Object.DontDestroyOnLoad(_root);

            _bgmAudio = new GameObject { name = "BgmAudio" }.AddComponent<AudioSource>();
            _bgmAudio.transform.parent = _root.transform;
            _bgmAudio.playOnAwake = false;
            _bgmAudio.loop = true;

            _subEffectAudio = new GameObject { name = "SubEffectAudio" }.AddComponent<AudioSource>();
            _subEffectAudio.transform.parent = _root.transform;
            _subEffectAudio.playOnAwake = false;
            _subEffectAudio.loop = true;

            _effectAudio = new AudioSource[MAX_EFFECT_COUNT];

            for (int i = 0; i < MAX_EFFECT_COUNT; i++)
            {
                _effectAudio[i] = new GameObject { name = $"EffectAudio_{i}" }.AddComponent<AudioSource>();
                _effectAudio[i].transform.parent = _root.transform;
                _effectAudio[i].playOnAwake = false;
            }
        }
	}

	public void Clear()
	{
        _bgmAudio.Stop();
        _bgmAudio.clip = null;

        for (int i = 0; i < MAX_EFFECT_COUNT; i++)
        {
            _effectAudio[i].Stop();
            _effectAudio[i].clip = null;
        }

        _subEffectAudio.Stop();
        _subEffectAudio.clip = null;

        _audioClips.Clear();
	}

    public void Play(string key, Sound type, float pitch = 1.0f)
    {
        AudioSource audioSource = null;

        if (type == Sound.Bgm)
        {
            audioSource = _bgmAudio;
            LoadAudioClip(key, (audioClip) =>
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();

                audioSource.clip = audioClip;
                audioSource.Play();
            });
        }
        else if (type == Sound.Effect)
        {
            for (int i = 0; i < MAX_EFFECT_COUNT; i++)
            {
                if (!_effectAudio[i].isPlaying)
                {
                    audioSource = _effectAudio[i];
                    break;
                }
            }

            if (audioSource == null)
            {
                Debug.Log("모든 효과음 소스가 사용중입니다.");
                return;
            }

            LoadAudioClip(key, (audioClip) =>
            {
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(audioClip);
            });
        }
        else if (type == Sound.SubEffect)
        {
            audioSource = _subEffectAudio;
            LoadAudioClip(key, (audioClip) =>
            {
                audioSource.clip = audioClip;
                audioSource.Play();
            });
        }
    }

    //현재 pause사용은 하나의 음악만 사용하므로 문제 발생 여부가 없지만
    //추후 한번에 두개 이상이 사용될시 문제 발생 및 해결 필요
    public void UnPauseSubEffect()
    {
        if (_subEffectAudio.clip != null)
        {
            _subEffectAudio.UnPause();
        }
    }

    public void PauseSubEffect()
    {
        if (_subEffectAudio.clip != null)
        {
            _subEffectAudio.Pause();
        }
    }

    public void StopSubEffect()
    {
        _subEffectAudio.Stop();
        _subEffectAudio.clip = null;
    }

    private void LoadAudioClip(string key, Action<AudioClip> callback)
    {
        AudioClip audioClip = null;
        if (_audioClips.TryGetValue(key, out audioClip))
        {
            callback?.Invoke(audioClip);
            return;
        }

        audioClip = Managers.Resource.Load<AudioClip>($"Sounds/{key}");

        if (!_audioClips.ContainsKey(key))
            _audioClips.Add(key, audioClip);

        callback?.Invoke(audioClip);
    }

   
}
