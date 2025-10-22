using System.Collections.Generic;
using UnityEngine;

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
        foreach (var d in all)
        {
            if (string.IsNullOrWhiteSpace(d.id)) continue;
            map[d.id] = d;
        }
        Debug.Log($"Dialogue DB loaded: {map.Count} rows");
    }

    public bool TryGet(string id, out DialogueData row)
    {
        if (map == null)
        {
            row = null;          // <- out 매개변수 보장
            return false;
        }

        return map.TryGetValue(id, out row);
    }
}
