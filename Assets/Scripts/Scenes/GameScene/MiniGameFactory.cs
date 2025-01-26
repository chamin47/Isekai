using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Data
public struct MiniGameInfo
{
    public MiniGameDifficulty difficulty;
    public int startGauge;
    public int perDecreaseGauge;
    public int perIncreaseGauge;
    public int succedGauge;
    public int failGauge;
    public int runGauge;
    public int limitTime;

    public List<KeyCode> requiredKeys;
    public int requiredKeyCount;
    public bool canPressConcurrent;
    public string dialog;
}

#endregion

public struct SpawnInfo
{
    public Vector2 position;
    public bool isLeft;
}

public class MiniGameFactory : MonoBehaviour
{
    [SerializeField] private UI_MiniGame _miniGame;
    [SerializeField] private WorldInfo _worldInfo;

    [SerializeField] private KeySpriteFactory _keySpriteFactory;

    [SerializeField] private Transform target;
    [SerializeField] private float _minX = -10f;
    [SerializeField] private float _maxX = 10f;
    [SerializeField] private float _pivotY = 4f;

    private bool _isGameEnd = false;

    private Queue<UI_MiniGame> _miniGameQueue = new Queue<UI_MiniGame>();

    public event Action<bool> OnGameEnd;

    public void Init(WorldType worldType)
    {
        SetWorld(worldType);
    
        Managers.Happy.OnHappinessChanged += CreateControll;

        _keySpriteFactory = new KeySpriteFactory();
        _keySpriteFactory.Init();

        StartCoroutine(CreateMiniGame());
    }

    public IEnumerator CreateMiniGame()
    {
        while (true)
        {
            Debug.Log("CreateMiniGame");
            SpawnInfo spawnInfo = GetRandomPosition();
            UI_MiniGame miniGame = Instantiate(_miniGame, spawnInfo.position, Quaternion.identity);
            MiniGameInfo miniGameInfo = _worldInfo.GetRandomMiniGameInfo();
            _miniGameQueue.Enqueue(miniGame);
            miniGame.Init(miniGameInfo, spawnInfo, _keySpriteFactory);
            yield return new WaitForSeconds(4f);
        }
    }

    // target�� �������� ���� �Ÿ���ŭ ������ ���� ����
    private SpawnInfo GetRandomPosition()
    {
        float randomX = UnityEngine.Random.Range(_minX, _maxX);
        bool isLeft = randomX < 0;

        Vector2 spawnPos = target.transform.position;

        if(randomX < 0)
        {
            spawnPos += new Vector2(_minX,  _pivotY);
        }
        else
        {
            spawnPos += new Vector2(_maxX,  _pivotY);
        }

        return new SpawnInfo() { position = spawnPos, isLeft = isLeft };
    }

    // �ູ���� ���� �������Ῡ�� �Ǵ�
    private void CreateControll(float happiness)
    {
        if(_isGameEnd) return;


        if (happiness <= 0 || happiness >= 100)
        {
            _isGameEnd = true;
            StopAllCoroutines();
            foreach (var miniGame in _miniGameQueue)
            {
                miniGame.gameObject.SetActive(false);
            }

            OnGameEnd?.Invoke(happiness >= 100);
        }
    }

    // ���� ���� ����
    private void SetWorld(WorldType worldType)
    {
        switch (worldType)
        {
            case WorldType.Vinter:
                _worldInfo = new VinterWorldInfo();
                break;
            case WorldType.Chaumm:
                _worldInfo = new ChaummWorldInfo();
                break;
            case WorldType.Gang:
                _worldInfo = new GangWorldInfo();
                break;
        }
    }

    private void OnDestroy()
    {
        Managers.Happy.OnHappinessChanged -= CreateControll;
    }
}
