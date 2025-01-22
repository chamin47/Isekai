using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ð��� ���� �������� ���� �پ���
/// �����̽��ٸ� ������ �������� ���ݾ� ����
/// �������� 100���� �ؿ�� �ູ�� ��� �׷��� ������ �ູ�� �϶�
/// 
/// ���̵��� �� �� ������ �������� �ִ�
/// �� :
///     �Ϲ����� ��Ȳ
///     
/// �� :
///     Ű���� Ű�� ��� �������� ���丮���ٸ� ������ �������� ���� ( 3 ~ 6�� ����)
///     
/// �� :
///     ���ÿ� ������ �Ǵ� Ű���嵵 �����Ѵ�
/// </summary>


#region Data
struct MiniGameInfo
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

public class UI_MiniGame : UI_Popup
{
    [SerializeField] private Image _minigameGaugeBar;
    [SerializeField] private WorldInfo _worldInfo;
    [SerializeField] private TextMeshProUGUI _dialogText;
    [SerializeField] private Transform _miniGamePosition;
    [SerializeField] private Transform[] _miniGameSpawnPosition;
    [SerializeField] private TextMeshProUGUI _remainTimeText;

    [SerializeField] private float _createDelayTime = 4f;

    private float _sudoGauge;
    public float SudoGauge
    {
        get => _sudoGauge;
        set
        {
            _sudoGauge = Mathf.Clamp(value, 0f, 100f); // ������ ���� (0 ~ 100)
            _minigameGaugeBar.fillAmount = _sudoGauge / 100f;
        }
    }

    public void Init(WorldInfo worldInfo)
    {
        _worldInfo = worldInfo;
        SudoGauge = 100f;

        StartCoroutine(StartMiniGames());
    }

