using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 1. �̴ϰ����� ������ �÷��̾���� �浹�� ���� ���۵ȴ�.
/// 2. �⺻������ �����̽��� �Է��� ���� ������ �����Ѵ�
/// 3. Ű �Է��� �ִ� ��� ���������� Ű�� �Է��ؾ� �ϸ� �׷��� ���� ��� �̴ϰ����� �����Ѵ�
/// </summary>

public class UI_MiniGame : UI_Popup
{

    [Header("��ǳ��")]
    [SerializeField] private Image _bubbleImage;            // ��ǳ�� �̹���
    [SerializeField] private TextMeshProUGUI _bubbleText;   // ��ǳ�� �ؽ�Ʈ


    [SerializeField] private Image _minigameGaugeBar;       // ���� ��������
    [SerializeField] private Transform _keyBoardTransform;  // Ű�Է� ���� ��ġ

    [Header("Ÿ�̸� �ؽ�Ʈ")]
    [SerializeField] private TextMeshProUGUI _remainTimeText;

    [Header("�̴ϰ��� ����")]
    [SerializeField] private float _keyPositionGap = 0.1f;
    [SerializeField] private float _mosaicRemoveSpeed = 1.0f;


    private bool _isGameEnd = false;                        
    private bool _isGameStart = false;
    private bool _canSpacePress = true;  

    private float _remainTime;
    private float _currentGaugeValue;

    public float CurrentGaugeValue
    {
        get { return _currentGaugeValue; }
        private set
        {
            _currentGaugeValue = value;

            if (_currentGaugeValue < 0)
            {
                _currentGaugeValue = 0;
                EndMiniGame(false);
            }

            if (_currentGaugeValue >= 100f)
            {
                EndMiniGame(true);
                onMiniGameSucced?.Invoke();
            }
        }
    }

    private string _originalText;

    private List<KeyButton> _requiredKeys = new List<KeyButton>();

    // ĳ��
    private KeySpriteFactory _keySpriteFactory;
    private SpawnInfo _spawnInfo;              
    private MiniGameInfo _miniGameInfo;

    private bool _isTextShowed = false;
    int _pressedKeyCount = 0;

    public event Action onMiniGameSucced;

    public void Init(MiniGameInfo miniGameInfo, SpawnInfo spawnInfo, KeySpriteFactory keySpriteFactory)
    {
        _keySpriteFactory = keySpriteFactory;

        _spawnInfo = spawnInfo;

        if (!_spawnInfo.isLeft)
        {
            _bubbleImage.transform.Rotate(0, 180, 0);
        }

        SetMiniGameInfo(miniGameInfo);
        SetKeyPressButton();
        
        // ����� �ڵ����� ������ �ش�.
        FixBubbleSize();

        UpdateUI();
    }

    // �ν�����â �׽�Ʈ ��
    [ContextMenu("FixBubbleSize")]
    private void FixBubbleSize()
    {
        float preferWidth = Mathf.Max(_bubbleText.preferredWidth + 3f, 10f);
        float preferHeight = Mathf.Max(_bubbleText.preferredHeight + 1f, 2.2f);

        Vector2 preferSize = new Vector2(preferWidth, preferHeight);

        _bubbleImage.rectTransform.sizeDelta = preferSize;
    }

    // �����ð�ǥ�� �� �������� ������Ʈ
    private void UpdateUI()
    {
        _minigameGaugeBar.fillAmount = _currentGaugeValue / 100f;
        _remainTimeText.text = $"���� �ð�: {_remainTime:F1}��";
    }

    #region Init
    private void SetMiniGameInfo(MiniGameInfo miniGameInfo)
    {
        _miniGameInfo = miniGameInfo;
        _originalText = _miniGameInfo.dialog;
        _bubbleText.text = _miniGameInfo.dialog.GetRandomMaskedText();
        _currentGaugeValue = _miniGameInfo.startGauge;
        _remainTime = _miniGameInfo.limitTime;
    }
   
    private void SetKeyPressButton()
    {
        if (_miniGameInfo.requiredKeys == null) return;

        _requiredKeys.Clear(); // �ʱ�ȭ

        int requiredKeyCount = _miniGameInfo.requiredKeyCount;

        int hasConcurrentKey = 0;

        // ���� �Է��� ������ ���
        // 50% Ȯ���� 2���� Ű�� ���ÿ� ������ �ϴ� �̴ϰ���
        if (_miniGameInfo.canPressConcurrent)
        {
            hasConcurrentKey = MakeConcurrenceButton(2, 2, 100f);
        }

        for (int i = 0; i < requiredKeyCount - hasConcurrentKey; i++)
        {
            // KeyButton ����
            KeyButton keyButton = Managers.UI.MakeSubItem<KeyButton>(_keyBoardTransform, "KeyButton");
            KeyCode keyCode = _miniGameInfo.requiredKeys[i];
            keyButton.Init(keyCode, _keySpriteFactory.GetKeySprite(keyCode)); // KeyCode ����
            _requiredKeys.Add(keyButton); // ����Ʈ�� �߰�
        }

        if (requiredKeyCount > 0)
        {
            _canSpacePress = false; // Space ��Ȱ��ȭ 
            SetKeyButtonPosition();
        }

        // keyŬ���� �̺�Ʈ ����
        for(int i = 0; i < _requiredKeys.Count; i++)
        {
            _requiredKeys[i].OnKeyPressed += OnKeyPressed;
            _requiredKeys[i].OnKeyMissed += () => EndMiniGame(false);
        }
    }

