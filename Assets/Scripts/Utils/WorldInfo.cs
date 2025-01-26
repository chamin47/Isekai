using System.Collections.Generic;
using UnityEngine;

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
        perDecreaseGauge.AddRange(new List<int>() { -10, -10, -10 });
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
    public override MiniGameInfo GetRandomMiniGameInfo()
    {
        MiniGameInfo miniGameInfo = base.GetRandomMiniGameInfo();

        int keyCount = requiredKeyCount[(int)miniGameInfo.difficulty];

        if (keyCount != 0)
        {
            miniGameInfo.requiredKeys = requireKeys.GetRandomN<KeyCode>(keyCount);
            miniGameInfo.requiredKeyCount = requiredKeyCount;
            miniGameInfo.canPressConcurrent = canPressConcurrent;
        }

        return miniGameInfo;
    }
}