    private IEnumerator StartMiniGames()
    {
        // ���� �ε��� ����
        List<int> randomIndex = new List<int>(_worldInfo.dialog.Count);
        for (int i = 0; i < _worldInfo.dialog.Count; i++)
        {
            randomIndex.Add(i);
        }
        randomIndex.Shuffle();

        int randomIndexCount = 0;

        // ���̵� ���� ����
        for (int i = 0; i < (int)MiniGameDifficulty.Max; i++)
        {
            for (int j = 0; j < _worldInfo.difficulty[i]; j++)
            {
                if (randomIndexCount >= randomIndex.Count) break;

                MiniGameInfo miniGameInfo = new MiniGameInfo()
                {
                    difficulty = (MiniGameDifficulty)i,
                    startGauge = _worldInfo.startGauge[i],
                    perDecreaseGauge = _worldInfo.perDecreaseGauge[i],
                    perIncreaseGauge = _worldInfo.perIncreaseGauge[i],
                    succedGauge = _worldInfo.succedGauge[i],
                    failGauge = _worldInfo.failGauge[i],
                    runGauge = _worldInfo.runGauge[i],
                    limitTime = _worldInfo.limitTime[i],

                    dialog = _worldInfo.dialog[randomIndex[randomIndexCount++]]

                };

                // key����
                if(_worldInfo is ChaummWorldInfo chaummWorldInfo)
                {
                    miniGameInfo.requiredKeys = chaummWorldInfo.requireKeys;
                    miniGameInfo.requiredKeyCount = chaummWorldInfo.requiredKeyCount;
                    miniGameInfo.canPressConcurrent = chaummWorldInfo.canPressConcurrent;
                }
              

                yield return StartCoroutine(HandleMiniGame(miniGameInfo));
                yield return new WaitForSeconds(_createDelayTime);
                _miniGamePosition.gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator HandleMiniGame(MiniGameInfo miniGameInfo)
    {
        _miniGamePosition.gameObject.SetActive(true);
        Debug.Log($"�̴ϰ��� ����! {miniGameInfo.dialog}");

        // �̴ϰ��� �ʱ�ȭ
        SudoGauge = miniGameInfo.startGauge;
        SetMiniGamePositionAndDialog(miniGameInfo);

        List<KeyCode> requiredKeys = null;
        bool canPressConcurrent = false;

        if (miniGameInfo.requiredKeys != null)
        {
            canPressConcurrent = miniGameInfo.canPressConcurrent[(int)miniGameInfo.difficulty];
            int requiredKeyCount = miniGameInfo.requiredKeyCount[(int)miniGameInfo.difficulty];
            requiredKeys = miniGameInfo.requiredKeys.GetRandomN(requiredKeyCount);
        }

        float remainingTime = miniGameInfo.limitTime;
        _remainTimeText.text = $"���� �ð�: {remainingTime:F1}��";

        Coroutine decreaseGaugeRoutine = StartCoroutine(DecreaseGauge(miniGameInfo.perDecreaseGauge));
        Coroutine miniGameRoutine = StartCoroutine(MiniGameStart(miniGameInfo, requiredKeys, canPressConcurrent));

        float elapsedTime = 0f;
        while (elapsedTime < miniGameInfo.limitTime)
        {
            _remainTimeText.text = $"���� �ð�: {remainingTime - elapsedTime:F1}��";
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        _remainTimeText.text = $"���� �ð�: 0.0��";

        // �ڷ�ƾ ����
        StopCoroutine(decreaseGaugeRoutine);
        StopCoroutine(miniGameRoutine);

        // ��� üũ
        CheckMiniGameResult(miniGameInfo);
    }

    private void CheckMiniGameResult(MiniGameInfo miniGameInfo)
    {
        if (SudoGauge >= 100)
        {
            // ����
            Managers.Happy.ChangeHappiness(miniGameInfo.succedGauge);
            Debug.Log("�̴ϰ��� ����!");
        }
        else
        {
            // ����
            Managers.Happy.ChangeHappiness(miniGameInfo.failGauge);
            Debug.Log("�̴ϰ��� ����...");
        }
    }

    private void SetMiniGamePositionAndDialog(MiniGameInfo miniGameInfo)
    {
        int pos = UnityEngine.Random.Range(0, _miniGameSpawnPosition.Length);
        _miniGamePosition.position = _miniGameSpawnPosition[pos].position;
        _dialogText.text = miniGameInfo.dialog;
    }

    private IEnumerator MiniGameStart(MiniGameInfo miniGameInfo, List<KeyCode> keys, bool canPressConcurrent)
    {
        
        while (true)
        {
            if (miniGameInfo.difficulty == MiniGameDifficulty.Easy)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SudoGauge += miniGameInfo.perIncreaseGauge;
                }
            }
            else if (miniGameInfo.difficulty == MiniGameDifficulty.Normal)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    SudoGauge += miniGameInfo.perIncreaseGauge;
                }
            }
            else if (miniGameInfo.difficulty == MiniGameDifficulty.Hard)
            {
                
            }

            yield return null;
        }
    }

    // 1�ʸ��� ������ ����
    private IEnumerator DecreaseGauge(int perDecreaseGauge)
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SudoGauge += perDecreaseGauge;
        }
    }

    private void SpawnKeys(List<KeyCode> keys, bool canPressConcurrent)
    {
        RectTransform rectTransform = _minigameGaugeBar.rectTransform;
        float startX = rectTransform.position.x - rectTransform.rect.width / 2;
        float endX = rectTransform.position.x + rectTransform.rect.width / 2;

        for (int i = 0; i < keys.Count; i++)
        {
            float interval = (i + 1) / (keys.Count + 1);
            float posX = Mathf.Lerp(startX, endX, interval);
            float posY = rectTransform.position.y;

            Managers.UI.MakeSubItem<KeyButton>(this.transform);
        }
    }

    public override void Init()
    {
        // �߰� �ʱ�ȭ�� �ʿ��� ��� ���⿡ �ۼ�
    }
}