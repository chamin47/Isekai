using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 간단 입력 프롬프트.
/// 플레이어 앵커 위치에 입력 말풍선을 띄워 텍스트 입력을 받고 콜백으로 전달한다.
/// </summary>
public class SimpleInputPrompt : MonoBehaviour, IInputPrompt
{
    [Header("Anchors")]
    [SerializeField] private Transform _playerAnchor;     // 플레이어 머리 위 빈 오브젝트 등

    [SerializeField] private DialogueTextPresenter _textPresenter;

    public IEnumerator Prompt(string prompt, Action<string> onDone)
    {
        _textPresenter.AllStacksUp();

        // 월드 스페이스 입력 말풍선 생성
        var ui = Managers.UI.MakeWorldSpaceUI<UI_InputBalloon>();
        ui.Init(_playerAnchor);

        string captured = "";
        yield return ui.CoPrompt(prompt, s => captured = s);

        onDone?.Invoke(captured);
    }
}
