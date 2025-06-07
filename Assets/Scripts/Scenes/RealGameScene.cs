using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class RealGameScene : BaseScene
{
    [SerializeField] private GameObject _vinterRealWorld;
    [SerializeField] private GameObject _gangRealWorld;
    [SerializeField] private GameObject _chaumRealWorld;

    public void Start()
    {
        switch (Managers.World.CurrentWorldType)
        {
            case WorldType.Gang:
                _gangRealWorld.SetActive(true);
                break;
            case WorldType.Vinter:
                _vinterRealWorld.SetActive(true);
                break;
            case WorldType.Chaumm:
                _chaumRealWorld.SetActive(true);
                break;
            default:
                Debug.Log("나올 수 없는 월드 입니다");
                break;
        }
    }


    protected override void Init()
    {
        SceneType = Scene.RealGameScene;
    }

    public override void Clear()
    {
    }
}
