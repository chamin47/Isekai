using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealGameDiary : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 일기장 UI를 활성화하는 코드
            Time.timeScale = 0f;
        }
        else if(collision.CompareTag("MainCamera"))
        {
            // 적이 일기장에 접근했을 때의 처리
            Debug.Log("적이 일기장에 접근했습니다.");
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = 1f;
            
        }
    }
}
