using System.Collections;
using UnityEngine;

// 이 ScriptableObject는 대사 하나하나의 데이터를 담게 됩니다.
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
