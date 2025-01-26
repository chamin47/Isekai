using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_MiniGame : UI_Popup
{
    [SerializeField] private Image _bubbleImage;
    [SerializeField] private TextMeshProUGUI _bubbleText;
    [SerializeField] private Image _minigameGaugeBar;

    private SpawnInfo _spawnInfo;

    [Tooltip("�̴ϰ��� ����")]
    [SerializeField] private MiniGameInfo _miniGameInfo;

    [Tooltip("Ÿ�̸� �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI _remainTimeText;

    [SerializeField] private float _mosaicRemoveSpeed = 1.0f;

    private bool _isGameEnd = false;
    private bool _isGameStart = false;

    private float _remainingTime;
    private float _currentGauge;

    private List<KeyCode> _requiredKeys = new List<KeyCode>(); // ���̵����� �ʿ��� Ű

    public float CheetCode = 1f;
    public void Init(MiniGameInfo miniGameInfo, SpawnInfo spawnInfo)
    {        
        //�̴ϰ��� ���� ����
        _miniGameInfo = miniGameInfo;
        _bubbleText.text = _miniGameInfo.dialog;
        _currentGauge = _miniGameInfo.startGauge;
        _remainingTime = _miniGameInfo.limitTime + 5f;

        _spawnInfo = spawnInfo;

        if (_spawnInfo.isLeft)
        {
            _bubbleImage.transform.Rotate(0, 180, 0);
        }

        // �ʱ�ȭ
        


        GenerateKeysByDifficulty();
        UpdateUI();
    }

    private IEnumerator ShowText()
    {
        while (_bubbleText.outlineWidth > 0f)
        {
            _bubbleText.outlineWidth -= _mosaicRemoveSpeed * Time.deltaTime;
            yield return null;
        }
    }

    private void Update()
    {
        if (_isGameEnd) return;

        // �ð� ����
        _remainingTime -= Time.deltaTime;
        if (_remainingTime <= 0)
        {
            EndMiniGame(false);
            return;
        }

        // ������ ����
        ChangeGauge(_miniGameInfo.perDecreaseGauge * Time.deltaTime);
        if (_currentGauge <= 0)
        {
            EndMiniGame(false);
            return;
        }

        // Ű �Է� ó��
        HandleKeyPress();

        // UI ������Ʈ
        UpdateUI();
    }

    private void HandleKeyPress()
    {
        if(!_isGameStart) return;


        if (_miniGameInfo.difficulty == MiniGameDifficulty.Easy)
        {
            // �����̽��ٸ� ������ ������ ����
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeGauge(_miniGameInfo.perIncreaseGauge + CheetCode);
            }
        }
        else if (_miniGameInfo.difficulty == MiniGameDifficulty.Normal)
        {
            // ��� Ű�� ���� �� �����̽��ٸ� ������ ���̵�
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ChangeGauge(_miniGameInfo.perIncreaseGauge);
                ResetKeyStates();
            }
        }
        else if (_miniGameInfo.difficulty == MiniGameDifficulty.Hard)
        {
            // ���ÿ� Ư�� Ű�� ������ ���̵�
            if (AreAllKeysPressed())
            {
                ChangeGauge(_miniGameInfo.perIncreaseGauge);
                ResetKeyStates();
            }
        }
    }

    private void GenerateKeysByDifficulty()
    {
        _requiredKeys.Clear();
        if (_miniGameInfo.difficulty == MiniGameDifficulty.Normal || _miniGameInfo.difficulty == MiniGameDifficulty.Hard)
        {
            int keyCount = UnityEngine.Random.Range(3, 7); // 3~6���� Ű�� �������� ����
            Array values = Enum.GetValues(typeof(KeyCode));
            for (int i = 0; i < keyCount; i++)
            {
                KeyCode randomKey = (KeyCode)values.GetValue(UnityEngine.Random.Range(0, values.Length));
                _requiredKeys.Add(randomKey);
            }
        }
    }

    private bool AreAllKeysPressed()
    {
        foreach (var key in _requiredKeys) 
        {
            if (!Input.GetKey(key))
                return false;
        }
        return true;
    }

    private void ResetKeyStates()
    {
        // Ű ���� �ʱ�ȭ (�ʿ� ��)
    }

    private void ChangeGauge(float amount)
    {
        _currentGauge += amount;
        if (_currentGauge >= 100f)
        {
            EndMiniGame(true);
        }
    }

    private void UpdateUI()
    {
        _minigameGaugeBar.fillAmount = _currentGauge / 100f;
        _remainTimeText.text = $"���� �ð�: {_remainingTime:F1}��";
    }

    private void EndMiniGame(bool isSuccess)
    {
        if (_isGameEnd) return;

        _isGameEnd = true;

        if (isSuccess)
        {
            Debug.Log("�̴ϰ��� ����! �ູ���� ����մϴ�.");
            Managers.Happy.ChangeHappiness(_miniGameInfo.succedGauge + CheetCode);
        }
        else
        {
            Debug.Log("�̴ϰ��� ����! �ູ���� �����մϴ�.");
            Managers.Happy.ChangeHappiness(_miniGameInfo.failGauge - CheetCode);
        }

        // ���� ���� ����
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isGameStart) return;

        _isGameStart = true;

        if (collision.CompareTag("Player"))
        {
            StartCoroutine(ShowText());
        }
    }

    public override void Init()
    {
        // �߰� �ʱ�ȭ�� �ʿ��� ��� ���⿡ �ۼ�
    }
}