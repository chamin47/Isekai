using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MiniGameSpawner : MonoBehaviour
{
    [Header("Objects")]
    [SerializeField] private UI_MiniGame _miniGamePrefab;
    [SerializeField] private GridSystem _gridSystem;
    [SerializeField] private Transform _target; // 플레이어의 위치

    [Header("Spawning Settings")]
    [SerializeField] private float _minBubbleYPos = 0f;
    [SerializeField] private float _maxBubbleYPos = 2f;
    [SerializeField] private float _spawnDelay = 4f;
    [SerializeField] private float _waitBeforeGameStartTime = 1f;

    public event Action<UI_MiniGame> OnMiniGameSpawned;

    private Coroutine _spawnCoroutine;
    private WorldInfo _worldInfo;
    private KeySpriteFactory _keySpriteFactory;
    private bool _isFirstMiniGame = true;

    // 대사 관련
    private int _dialogIndexIter = 0;
    private List<int> _randomDialogueIndex = new List<int>();

    public void Init()
    {
        _worldInfo = Managers.World.GetWorldInfo();

        _keySpriteFactory = new KeySpriteFactory();
        _keySpriteFactory.Init();
        _gridSystem.Init(_target);

        int dialogueCount = _worldInfo.dialog.Count;

        _randomDialogueIndex = Enumerable.Range(0, dialogueCount).ToList();
        ShuffleDialogueIndex();
    }

    public void BeginSpawning()
    {
        if (_spawnCoroutine != null) StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = StartCoroutine(CreateMiniGameRoutine());
    }

    public void StopSpawning()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private void ShuffleDialogueIndex()
    {
        _randomDialogueIndex.Shuffle();
    }

    private IEnumerator CreateMiniGameRoutine()
    {
        yield return WaitForSecondsCache.Get(_waitBeforeGameStartTime);

        while (true)
        {
            Vector2 spawnPos;
            if (!_isFirstMiniGame && TryGetRandomPosition(out spawnPos))
            {
                SpawnMiniGame(spawnPos);
            }
            else
            {
                _isFirstMiniGame = false;
                spawnPos = new Vector2(_target.position.x, _target.position.y + 5.5f);
                SpawnMiniGame(spawnPos);
            }

            yield return WaitForSecondsCache.Get(_spawnDelay);
        }
    }

    private void SpawnMiniGame(Vector2 spawnPosition)
    {
        UI_MiniGame miniGameInstance = Instantiate(_miniGamePrefab, spawnPosition, Quaternion.identity);

        MiniGameInfo miniGameInfo = _worldInfo.GetRandomMiniGameInfo(GetNextDialogueIndex());
        SpawnInfo spawnInfo = new SpawnInfo { position = spawnPosition, isLeft = spawnPosition.x < _target.position.x };

        miniGameInstance.Init(miniGameInfo, spawnInfo, _keySpriteFactory);

        OnMiniGameSpawned?.Invoke(miniGameInstance);
        Managers.Sound.Play("i_mini_say1", Sound.Effect);
    }

    private int GetNextDialogueIndex()
    {
        int dialogueIndex = _randomDialogueIndex[_dialogIndexIter];
        _dialogIndexIter++;
        if (_dialogIndexIter >= _randomDialogueIndex.Count)
        {
            _dialogIndexIter = 0;
            ShuffleDialogueIndex();
        }
        return dialogueIndex;
    }

    private bool TryGetRandomPosition(out Vector2 randomPos)
    {
        if (!_gridSystem.TryGetEmptyPosition(out Vector2 spawnPos))
        {
            Debug.Log("모든 공간이 찼습니다.");
            randomPos = Vector2.zero;
            return false;
        }
        float randomY = UnityEngine.Random.Range(_minBubbleYPos, _maxBubbleYPos);
        randomPos = spawnPos + new Vector2(0, randomY);
        return true;
    }
}