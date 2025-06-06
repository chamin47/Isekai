using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �̼��� -> ���Ǽ��� �̵�
/// </summary>
public class UI_TodoList : MonoBehaviour
{
    private LoadingGameSceneData _data;

    [SerializeField] private Transform _toggleGroup;
    [SerializeField] private TMP_Text _diaryText;

    [SerializeField] private List<Sub_Toggle> _toggles = new List<Sub_Toggle>();
    int index = 0;
    public void Init(LoadingGameSceneData data)
    {
        _data = data;
        // ���࿡ ��ȭ�� ������ �޶����ٸ� �������� ���� + ���� ������ �������� �˸°� ��ġ�Ѵ�
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

    private IEnumerator OnToggleOn()
    {
        yield return WaitForSecondsCache.Get(1f);

        if (index == _toggles.Count)
        {
            _diaryText.gameObject.SetActive(true);
            yield return WaitForSecondsCache.Get(1f);
            Managers.Sound.Play("writing", Sound.SubEffect);
            yield return StartCoroutine(_diaryText.CoTypeEffectWithRichText(_data.diary, 0.07f));
            Managers.Sound.StopSubEffect();

            yield return WaitForSecondsCache.Get(1f);
            Managers.Scene.LoadScene(Scene.RealGameScene);
        }
        else
        {
            _toggles[index].gameObject.SetActive(true);
            Managers.Sound.Play("keyboard_long", Sound.SubEffect);
            yield return StartCoroutine(_toggles[index].Text.CoTypingEffect(_data.todoList[index], 0.07f));
            Managers.Sound.StopSubEffect();
            _toggles[index].BackGround.SetActive(true);
        }
        index++;
    }

}
