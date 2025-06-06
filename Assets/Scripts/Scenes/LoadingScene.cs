using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ -> �̼��� GameLoadingUI
/// ���Ǽ��� -> ������ UI_Loading
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
        if(Managers.Scene.prevSceneType != Scene.GameScene)
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
