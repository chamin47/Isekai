using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 이세계 -> 현실세계 이동
/// </summary>
public class UI_TodoList : MonoBehaviour
{
    private LoadingGameSceneData _data;

    [SerializeField] private Transform _toggleGroup;
    [SerializeField] private TMP_Text _diaryText;

    [SerializeField] private List<Sub_Toggle> _toggles = new List<Sub_Toggle>();
    [SerializeField] private Image _fadeImage;

    int index = 0;

	private bool _isTyping;
	private bool _skipRequested;

	public void Init(LoadingGameSceneData data)
    {
        _data = data;

        // 만약에 대화의 개수가 달라진다면 동적으로 생성 + 허용된 범위를 기준으로 알맞게 배치한다
        for(int i = 0; i < _data.todoList.Count; i++)
        {
            Sub_Toggle go = _toggles[i];

            go.Init(_data.todoList[i], i);
            go.Toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    go.Toggle.interactable = false;
                    StartCoroutine(OnToggleOn());
                }
            });
            go.gameObject.SetActive(false);
        }

        StartCoroutine(OnToggleOn());
    }

	private void Update()
	{
		if (!_isTyping)
			return;

		if (Input.GetMouseButtonDown(0))
		{
			_skipRequested = true;
		}
	}

	private IEnumerator OnToggleOn()
    {
        yield return WaitForSecondsCache.Get(1f);

        if (index == _toggles.Count)
        {
            // 일기장 text 
            _diaryText.gameObject.SetActive(true);
            yield return WaitForSecondsCache.Get(1f);

			_isTyping = true;
			_skipRequested = false;

			yield return StartCoroutine(
				CoTypingWithSkip(
					_diaryText,
					"writing",
					_data.diary,
					0.07f,
					useRichText: true
				)
			);

			_isTyping = false;

            yield return WaitForSecondsCache.Get(1f);

            Managers.Sound.Play("s2_book1", Sound.Effect);
            yield return StartCoroutine(_fadeImage.CoFadeOut(2f));
            Managers.Scene.LoadScene(Scene.RealGameScene);
        }
        else
        {
            // 메모 text
            _toggles[index].gameObject.SetActive(true);

			_isTyping = true;
			_skipRequested = false;

			yield return StartCoroutine(
				CoTypingWithSkip(
					_toggles[index].Text,
					"keyboard_long",
					_data.todoList[index],
					0.07f,
					useRichText: true
				)
			);

			_isTyping = false;

            _toggles[index].BackGround.SetActive(true);
        }

        index++;
    }

	/// <summary>
	/// 마우스 클릭으로 스킵 가능한 타이핑 코루틴
	/// </summary>
	private IEnumerator CoTypingWithSkip(TMP_Text text, string sound, string content, float speed, bool useRichText = false)
	{
		text.text = "";

		Managers.Sound.Play(sound, Sound.SubEffect);

		if (useRichText)
		{
			int i = 0;
			while (i < content.Length)
			{
				if (_skipRequested)
				{
					text.text = content;
					break;
				}

				char c = content[i];

				if (c == '<')
				{
					int closeIndex = content.IndexOf('>', i);
					if (closeIndex != -1)
					{
						text.text += content.Substring(i, closeIndex - i + 1);
						i = closeIndex + 1;
						continue;
					}
				}

				text.text += c;
				i++;

				yield return WaitForSecondsCache.Get(speed);
			}
		}
		else
		{
			for (int i = 0; i < content.Length; i++)
			{
				if (_skipRequested)
				{
					text.text = content;
					break;
				}

				text.text += content[i];
				yield return WaitForSecondsCache.Get(speed);
			}
		}

		Managers.Sound.StopSubEffect();
	}
}
