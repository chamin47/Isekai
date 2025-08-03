using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;


// 1.어느 World, 어느 씬에서 그만했는지
// 2. 이세계 미니게임별 클리어 시간
// 3. 이세계별 게임 오버 횟수

[Serializable]
public class WorldDebugInfo
{
    public float clearTotalTime; // 이세계 별 클리어 시간
    public int gameOverCount; // 이세계 별 게임 오버 횟수
}

[Serializable]
public class DebugInfoManager
{
    // 최종 도착된 월드 정보 -> 도서관 기준으로 설정
    public WorldType LastWorldType = WorldType.None;
    // 그만둔 씬 정보 -> 저장된 씬 정보
    public Scene LastSceneType = Scene.Unknown;
    public Dictionary<WorldType, WorldDebugInfo> worldInfo = new Dictionary<WorldType, WorldDebugInfo>();

    public string SaveFilePath;

    public void Init()
    {
        SaveFilePath = Directory.GetCurrentDirectory();
        SaveFilePath = Path.Combine(SaveFilePath, "GameDebugLog.csv");
        Debug.Log($"[DebugInfoManager] 로그 파일이 저장될 경로: {SaveFilePath}");

        if (!File.Exists(SaveFilePath))
        {
            StringBuilder headerBuilder = new StringBuilder();
            headerBuilder.Append("저장시간,최종 도착 월드,종료시점 마지막 씬");

            foreach (WorldType world in Enum.GetValues(typeof(WorldType)))
            {
                if (world == WorldType.None) continue;

                headerBuilder.Append($",{world}_클리어시간, {world}_실패 횟수");
            }
            headerBuilder.Append("\n");

            File.WriteAllText(SaveFilePath, headerBuilder.ToString(), Encoding.UTF8);
        }
    }

    /// <summary>
    /// 현재 디버그 정보를 CSV 파일에 추가(저장)합니다.
    /// </summary>
    public void SaveDebugInfo()
    {
        if (string.IsNullOrEmpty(SaveFilePath))
        {
            Debug.LogError("[DebugInfoManager] Init()이 먼저 호출되어야 합니다. 저장을 건너뜁니다.");
            return;
        }

        try
        {
            StringBuilder lineBuilder = new StringBuilder();
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // 1. 고정된 기본 정보 추가
            lineBuilder.Append($"{timestamp},{LastWorldType},{LastSceneType}");

            // 2. 헤더 순서와 동일하게 WorldType enum의 모든 값을 순회하며 데이터 추가
            foreach (WorldType world in Enum.GetValues(typeof(WorldType)))
            {
                if (world == WorldType.None) continue;

                // 해당 world에 대한 정보가 딕셔너리에 있는지 확인
                if (worldInfo.TryGetValue(world, out WorldDebugInfo info))
                {
                    // 정보가 있으면 해당 값을 추가
                    lineBuilder.Append($",{info.clearTotalTime},{info.gameOverCount}");
                }
                else
                {
                    // 정보가 없으면 기본값(0)을 추가하여 열을 맞춤
                    lineBuilder.Append(",0,0");
                }
            }
            lineBuilder.Append("\n"); // 줄바꿈 추가

            // 완성된 한 줄의 문자열을 파일의 끝에 추가합니다.
            File.AppendAllText(SaveFilePath, lineBuilder.ToString(), Encoding.UTF8);

            Debug.Log($"[DebugInfoManager] {SaveFilePath}에 현재 디버그 정보를 성공적으로 저장했습니다.");
        }
        catch (Exception e)
        {
            Debug.LogError($"[DebugInfoManager] 파일 저장 중 오류 발생: {e.Message}");
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
