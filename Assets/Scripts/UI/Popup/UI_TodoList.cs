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
                    StartCoroutine(OnToggleOn());
                }
            });
            go.gameObject.SetActive(false);
        }
        _toggles[0].gameObject.SetActive(true);
    }

    private IEnumerator OnToggleOn()
    {
        yield return StartCoroutine(_toggles[index].Text.CoTypingEffect(_data.todoList[index], 0.1f));

        if(index == _toggles.Count - 1)
        {
            _diaryText.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            yield return StartCoroutine(_diaryText.CoTypeEffectWithRichText(_data.diary, 0.05f));

            yield return new WaitForSeconds(1f);
            Managers.Scene.LoadScene(Scene.RealGameScene);
        }
        else
        {
            _toggles[index + 1].gameObject.SetActive(true);
        }
        index++;
    }
}
