using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
	public Scene SceneType { get; protected set; } = Scene.Unknown;
	
	private void Awake()
	{
		Init();
	}

	/// <summary>
	/// Awake�������� �ʱ�ȭ ����
	/// </summary>
	protected abstract void Init();

	/// <summary>
	/// Scene�� ��ȯ�� �Ͼ� �� �� �ڵ����� ȣ��
	/// </summary>
	public abstract void Clear();
}
