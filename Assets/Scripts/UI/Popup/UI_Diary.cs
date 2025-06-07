using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Diary : MonoBehaviour
{
    public int ID = 1; // ���� �����ý����� ����� ���� �ʿ��� ����

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
                gameObject.SetActive(false); // �ϱ��� UI ��Ȱ��ȭ
           }
        }
    }

    private void OnDisable()
    {
        Time.timeScale = 1f; // ���� �ӵ� �������
        _timer = 0; // Ÿ�̸� �ʱ�ȭ
        Managers.Sound.UnPauseBGM();
    }
}
