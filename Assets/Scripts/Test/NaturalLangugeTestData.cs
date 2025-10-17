using System.Collections;
using UnityEngine;

// �� ScriptableObject�� ��� �ϳ��ϳ��� �����͸� ��� �˴ϴ�.
[System.Serializable]
[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/TestGoogle", order = 1)]
public class DialogueData : ScriptableObject
{
    public string id;
    public string speaker;
    public string animName;
    public string eventName;
    public string eventParam;
    public string nextID;
    public string nextFalseID;
    public string script;
}
