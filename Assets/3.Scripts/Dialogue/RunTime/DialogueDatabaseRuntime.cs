using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 런타임 대화 DB 로더.
/// Resources/DB 에서 DialogueData 에셋을 읽어 ID→Row 매핑 딕셔너리를 구성한다.
/// </summary>
public class DialogueDatabaseRuntime : MonoBehaviour
{
    public Dictionary<string, DialogueData> map { get; private set; }

	void Awake()
	{
		if (map == null) LoadAllFromResources();
	}

	public void LoadAllFromResources()
    {
        map = new Dictionary<string, DialogueData>();
        var all = Resources.LoadAll<DialogueData>("DB");
        foreach (var data in all)
        {
            if (string.IsNullOrWhiteSpace(data.id)) 
                continue;
            map[data.id] = data;
        }
        Debug.Log($"Dialogue DB loaded: {map.Count} rows");
    }

    public bool TryGet(string id, out DialogueData row)
    {
        if (map == null)
        {
            row = null;         
            return false;
        }

        return map.TryGetValue(id, out row);
    }
}
