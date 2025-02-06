using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;



public class UI_MiniGame : UI_Popup
{
    
    [SerializeField] private Image _bubbleImage;
    [SerializeField] private TextMeshProUGUI _bubbleText;
    [SerializeField] private Image _minigameGaugeBar;
    [SerializeField] private Image _positionImage;

    private string _originalText;

    private SpawnInfo _spawnInfo;

    [Tooltip("�̴ϰ��� ����")]
    [SerializeField] private MiniGameInfo _miniGameInfo;

    [Tooltip("Ÿ�̸� �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI _remainTimeText;

    [SerializeField] private float _mosaicRemoveSpeed = 1.0f;

    [SerializeField] private string[] _maskCharacters = { "#", "*", "@", "$", "%", "&", "!" }; // Ư������ ����

    private bool _isGameEnd = false;
    private bool _isGameStart = false;
    private bool _canPressKey = true;  

    private float _remainingTime;
    private float _currentGauge;

    private List<KeyButton> _requiredKeys = new List<KeyButton>();

    int _keyCount = 0;

    public float CheetCode = 1f;

    private KeySpriteFactory _keySpriteFactory;
    public void Init(MiniGameInfo miniGameInfo, SpawnInfo spawnInfo, KeySpriteFactory keySpriteFactory)
    {
        //�̴ϰ��� ���� ����
        _miniGameInfo = miniGameInfo;
        _originalText = _miniGameInfo.dialog;
        _bubbleText.text = GetRandomMaskedText(_miniGameInfo.dialog.Length);
        _currentGauge = _miniGameInfo.startGauge;
        _remainingTime = _miniGameInfo.limitTime + 5f;
        _keySpriteFactory = keySpriteFactory;

        SetKeyPressButton();

        _spawnInfo = spawnInfo;

        if (!_spawnInfo.isLeft)
        {
            _bubbleImage.transform.Rotate(0, 180, 0);
        }

        // �ʱ�ȭ
        UpdateUI();
    }

    private string GetRandomMaskedText(int length)
    {
        StringBuilder randomText = new StringBuilder(length);
        for (int i = 0; i < length; i++)
        {
            randomText.Append(_maskCharacters[UnityEngine.Random.Range(0, _maskCharacters.Length)]);
        }
        return randomText.ToString();
    }

    // �ؽ�Ʈ ȿ�� 
    private IEnumerator ShowText()
    {

        int textLength = _originalText.Length;
        float totalDuration = 1.0f; // ��ü ��ȯ �ð�
        float delayPerChar = totalDuration / textLength; // �� ���ڴ� ������

        // StringBuilder�� ����Ͽ� ȿ�������� �ؽ�Ʈ ����
        StringBuilder currentText = new StringBuilder(_bubbleText.text);

        List<int> order = new List<int>();
        for (int i = 0; i < textLength; i++)
        {
            order.Add(i);
        }

        order.Shuffle();

        for (int i = 0; i < textLength; i++)
        {
            int index = order[i];
            // ���� �ؽ�Ʈ�� i��° ���ڸ� ������Ʈ
            currentText[index] = _originalText[index];

            _bubbleText.text = currentText.ToString();
            yield return new WaitForSeconds(delayPerChar);
        }
    }

    private void SetKeyPressButton()
    {
        if (_miniGameInfo.requiredKeys == null) return;

        _requiredKeys.Clear(); // �ʱ�ȭ

        int requiredKeyCount = _miniGameInfo.requiredKeyCount;

        int adder = 0;
        //���� �Է��� ������ ���
        if (_miniGameInfo.canPressConcurrent)
        {
            List<KeyCode> keyCode = new List<KeyCode>();

            int randomKeyCount = UnityEngine.Random.Range(2, 4);
            for (int i = _miniGameInfo.requiredKeys.Count - 1; i >= _miniGameInfo.requiredKeys.Count - randomKeyCount; i--)
            {
                keyCode.Add(_miniGameInfo.requiredKeys[i]);
            }

            if(randomKeyCount == 2)
            {
                // KeyButton ����
                TwoKeyButton keyButton = Managers.UI.MakeSubItem<TwoKeyButton>(_minigameGaugeBar.transform, "TwoKeyButton");
                keyButton.OnKeyPressed += OnKeyPressed; // �Է� �̺�Ʈ ����
                keyButton.Init(keyCode[0], keyCode[1], _keySpriteFactory.GetKeySprite(keyCode[0]), _keySpriteFactory.GetKeySprite(keyCode[1])); // KeyCode ����
                _requiredKeys.Add(keyButton); // ����Ʈ�� �߰�
            }
            else if(randomKeyCount == 3)
            {
                // KeyButton ����
                ThreeKeyButton keyButton = Managers.UI.MakeSubItem<ThreeKeyButton>(_minigameGaugeBar.transform, "ThreeKeyButton");
                keyButton.OnKeyPressed += OnKeyPressed; // �Է� �̺�Ʈ ����
                keyButton.Init(keyCode[0], keyCode[1], keyCode[2], _keySpriteFactory.GetKeySprite(keyCode[0]), _keySpriteFactory.GetKeySprite(keyCode[1]), _keySpriteFactory.GetKeySprite(keyCode[2])); // KeyCode ����
                _requiredKeys.Add(keyButton); // ����Ʈ�� �߰�
            }

            adder = 1;
        }

        for(int i = 0; i < requiredKeyCount - adder; i++)
        {
            // KeyButton ����
            KeyButton keyButton = Managers.UI.MakeSubItem<KeyButton>(_minigameGaugeBar.transform, "KeyButton");
            keyButton.OnKeyPressed += OnKeyPressed; // �Է� �̺�Ʈ ����
            KeyCode keyCode = _miniGameInfo.requiredKeys[i];
            keyButton.Init(keyCode, _keySpriteFactory.GetKeySprite(keyCode)); // KeyCode ����
            _requiredKeys.Add(keyButton); // ����Ʈ�� �߰�
        }

        if (requiredKeyCount > 0)
        {
            _canPressKey = false; // Space ��Ȱ��ȭ
            _keyCount = _requiredKeys.Count;
            SetKeyButtonPosition();
        }
    }


    private void SetKeyButtonPosition()
    {
        int keyCount = _requiredKeys.Count;

        float length = _minigameGaugeBar.rectTransform.rect.width;
        float gap = length / keyCount;
        float startX = -length / 2 + gap / 2;

        for (int i = 0; i < keyCount; i++)
        {
            _requiredKeys[i].transform.localPosition = new Vector3(startX + gap * i, 0, 0);
        }
    }

    private void OnKeyPressed()
    {
        _keyCount--;
        if(_keyCount == 0)
        {
            _canPressKey = true;
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
            if (_canPressKey && Input.GetKeyDown(KeyCode.Space))
            {
                ChangeGauge(_miniGameInfo.perIncreaseGauge);
            }
        }
        else if (_miniGameInfo.difficulty == MiniGameDifficulty.Hard)
        {
            // ���ÿ� Ư�� Ű�� ������ ���̵�
            if (_canPressKey && Input.GetKeyDown(KeyCode.Space))
            {
                ChangeGauge(_miniGameInfo.perIncreaseGauge);
            }
        }
    }

    private void ChangeGauge(float amount)
    {
        _currentGauge += amount;

        if (_currentGauge < 0)
        {
            _currentGauge = 0;
            EndMiniGame(false);
        }

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
            foreach (var key in _requiredKeys)
            {
                key.EnableKeyPress();
            }
            StartCoroutine(ShowText());
        }
    }

    public void ShowExclamation(Vector3 position)
    {
        if (!_positionImage.gameObject.activeSelf) _positionImage.gameObject.SetActive(true);

        _positionImage.transform.position = position;
    }

    public void HideExclamation()
    {
        if (!_positionImage.gameObject.activeSelf) return;

        _positionImage.gameObject.SetActive(false);
    }
    public override void Init()
    {
        // �߰� �ʱ�ȭ�� �ʿ��� ��� ���⿡ �ۼ�
    }
}