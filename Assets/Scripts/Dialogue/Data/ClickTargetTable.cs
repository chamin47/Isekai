using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ClickTargetTable", menuName = "Dialogue/ClickTargetTable")]
public class ClickTargetTable : ScriptableObject
{
    [System.Serializable]
    public class Option
    {
        public string targetName;
        public string nextID;
    }

    [System.Serializable]
    public class TargetRow
    {
        public string ChoiceID;
        public List<Option> options = new(); // �� ���� Ȯ�� ����
    }

    public List<TargetRow> targets = new();

    public TargetRow Get(string id)
    {
        return targets.FirstOrDefault(c => c.ChoiceID == id);
    }
}
