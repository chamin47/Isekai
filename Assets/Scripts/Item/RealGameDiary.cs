using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealGameDiary : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // �ϱ��� UI�� Ȱ��ȭ�ϴ� �ڵ�
            Time.timeScale = 0f;
        }
        else if(collision.CompareTag("MainCamera"))
        {
            // ���� �ϱ��忡 �������� ���� ó��
            Debug.Log("���� �ϱ��忡 �����߽��ϴ�.");
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
