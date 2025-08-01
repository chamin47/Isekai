using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 1. 미니게임의 시작은 플레이어와의 충돌을 통해 시작된다.
/// 2. 기본적으로 스페이스바 입력을 통해 게임을 제거한다
/// 3. 키 입력이 있는 경우 순차적으로 키를 입력해야 하며 그렇지 않은 경우 미니게임이 실패한다
/// </summary>

public class UI_MiniGame : UI_Popup
{

    [Header("말풍선")]
    [SerializeField] private Image _bubbleImage;            // 말풍선 이미지
    [SerializeField] private TextMeshProUGUI _bubbleText;   // 말풍선 텍스트


    [SerializeField] private Image _minigameGaugeBar;       // 게임 게이지바
    [SerializeField] private Transform _keyBoardTransform;  // 키입력 생성 위치

    //[Header("타이머 텍스트")]
    //[SerializeField] private TextMeshProUGUI _remainTimeText;

    [Header("미니게임 설정")]
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
                Debug.Log("미니게임 실패! 게이지가 0이 되었습니다.");
                Managers.Sound.Play("i_mini_game_fail", Sound.Effect);
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

    // 캐싱
    private KeySpriteFactory _keySpriteFactory;
    private SpawnInfo _spawnInfo;              
    private MiniGameInfo _miniGameInfo;

    private bool _isTextShowed = false;
    int _pressedKeyCount = 0;

    public event Action onMiniGameSucced;
    public event Action onMiniGameDestroyed;

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
        
        // 사이즈를 자동으로 조정해 준다.
        FixBubbleSize();

