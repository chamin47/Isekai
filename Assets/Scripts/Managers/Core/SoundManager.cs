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
        //여기 오류 끝기지 않음
        else if (type == Sound.SubEffect)
        {
            audioSource = _subEffectAudio;

            if (isPaused)
            {
                ReStartSubEffect();
            }

            LoadAudioClip(key, (audioClip) =>
            {
                if (audioSource.isPlaying)
                    audioSource.Stop();
                audioSource.clip = audioClip;
                audioSource.loop = true;
                audioSource.Play();
            });
        }
    }

    private bool isPaused = false;

    public void ReStartSubEffect()
    {
        Debug.Log("SubEffectAudio ReStart");
        _subEffectAudio.UnPause();
        isPaused = false;
    }

    public void PauseSubEffect()
    {
        Debug.Log("SubEffectAudio Pause");
        _subEffectAudio.Stop();
        isPaused = true;
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
