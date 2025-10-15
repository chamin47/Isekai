using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ����� �ʿ��� �����͸� �����ϴ� Ŭ����
/// </summary>
public class GameManagerEx
{
    // �������� �ô��� ���� �ʾҴ��� üũ
    public bool IsShowEnding
    {
        get { return PlayerPrefs.GetInt("IsShowEnding", 0) == 1; }
        set { PlayerPrefs.SetInt("IsShowEnding", value ? 1 : 0); }
    }

	// ���� �̴ϰ����� ��ȭ������� �ƴ��� üũ
	public bool DialogueActive { get; set; } = false;

    public void Init()
    {

    }
}
