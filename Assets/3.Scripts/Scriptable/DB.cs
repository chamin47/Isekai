using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LoadingSceneData
{
    public string name;
    public WorldType worldType;
    public string tip;
    public Sprite backgroundImage;
    public List<Sprite> subImages;
}

[System.Serializable]
public struct LoadingGameSceneData
{
    public WorldType worldType;
    public List<string> todoList;
    public string diary;
    public string startDate;
    public string endDate;
    public string worldName;
}


[System.Serializable]
public struct LibrarySceneData
{
    public WorldType worldType;
    public LibraryBookData bookData;
}

[System.Serializable]
public struct LibraryBookData
{
    public string title;
    public string content;
}


[System.Serializable]
public struct RealGameSceneData
{
    public WorldType worldType;
    public float cameraSpeed;
}

[System.Serializable]
public struct GameSceneData
{
    public WorldType worldType;
    public List<int> difficulty;
    public List<int> startGauge;
    public List<int> perDecreaseGauge;
    public List<int> perIncreaseGauge;
    public List<int> succedGauge;
    public List<int> failGauge;
    public List<int> runGauge;
    public List<int> limitTime;
    public List<string> dialog;
    public List<KeyCode> requireKeys;
    public List<int> requiredKeyCount;
    public List<bool> canPressConcurrent;
}

[System.Serializable]
public struct PlayerData
{
    public List<float> moveSpeed;
}

[System.Serializable]
public struct EndingSceneData
{
    public List<string> newsDialog;
    public List<string> finalDialog;
}


[System.Serializable]
public struct HappinessLevel
{
    public float threshold; // 이 행복도 이상일 때 적용될 기준값
    public Sprite sprite; // 해당 레벨의 표정 스프라이트
}


[CreateAssetMenu(fileName = "DB", menuName = "ScriptableObject/DB", order = 0)]
public class DB : ScriptableObject
{
    [SerializeField] private List<LoadingSceneData> loadingTipDataList = new();
    private Dictionary<WorldType, LoadingSceneData> loadingTipDataDic = new();

    [SerializeField] private List<LoadingGameSceneData> loadingGameSceneDataList = new();
    private Dictionary<WorldType, LoadingGameSceneData> loadingGameSceneDataDic = new();

    [SerializeField] private List<RealGameSceneData> realGameSceneDataList = new();
    private Dictionary<WorldType, RealGameSceneData> realGameSceneDataDic = new();

    [SerializeField] private List<LibrarySceneData> librarySceneDataList = new();
    private Dictionary<WorldType, LibrarySceneData> librarySceneDataDic = new();

    [SerializeField] private PlayerData playerData = new();
    private PlayerData _gamePlayerData = new();

    [SerializeField] private EndingSceneData endingSceneData;

    [SerializeField] private List<HappinessLevel> happinessLevels = new();
    public List<HappinessLevel> HappinessLevels => happinessLevels;

    [SerializeField] private List<SoundData> soundDatas = new();
    private Dictionary<string, SoundData> soundDataDic = new();


    public void Init()
    {
//        #region 치트수정 방지용
//#if Unity_Editor
//        string savePath = Application.dataPath + "/Datas";
//        string path = System.IO.Path.Combine(savePath, "DB.json");
//        string json = System.IO.File.ReadAllText(path);

//        JsonUtility.FromJsonOverwrite(json, this);
//#endif
//        #endregion

        loadingTipDataDic.Clear();
        foreach (var data in loadingTipDataList)
        {
            loadingTipDataDic.Add(data.worldType, data);
        }

        realGameSceneDataDic.Clear();
        foreach (var data in realGameSceneDataList)
        {
            realGameSceneDataDic.Add(data.worldType, data);
        }

        librarySceneDataDic.Clear();
        foreach (var data in librarySceneDataList)
        {
            librarySceneDataDic.Add(data.worldType, data);
        }

        loadingGameSceneDataDic.Clear();
        foreach (var data in loadingGameSceneDataList)
        {
            loadingGameSceneDataDic.Add(data.worldType, data);
        }

        // 데이터 참조지역 추후 초기화
        _gamePlayerData = playerData;
    }

    public LoadingSceneData GetLoadingSceneData(WorldType worldType)
    {
        if (loadingTipDataDic.TryGetValue(worldType, out LoadingSceneData data))
        {
            return data;
        }

        return new LoadingSceneData();
    }

    public RealGameSceneData GetRealGameSceneData(WorldType worldType)
    {
        if (realGameSceneDataDic.TryGetValue(worldType, out RealGameSceneData data))
        {
            return data;
        }
        return new RealGameSceneData();
    }

    public LibrarySceneData GetLibrarySceneData(WorldType worldType)
    {
        if (librarySceneDataDic.TryGetValue(worldType, out LibrarySceneData data))
        {
            return data;
        }
        return new LibrarySceneData();
    }

    public EndingSceneData GetEndingSceneData()
    {
        return endingSceneData;
    }

    public PlayerData GetPlayerData()
    {
        return _gamePlayerData;
    }

    public LoadingGameSceneData GetLoadingGameSceneData(WorldType worldType)
    {
        if (loadingGameSceneDataDic.TryGetValue(worldType, out LoadingGameSceneData data))
        {
            return data;
        }
        return new LoadingGameSceneData();
    }

    public void SetPlayerData(PlayerData data)
    {
        _gamePlayerData = data;
    }

    public void ResetPlayerData()
    {
        _gamePlayerData = playerData;
    }
}
