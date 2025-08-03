using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


// 1.��� World, ��� ������ �׸��ߴ���
// 2. �̼��� �̴ϰ��Ӻ� Ŭ���� �ð�
// 3. �̼��躰 ���� ���� Ƚ��

[Serializable]
public class WorldDebugInfo
{
    public float clearTotalTime; // �̼��� �� Ŭ���� �ð�
    public int gameOverCount; // �̼��� �� ���� ���� Ƚ��
}

[Serializable]
public class DebugInfoManager
{
    // ���� ������ ���� ���� -> ������ �������� ����
    public WorldType LastWorldType = WorldType.None;
    // �׸��� �� ���� -> ����� �� ����
    public Scene LastSceneType = Scene.Unknown;
    public Dictionary<WorldType, WorldDebugInfo> worldInfo = new Dictionary<WorldType, WorldDebugInfo>();

    public string SaveFilePath;

    public void Init()
    {
        SaveFilePath = Directory.GetCurrentDirectory();
        SaveFilePath = Path.Combine(SaveFilePath, "GameDebugLog.csv");
        Debug.Log($"[DebugInfoManager] �α� ������ ����� ���: {SaveFilePath}");

        if (!File.Exists(SaveFilePath))
        {
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.Append("����ð�,���� ���� ����,������� ������ ��");

            foreach (WorldType world in Enum.GetValues(typeof(WorldType)))
            {
                if (world == WorldType.None) continue;

                headerBuilder.Append($",{world}_Ŭ����ð�, {world}_���� Ƚ��");
            }
            headerBuilder.Append("\n");

            File.WriteAllText(SaveFilePath, headerBuilder.ToString(), Encoding.UTF8);
        }
    }

    /// <summary>
    /// ���� ����� ������ CSV ���Ͽ� �߰�(����)�մϴ�.
    /// </summary>
    public void SaveDebugInfo()
    {
        if (string.IsNullOrEmpty(SaveFilePath))
        {
            Debug.LogError("[DebugInfoManager] Init()�� ���� ȣ��Ǿ�� �մϴ�. ������ �ǳʶݴϴ�.");
            return;
        }

        try
        {
            StringBuilder lineBuilder = new StringBuilder();
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 1. ������ �⺻ ���� �߰�
            lineBuilder.Append($"{timestamp},{LastWorldType},{LastSceneType}");

            // 2. ��� ������ �����ϰ� WorldType enum�� ��� ���� ��ȸ�ϸ� ������ �߰�
            foreach (WorldType world in Enum.GetValues(typeof(WorldType)))
            {
                if (world == WorldType.None) continue;

                // �ش� world�� ���� ������ ��ųʸ��� �ִ��� Ȯ��
                if (worldInfo.TryGetValue(world, out WorldDebugInfo info))
                {
                    // ������ ������ �ش� ���� �߰�
                    lineBuilder.Append($",{info.clearTotalTime},{info.gameOverCount}");
                }
                else
                {
                    // ������ ������ �⺻��(0)�� �߰��Ͽ� ���� ����
                    lineBuilder.Append(",0,0");
                }
            }
            lineBuilder.Append("\n"); // �ٹٲ� �߰�

            // �ϼ��� �� ���� ���ڿ��� ������ ���� �߰��մϴ�.
            File.AppendAllText(SaveFilePath, lineBuilder.ToString(), Encoding.UTF8);

            Debug.Log($"[DebugInfoManager] {SaveFilePath}�� ���� ����� ������ ���������� �����߽��ϴ�.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DebugInfoManager] ���� ���� �� ���� �߻�: {e.Message}");
        }
    }
    public void SetClearTime(WorldType worldType, float clearTime)
    {
        if (!worldInfo.ContainsKey(worldType))
        {
            worldInfo[worldType] = new WorldDebugInfo();
        }
        worldInfo[worldType].clearTotalTime += clearTime;
    }

    public void IncrementGameOverCount(WorldType worldType)
    {
        if (!worldInfo.ContainsKey(worldType))
        {
            worldInfo[worldType] = new WorldDebugInfo();
        }
        worldInfo[worldType].gameOverCount++;
    }
}
