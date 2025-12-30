using UnityEditor;
using UnityEngine;
using TMPro;

public static class TMPFontReplaceTool
{
    [MenuItem("Tools/TMP/Replace Font In All Prefabs")]
    public static void ReplaceFontInPrefabs()
    {
        TMP_FontAsset targetFont =
            AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(
                "Assets/100.Font/Galmuri11 SDF.asset");

        string[] prefabGuids =
            AssetDatabase.FindAssets("t:Prefab");

        int count = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab =
                AssetDatabase.LoadAssetAtPath<GameObject>(path);

            TMP_Text[] texts =
                prefab.GetComponentsInChildren<TMP_Text>(true);

            if (texts.Length == 0)
                continue;

            foreach (var text in texts)
            {
                text.font = targetFont;
                EditorUtility.SetDirty(text);
                count++;
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Replaced TMP fonts: {count}");
    }
}