    // ���� ���ÿ� ���� �� �ִ� Ű�� 2�� �������� ���� ������ �ʿ��� �� �ִ�
    private int MakeConcurrenceButton(int minKeyCount, int maxKeyCount, float spawnPercent)
    {
        // spawnPercent Ȯ���� ���� �Է�Ű ����
        if(UnityEngine.Random.Range(0, 100) > spawnPercent)
        {
            return 0;
        }

        List<KeyCode> keyCode = new List<KeyCode>();
        
        int randomKeyCount = UnityEngine.Random.Range(minKeyCount, maxKeyCount + 1);

        for (int i = _miniGameInfo.requiredKeys.Count - 1; i >= _miniGameInfo.requiredKeys.Count - randomKeyCount; i--)
        {
            keyCode.Add(_miniGameInfo.requiredKeys[i]);
        }
        // KeyButton ����
        TwoKeyButton keyButton = Managers.UI.MakeSubItem<TwoKeyButton>(_keyBoardTransform, "TwoKeyButton");
        keyButton.Init(keyCode[0], keyCode[1], _keySpriteFactory.GetKeySprite(keyCode[0]), _keySpriteFactory.GetKeySprite(keyCode[1])); // KeyCode ����
        _requiredKeys.Add(keyButton); // ����Ʈ�� �߰�
        
        return 1;
    }

    private void SetKeyButtonPosition()
    {
        int keyCount = _requiredKeys.Count;

        List<float> keyWidths = new List<float>();
        float totalWidth = 0;

        for (int i = 0; i < keyCount; i++)
        {
            keyWidths.Add(_requiredKeys[i].Width);
            totalWidth += keyWidths[i];
        }

        totalWidth += _keyPositionGap * (keyCount - 1);

        float startX = -totalWidth / 2;

        for (int i = 0; i < keyCount; i++)
        {
            float posX = startX + keyWidths[i] / 2;
            _requiredKeys[i].transform.localPosition = new Vector3(posX, 0, 0);
            startX += keyWidths[i] + _keyPositionGap;
        }
    }
    #endregion

    private void OnKeyPressed()
    {
        _pressedKeyCount++;

        if (_pressedKeyCount == _requiredKeys.Count)
        {
            _canSpacePress = true;
            return;
        }

        _requiredKeys[_pressedKeyCount].EnableKeyPress();
    }

    private void Update()
    {
        if (_isGameEnd) return;

        // �ð� ����
        _remainTime -= Time.deltaTime;

        if (_remainTime <= 0)
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
                OnSpaceKeyPressed();
            }
        }
        else if (_miniGameInfo.difficulty == MiniGameDifficulty.Normal)
        {
            // ��� Ű�� ���� �� �����̽��ٸ� ������ ���̵�
            if (_canSpacePress && Input.GetKeyDown(KeyCode.Space))
            {
                OnSpaceKeyPressed();
            }
        }
        else if (_miniGameInfo.difficulty == MiniGameDifficulty.Hard)
        {
            // ���ÿ� Ư�� Ű�� ������ ���̵�
            if (_canSpacePress && Input.GetKeyDown(KeyCode.Space))
            {
                OnSpaceKeyPressed();
            }
        }
    }

    private void OnSpaceKeyPressed()
    {
        Managers.Sound.Play("i_mini_space2", Sound.Effect);
        ChangeGauge(_miniGameInfo.perIncreaseGauge);
    }

    private void ChangeGauge(float amount)
    {
        CurrentGaugeValue += amount;
    }
    private void EndMiniGame(bool isSuccess)
    {
        if (_isGameEnd) return;

        _isGameEnd = true;

        if (isSuccess)
        {
            //Debug.Log("�̴ϰ��� ����! �ູ���� ����մϴ�.");
            Managers.Happy.ChangeHappiness(_miniGameInfo.succedGauge);
        }
        else
        {
            //Debug.Log("�̴ϰ��� ����! �ູ���� �����մϴ�.");
            Managers.Happy.ChangeHappiness(_miniGameInfo.failGauge);
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
            MiniGameStart();
        }
    }
    /// <summary>
    /// ����ũ�� �ؽ�Ʈ�� ���� �ε��� ������ ���� �ؽ�Ʈ�� ��ȯ�Ѵ�
    /// </summary>
    private IEnumerator ShowText()
    {
        _isTextShowed = true;

        int textLength = _originalText.Length;
        float totalDuration = 1.0f; // ��ü ��ȯ �ð�
        float delayPerChar = totalDuration / textLength; // �� ���ڴ� ������

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
            yield return WaitForSecondsCache.Get(delayPerChar);
        }
    }

    // ó�� Ű�� ��ŸƮ �������� ����
    private void MiniGameStart()
    {
        foreach (KeyButton key in _requiredKeys)
        {
            if(key != null)
            {
                key.EnableKeyPress();
                break;
            }
        }

        if (_isTextShowed == false)
        {
            StartCoroutine(ShowText());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_isGameStart) return;

        _isGameStart = false;

        if (other.CompareTag("Player"))
        {
            MiniGameStop();
        }
    }

    // Ű �Է��� ����
    private void MiniGameStop()
    {
        foreach (KeyButton key in _requiredKeys)
        {
            if (key != null)
            {
                key.DisableKeyPress();
            }
        }
    }

    public override void Init()
    {
        // �߰� �ʱ�ȭ�� �ʿ��� ��� ���⿡ �ۼ�
    }
}