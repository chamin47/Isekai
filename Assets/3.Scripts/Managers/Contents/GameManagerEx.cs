using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 게임 진행상에 필요한 데이터를 관리하는 클래스
/// </summary>
public class GameManagerEx
{
    // 엔딩씬을 봤는지 보지 않았는지 체크

    private bool _isShowEnding = false;
    public bool IsShowEnding
    {
        get { return _isShowEnding; }
        set { _isShowEnding = value; }
    }
    //public bool IsShowEnding
    //{
    //    get { return PlayerPrefs.GetInt("IsShowEnding", 0) == 1; }
    //    set { PlayerPrefs.SetInt("IsShowEnding", value ? 1 : 0); }
    //}

    // 현재 미니게임이 대화모드인지 아닌지 체크
    public bool DialogueActive { get; set; } = false;


    /// <summary>
    /// intro에 필요한 저장 데이터
    /// </summary>

    public bool IsIntroCommentSaved = false;
    public string IntroCommentText;

    public void Init()
    {

    }
}
