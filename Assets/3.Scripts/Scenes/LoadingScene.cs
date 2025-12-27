using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이세계 -> UI_GameLoading -> 현실세계
/// 도서관 -> UI_Loading -> 이세계
/// </summary>
public class LoadingScene : BaseScene
{
    private void Start()
    {
        ShowLodingUI();
    }

    protected override void Init()
    {
        SceneType = Scene.LoadingScene;
    }

    private void ShowLodingUI()
    {
        if(Managers.Scene.prevSceneType == Scene.LibraryScene)
        {
            Managers.UI.ShowSceneUI<UI_Loading>();  
        }
        else
        {
            Managers.UI.ShowSceneUI<UI_GameLoading>();
        }
    }

    public override void Clear()
    {
        
    }
}
