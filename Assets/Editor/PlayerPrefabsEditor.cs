using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class PlayerPrefabsEditor : Editor
{
    [MenuItem("PlayerPref/DeleteAll")]
    public static void DeleteAll()
    {
        PlayerPrefs.DeleteKey("IsShowEnding");
        PlayerPrefs.DeleteAll();
        Debug.Log("All PlayerPrefs have been deleted.");
    }

    [MenuItem("PlayerPref/SetWorldType/Vinter")]
    public static void SetWorldType_Vinter()
    {
        SetWorldType(WorldType.Vinter);
    }

    [MenuItem("PlayerPref/SetWorldType/Chaumm")]
    public static void SetWorldType_Chaumm()
    {
        SetWorldType(WorldType.Chaumm);
    }

    [MenuItem("PlayerPref/SetWorldType/Gang")]
    public static void SetWorldType_Gang()
    {
        SetWorldType(WorldType.Gang);
    }

    [MenuItem("PlayerPref/SetWorldType/Pelmanus")]
    public static void SetWorldType_Pelmanus()
    {
        SetWorldType(WorldType.Pelmanus);
    }

    private static void SetWorldType(WorldType type)
    {
        PlayerPrefs.SetInt("WorldType", (int)type);
        Debug.Log($"WorldType set to {type} ({(int)type})");
    }
}
