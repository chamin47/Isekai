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
    public List<int> requiredKeyCount;
    public List<bool> canPressConcurrent;
    public string dialog;
}

public abstract class WorldInfo
{
    public WorldType worldType;

    public List<int> difficulty = new List<int>();
    public List<int> startGauge = new List<int>();
    public List<int> perDecreaseGauge = new List<int>();
    public List<int> perIncreaseGauge = new List<int>();
    public List<int> succedGauge = new List<int>();
    public List<int> failGauge = new List<int>();
    public List<int> runGauge = new List<int>();
    public List<int> limitTime = new List<int>();

    public readonly List<string> dialog = new List<string>();

    public WorldInfo(WorldType worldType)
    {
        this.worldType = worldType;
    }

    public virtual MiniGameInfo GetRandomMiniGameInfo()
    {
        // ���� �ؽ�Ʈ ����
        int index = UnityEngine.Random.Range(0, dialog.Count);

        // ���� ���̵� ���� ����
        int difficultyIndex = UnityEngine.Random.Range(0, dialog.Count);

        if (difficultyIndex < difficulty[0])
        {
            difficultyIndex = 0;
        }
        else if (difficultyIndex < difficulty[0] + difficulty[1])
        {
            difficultyIndex = 1;
        }
        else
        {
            difficultyIndex = 2;
        }

        MiniGameInfo miniGameInfo = new MiniGameInfo()
        {
            difficulty = (MiniGameDifficulty)difficultyIndex,
            startGauge = startGauge[difficultyIndex],
            perDecreaseGauge = perDecreaseGauge[difficultyIndex],
            perIncreaseGauge = perIncreaseGauge[difficultyIndex],
            succedGauge = succedGauge[difficultyIndex],
            failGauge = failGauge[difficultyIndex],
            runGauge = runGauge[difficultyIndex],
            limitTime = limitTime[difficultyIndex],
            dialog = dialog[index]
        };

        return miniGameInfo;
    }
}

public class VinterWorldInfo : WorldInfo
{
    public VinterWorldInfo() : base(WorldType.Vinter)
    {
        difficulty.AddRange(new List<int>() { 6, 0, 0 });
        startGauge.AddRange(new List<int>() { 50, 50, 50 });
        perDecreaseGauge.AddRange(new List<int>() { -10, -10, -10 });
        perIncreaseGauge.AddRange(new List<int>() { 3, 3, 3 });
        succedGauge.AddRange(new List<int>() { 40, 40, 40 });
        failGauge.AddRange(new List<int>() { -10, -10, -10 });
        runGauge.AddRange(new List<int>() { -20, -20, -20 });
        limitTime.AddRange(new List<int>() { 4, 4, 4 });

        dialog.AddRange(new List<string>
        {
            "���� �� ������ ǰ���� ���� �Ϻ��� �ι��̼�!",
            "���۴��� ����� ���� ������ �����̾�.",
            "���۴��� �ܸ�� ���� ������ ���� ��ǰ ����.",
            "���۴��� Ī���ϴ� �͸����ε� �����̾�!",
            "���� ���� ��ü�� �� ������ ū �ູ����.",
            "��� ���ְ� ���۴��� �ܸ� ���� ������ �기����?"
        });
    }
}

public class ChaummWorldInfo : WorldInfo
{
    public readonly List<KeyCode> requireKeys = new List<KeyCode>()
    { KeyCode.Q, KeyCode.W, KeyCode.E, KeyCode.A, KeyCode.S, KeyCode.D };
    public List<int> requiredKeyCount = new List<int>() { 0, 3, 0 };
    public List<bool> canPressConcurrent = new List<bool>() { false, false, false };
    public ChaummWorldInfo() : base(WorldType.Chaumm)
    {
        difficulty.AddRange(new List<int>() { 3, 4, 0 });
        startGauge.AddRange(new List<int>() { 50, 40, 50 });
        perDecreaseGauge.AddRange(new List<int>() { -10, -15, -10 });
        perIncreaseGauge.AddRange(new List<int>() { 5, 5, 3 });
        succedGauge.AddRange(new List<int>() { 30, 30, 40 });
        failGauge.AddRange(new List<int>() { -10, -10, -10 });
        runGauge.AddRange(new List<int>() { -20, -20, -20 });
        limitTime.AddRange(new List<int>() { 4, 4, 4 });

        dialog.AddRange(new List<string>
        {
            "�� ���� �ʸ� ���� ���� ���°ž�!!",
            "�� ������. �߻���.",
            "������ �Ƹ��ٿ��̶� �󸶳� ������ ���ΰ�...",
            "�ʴ� ���� �Ƚᵵ �ǰڴ�.. ������ �������ϱ� ><",
            "������ ���� �ı��� ����!!",
            "���� ���� ������ ����",
            "�ܽ� ��ü�� �� ���̾�"
        });
    }
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
            miniGame.Init(miniGameInfo, spawnInfo);
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

        _isGameEnd = true;

        if (happiness <= 0 || happiness >= 100)
        {
            Debug.Log("GameEnd");
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
        }
    }

    private void OnDestroy()
    {
        Managers.Happy.OnHappinessChanged -= CreateControll;
    }
}
