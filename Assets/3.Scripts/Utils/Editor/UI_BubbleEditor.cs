using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UI_Bubble))]
public class UI_BubbleEditor : Editor
{
    private const string testText = "평범하기 짝이 없으면서 어딜 나서려고 하는 거야?";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        UI_Bubble bubble = target as UI_Bubble;
        if (GUILayout.Button("Init"))
        {
            bubble.Init(testText, 10);
        }
    }
}
