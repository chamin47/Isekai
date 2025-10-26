using System;
using System.Collections;
using UnityEngine;

public class SimpleInputPrompt : MonoBehaviour, IInputPrompt
{
    [Header("Anchors")]
    public Transform playerAnchor;     // 플레이어 머리 위 빈 오브젝트 등

    public IEnumerator Prompt(string prompt, Action<string> onDone)
    {
        // 월드 스페이스 입력 말풍선 생성
        var ui = Managers.UI.MakeWorldSpaceUI<UI_InputBalloon>();
        ui.Init(playerAnchor);

        string captured = "";
        yield return ui.CoPrompt(prompt, s => captured = s);

        onDone?.Invoke(captured);
    }
}
