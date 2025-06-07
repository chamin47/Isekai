using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Diary : MonoBehaviour
{
    public int ID = 1; // 추후 업적시스템을 만들기 위해 필요할 예정

    private float _timer = 0;

    private void OnEnable()
    {
        Time.timeScale = 0f;
        _timer = 0;
        Managers.Sound.PauseBGM();
    }

    public void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if(Input.GetMouseButtonDown(0))
        {
           if(_timer > 3f)
           {
                gameObject.SetActive(false); // 일기장 UI 비활성화
           }
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f; // 게임 속도 원래대로
        _timer = 0; // 타이머 초기화
        Managers.Sound.UnPauseBGM();
    }
}
