using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneEx : BaseScene
{
	[SerializeField] private MiniGameFactory _miniGameFactory;
	[SerializeField] private Transform _player;
	[SerializeField] private WorldType _worldType = WorldType.Chaumm;
    protected override void Init()
	{
		base.Init();

		SceneType = Scene.GameScene;

		//�̴ϰ��� ���� �ʱ�ȭ
		_miniGameFactory.Init(_worldType);
        _miniGameFactory.OnGameEnd += GameOver;

        //������� ���
        Managers.Sound.Play("anotherWorldBgm", Sound.Bgm);
	}

	//���� �����
	public void GameOver(bool isWin)
	{
		if (isWin)
		{
			// ���Ǽ���� �̵��ϴ� ��Ż�� �����ȴ�
			GameObject go = Managers.Resource.Instantiate("Item/Portal");
			Vector3 newPosition = _player.position + new Vector3(8f, 0, 0);
            newPosition.y = -2.5f;
            go.transform.position = newPosition;
        }
		else
		{
            // �� ����� ��Ӱ� ó���ǰ�, ���ӿ���â�� ��������
            Managers.UI.ShowPopupUI<UI_GameOver>();
        }
	}

	public override void Clear()
	{

	}
}
