using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class GameSceneEx : BaseScene
{
	// �̸� �̿��ϴ� ���� ���Ŀ� ObjectManager�� ����ϸ� player�� �����ϰ� ����� ���� ���ڴ�.
	public static Transform player;

	[SerializeField] private MiniGameFactory _miniGameFactory;
	[SerializeField] private Transform _player;
	
	[SerializeField] private Image _fadeImage;

    [SerializeField] private WorldType _worldType = WorldType.Gang;

	//[SerializeField] private List<GameObject> _portalList;
	[SerializeField] private float _gameStartDelay = 1.0f;
    protected override void Init()
	{
		base.Init();

        _worldType = Managers.World.CurrentWorldType;
        GameSceneEx.player = _player;

        Debug.Log($"CurrentWorld is : {_worldType}");
		Debug.Log("GameSceneEx Init");

		SceneType = Scene.GameScene;

		//�̴ϰ��� ���� �ʱ�ȭ
		_miniGameFactory.Init(_worldType);
        _miniGameFactory.OnGameEnd += GameOver;

		GameObject background = Managers.Resource.Instantiate($"Background/{_worldType.ToString()}World");
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

			go.GetComponent<Portal>().onYesEvent += ClearEvent;
        }
		else
		{
            // �� ����� ��Ӱ� ó���ǰ�, ���ӿ���â�� ��������
            Managers.UI.ShowPopupUI<UI_GameOver>();
        }
	}

	private void ClearEvent()
	{
        StartCoroutine(CoFadeOut());
    }

    private IEnumerator CoFadeOut()
    {
        float fadeOutTime = 3.0f;
        float currentTime = 0.0f;
        Color color = _fadeImage.color;

        while (currentTime < fadeOutTime)
        {
            currentTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, currentTime / fadeOutTime);
            _fadeImage.color = color;
            yield return null;
        }

        Managers.Scene.LoadScene(Scene.RealGameScene);

        yield return null;
    }


    public override void Clear()
	{

	}
}
