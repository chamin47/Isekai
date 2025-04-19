using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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

    public List<KeyCode> requiredKeys; // �ʿ��� Ű ����Ʈ
    public int requiredKeyCount; // �ʿ��� Ű ����
    public bool canPressConcurrent; // ���ÿ� ���� �� �ִ��� ����
    public string dialog;
}
public struct SpawnInfo
{
    public Vector2 position;
    public bool isLeft;
}
#endregion


/// <summary>
/// 1. �̴ϰ��� ����
/// 2. ���� �ʱ�ȭ
/// 
/// </summary>

public class MiniGameFactory : MonoBehaviour
{
    [SerializeField] private UI_MiniGame _miniGame;

    [SerializeField] private GridSystem _gridSystem;
    [SerializeField] private KeySpriteFactory _keySpriteFactory;

    [Header("�÷��̾� ����")]
    [SerializeField] private Transform _target;

    // �����丵 ������
    [SerializeField] private GameObject _leftExclamation;
    [SerializeField] private GameObject _rightExclamation;

    [Header("�̴ϰ��� ���� ����")]
    [SerializeField] private float _minBubbleYPos = 0f;
    [SerializeField] private float _maxBubbleYPos = 2f;
    [SerializeField] private float _spawnDelay = 4f;

    [SerializeField] private float _waitBeforeGameStartTime = 1f;       // �̴ϰ��� ������ ���ð�

    private Queue<UI_MiniGame> _miniGameQueue = new Queue<UI_MiniGame>();

    private WorldInfo _worldInfo;

    public event Action<bool> OnGameEnd;

    private bool _isGameEnd = false;
    private int successCount = 0;


    public void Init()
    {
        // ���� ������ ��������
        _worldInfo = Managers.World.GetWorldInfo();

        Managers.Happy.OnHappinessChanged += CheckMiniGameEnd;

        // Ű ��������Ʈ ��Ī �۾�
        _keySpriteFactory = new KeySpriteFactory();
        _keySpriteFactory.Init();

        // ��ǳ�� ��ġ Ž�� �۾�
        _gridSystem.Init(_target);

        StartCoroutine(CreateMiniGame());
    }
    public bool IsBubbleEmpty
    {
        get
        {
            float height = Camera.main.orthographicSize * 2f;
            float width = height * Camera.main.aspect;

            Collider2D col = Physics2D.OverlapBox(Camera.main.transform.position, new Vector2(width, height), 0, LayerMask.GetMask("UI"));
            return col == null;
        }
    }

    private void Update()
    {
        bool isLeft = false;
        bool isRight = false;

        if (IsBubbleEmpty)
        {
            // Ȱ��ȭ�� ���� ������Ʈ�� ���ؼ��� ó��
            foreach (UI_MiniGame _miniGame in _miniGameQueue)
            {
                if (_miniGame.gameObject.activeSelf)
                {
                    isLeft |= _miniGame.transform.position.x < _target.position.x;
                    isRight |= _miniGame.transform.position.x > _target.position.x;
                }
            }

            _leftExclamation.SetActive(isLeft);
            _rightExclamation.SetActive(isRight);
        }
        else
        {
            _leftExclamation.SetActive(false);
            _rightExclamation.SetActive(false);
        }
    }


    public IEnumerator CreateMiniGame()
    {
        yield return WaitForSecondsCache.Get(_waitBeforeGameStartTime);

        while (true)
        {
            
            if(TryGetRandomPosition(out Vector2 randomPos)){
                bool isLeftSide = randomPos.x < _target.position.x;

                SpawnInfo spawnInfo = new SpawnInfo
                {
                    position = randomPos,
                    isLeft = isLeftSide
                };

                MiniGameInfo miniGameInfo = _worldInfo.GetRandomMiniGameInfo();

                UI_MiniGame miniGame = Instantiate(_miniGame, spawnInfo.position, Quaternion.identity);
                _miniGameQueue.Enqueue(miniGame);

                miniGame.Init(miniGameInfo, spawnInfo, _keySpriteFactory);

                Managers.Sound.Play("i_mini_say1", Sound.Effect);

                // �� �κ��� ���� �Ž�����
                // ���谡 �ٲ��?
                // �縶���� ���迡���� ����Ʈ ���μ��̰� 7���� �̴ϰ����� �������� �� ���� ����
                if (Managers.World.CurrentWorldType == WorldType.Pelmanus)
                {
                    miniGame.onMiniGameSucced += () =>
                    {
                        successCount++;

                        GameSceneEx scene = Managers.Scene.CurrentScene as GameSceneEx;
                        scene.SetPostProcessing(successCount);

                        if(successCount == 5)
                        {
                            GameEnd(true);
                        }
                    };
                }
            }

            yield return WaitForSecondsCache.Get(_spawnDelay);
        }
    }

    private bool TryGetRandomPosition(out Vector2 randomPos)
    {
        float randomY = UnityEngine.Random.Range(_minBubbleYPos, _maxBubbleYPos);

        if(_gridSystem.TryGetEmptyPosition(out Vector2 spawnPos) == false)
        {
            Debug.Log("��� ������ á���ϴ�.");
            randomPos = Vector2.zero;
            return false;
        }

        randomPos = spawnPos + new Vector2(0, randomY);
        return true;
    }

    // �ູ���� ���� �������Ῡ�� �Ǵ�
    private void CheckMiniGameEnd(float happiness)
    {
        if(_isGameEnd) return;

        if (happiness <= 0 || happiness >= 100)
        {
            GameEnd(happiness >= 100);
        }
    }

    private void GameEnd(bool isSuccess)
    {
        _isGameEnd = true;
        StopAllCoroutines();

        foreach (var miniGame in _miniGameQueue)
        {
            miniGame.gameObject.SetActive(false);
        }

        OnGameEnd?.Invoke(isSuccess);
    }

    private void OnDestroy()
    {
        Managers.Happy.OnHappinessChanged -= CheckMiniGameEnd;
    }
}
