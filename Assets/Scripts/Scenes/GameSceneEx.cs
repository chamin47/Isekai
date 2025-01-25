using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneEx : BaseScene
{
	[SerializeField] private MiniGameFactory _miniGameFactory;
	protected override void Init()
	{
		base.Init();

		SceneType = Scene.GameScene;

		//�̴ϰ��� ���� �ʱ�ȭ
		_miniGameFactory.Init(WorldType.Vinter);

		//������� ���
        Managers.Sound.Play("anotherWorldBgm", Sound.Bgm);
	}

	//���� �����
	public void GameOver(bool isWin)
	{
		if (isWin)
		{
			// ���Ǽ���� �̵��ϴ� ��Ż�� �����ȴ�
		}
		else
		{
			// �� ����� ��Ӱ� ó���ǰ�, ���ӿ���â�� ��������
		}
	}

	public override void Clear()
	{

	}
}
