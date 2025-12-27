using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 대화 실행에 필요한 모든 서비스와 상태를 포함하는 컨텍스트
/// </summary>
public class DialogueContext
{
    // Player
    public PlayerController Player { get; set; }

    // Services
    public DialogueTextPresenter TextPresenter { get; set; }
    public CameraService CameraService { get; set; }
    public ActorDirectorSimple ActorDirector { get; set; }
    public SimpleChoiceUI ChoiceUI { get; set; }
    public SimpleInputPrompt InputPrompt { get; set; }
    public HappinessHUD Happiness { get; set; }
    public UI_LetterBox LetterBox { get; set; }
    public CameraController CameraController { get; set; }

    // Tables
    public BranchTable BranchTable { get; set; }
    public ChoiceTable ChoiceTable { get; set; }
    public ClickTargetTable ClickTargetTable { get; set; }

    // State
    public string NextID { get; set; }
    public int EndEventParam { get; set; }
    public Action<int> OnDialogueEnd { get; set; }

    // Helper
    public void Fire(MonoBehaviour mono, IEnumerator coroutine)
    {
        if (coroutine != null && mono != null)
            mono.StartCoroutine(coroutine);
    }
}