        UpdateUI();
    }

    // 인스펙터창 테스트 용
    [ContextMenu("FixBubbleSize")]
    private void FixBubbleSize()
    {
        float preferWidth = Mathf.Max(_bubbleText.preferredWidth + 3f, 10f);
        float preferHeight = Mathf.Max(_bubbleText.preferredHeight + 1f, 2.2f);

        Vector2 preferSize = new Vector2(preferWidth, preferHeight);

        _bubbleImage.rectTransform.sizeDelta = preferSize;
    }

    // 남은시간표기 및 게이지바 업데이트
    private void UpdateUI()
    {
        _minigameGaugeBar.fillAmount = _currentGaugeValue / 100f;
       // _remainTimeText.text = $"남은 시간: {_remainTime:F1}초";
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

    private int _hasConrurrentKey = 0;
    private void SetKeyPressButton()
    {
        if (_miniGameInfo.requiredKeys == null) return;

        _requiredKeys.Clear(); // 초기화

        int requiredKeyCount = _miniGameInfo.requiredKeyCount;

        _hasConrurrentKey = 0;

        // 동시 입력이 가능할 경우
        // 50% 확률로 2개의 키를 동시에 눌러야 하는 미니게임
        if (_miniGameInfo.canPressConcurrent)
        {
            _hasConrurrentKey = MakeConcurrenceButton(2, 2, 100f);
        }

        for (int i = 0; i < requiredKeyCount - _hasConrurrentKey; i++)
        {
            // KeyButton 생성
            KeyButton keyButton = Managers.UI.MakeSubItem<KeyButton>(_keyBoardTransform, "KeyButton");
            KeyCode keyCode = _miniGameInfo.requiredKeys[i];
            keyButton.Init(keyCode, _keySpriteFactory.GetKeySprite(keyCode)); // KeyCode 설정
            _requiredKeys.Add(keyButton); // 리스트에 추가
        }

        if (requiredKeyCount > 0)
        {
            _canSpacePress = false; // Space 비활성화 
            SetKeyButtonPosition();
        }

        // key클리어 이벤트 연결
        for(int i = 0; i < _requiredKeys.Count; i++)
        {
            _requiredKeys[i].OnKeyPressed += OnKeyPressed;
            _requiredKeys[i].OnKeyMissed += () => EndMiniGame(false);
        }
    }

    // 현재 동시에 누를 수 있는 키는 2개 한정으로 추후 수정이 필요할 수 있다
    private int MakeConcurrenceButton(int minKeyCount, int maxKeyCount, float spawnPercent)
    {
        // spawnPercent 확률로 동시 입력키 생성
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
        // KeyButton 생성
        TwoKeyButton keyButton = Managers.UI.MakeSubItem<TwoKeyButton>(_keyBoardTransform, "TwoKeyButton");
        keyButton.Init(keyCode[0], keyCode[1], _keySpriteFactory.GetKeySprite(keyCode[0]), _keySpriteFactory.GetKeySprite(keyCode[1])); // KeyCode 설정
        _requiredKeys.Add(keyButton); // 리스트에 추가
        
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

        if(_hasConrurrentKey == 1)
        {
            float posX = startX + keyWidths[0] / 2 - 0.3f;
            _requiredKeys[0].transform.localPosition = new Vector3(posX, 0, 0);
            startX += keyWidths[0] + _keyPositionGap + 0.2f;

            for (int i = 1; i < keyCount; i++)
            {
                float posX2 = startX + keyWidths[i] / 2;
                _requiredKeys[i].transform.localPosition = new Vector3(posX2, 0, 0);
                startX += keyWidths[i] + _keyPositionGap;
            }
        }
        else
        {
            for (int i = 0; i < keyCount; i++)
            {
                float posX = startX + keyWidths[i] / 2;
                _requiredKeys[i].transform.localPosition = new Vector3(posX, 0, 0);
                startX += keyWidths[i] + _keyPositionGap;
            }
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

        StartCoroutine(EnableKeyNextFrame(_pressedKeyCount));
    }

    private IEnumerator EnableKeyNextFrame(int keyIndex)
    {
        yield return null; // 다음 프레임까지 대기
        _requiredKeys[keyIndex].EnableKeyPress();
    }

    private void Update()
    {
        if (_isGameEnd) return;

        // 시간 감소
        _remainTime -= Time.deltaTime;

        if (_remainTime <= 0)
        {
            Debug.Log("미니게임 실패! 시간이 다 되었습니다.");
            Managers.Sound.Play("i_mini_pass", Sound.Effect);
            EndMiniGame(false);
            return;
        }

        // 게이지 감소
        ChangeGauge(_miniGameInfo.perDecreaseGauge * Time.deltaTime);

        // 키 입력 처리
        HandleKeyPress();

        // UI 업데이트
        UpdateUI();
    }

    private void HandleKeyPress()
    {
        if(!_isGameStart) return;


        if (_miniGameInfo.difficulty == MiniGameDifficulty.Easy)
        {
            // 스페이스바를 누르면 게이지 증가
            if (Input.GetKeyDown(KeyCode.Space))
            {
                OnSpaceKeyPressed();
            }
        }
        else if (_miniGameInfo.difficulty == MiniGameDifficulty.Normal)
        {
            // 모든 키를 누른 후 스페이스바를 누르는 난이도
            if (_canSpacePress && Input.GetKeyDown(KeyCode.Space))
            {
                OnSpaceKeyPressed();
            }
        }
        else if (_miniGameInfo.difficulty == MiniGameDifficulty.Hard)
        {
            // 동시에 특정 키를 누르는 난이도
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
            //Debug.Log("미니게임 성공! 행복도가 상승합니다.");
            Managers.Sound.Play("i_mini_game_success", Sound.Effect);
            Managers.Happy.AddHappiness(_miniGameInfo.succedGauge);
        }
        else
        {
            //Debug.Log("미니게임 실패! 행복도가 감소합니다.");
            Managers.Happy.AddHappiness(_miniGameInfo.failGauge);
        }

        // 게임 종료 로직
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
    /// 마스크된 텍스트를 랜덤 인덱스 순으로 원래 텍스트로 변환한다
    /// </summary>
    private IEnumerator ShowText()
    {
        _isTextShowed = true;

        int textLength = _originalText.Length;
        float totalDuration = 0.5f; // 전체 변환 시간
        float delayPerChar = totalDuration / textLength; // 각 글자당 딜레이

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
            // 원래 텍스트의 i번째 글자를 업데이트
            currentText[index] = _originalText[index];

            _bubbleText.text = currentText.ToString();
            yield return WaitForSecondsCache.Get(delayPerChar);
        }
    }

    // 처음 키를 스타트 지점으로 설정
    private void MiniGameStart()
    {
        _isGameStart = true;

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

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!_isGameStart) return;

        _isGameStart = false;

        if (other.CompareTag("Player"))
        {
            MiniGameStop();
        }
    }

    // 키 입력을 중지
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
        // 추가 초기화가 필요한 경우 여기에 작성
    }

    private void OnDestroy()
    {
        onMiniGameDestroyed?.Invoke();
    }
}