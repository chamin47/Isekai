using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameScene))]
public class GameSceneEditor : Editor
{
    private GameScene _gameSceneEx;
    private void OnEnable()
    {
        _gameSceneEx = target as GameScene; 
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("GameEnd"))
        {
            _gameSceneEx.GameOver(true);
        }

        if (GUILayout.Button("GameOver"))
        {
            _gameSceneEx.GameOver(false);
        }
    }

}
