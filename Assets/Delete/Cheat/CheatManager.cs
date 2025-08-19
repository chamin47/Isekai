using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CheatManager : MonoBehaviour
{
    public TMP_Dropdown sceneDropdown;
    public TMP_Dropdown sceneType;
    public Button SetWorldButton;
    public Button SaveCurrentPlayerInfoButton;
    public Button ResetAllInfoButton;
    
    public float playerSpeed = 10f;

    public GameObject cheatUI;

    private void Awake()
    {
        SetWorldButton.onClick.AddListener(SetWorld);
        sceneDropdown.ClearOptions();
        sceneType.ClearOptions();
        sceneDropdown.AddOptions(Enum.GetNames(typeof(WorldType)).ToList());
        sceneType.AddOptions(Enum.GetNames(typeof(Scene)).ToList());
        SaveCurrentPlayerInfoButton.onClick.AddListener(SaveCurrentPlayerInfo);
        ResetAllInfoButton.onClick.AddListener(ResetAllInfo);
        Debug.Log("CheatManager Awake");
        cheatUI?.SetActive(false);
    }

    private void SetWorld()
    {
        Managers.World.CurrentWorldType = (WorldType)sceneDropdown.value;
        Managers.Scene.LoadScene((Scene)sceneType.value);
    }

    private void SaveCurrentPlayerInfo()
    {
        Managers.DebugInfo.SaveDebugInfo();
    }

    private void ResetAllInfo()
    {
        PlayerPrefs.DeleteAll();
        Managers.DebugInfo.ResetData();
        Managers.Scene.LoadScene(Scene.TitleScene);
    }
    

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            cheatUI?.SetActive(!cheatUI.activeSelf);
        }
    }
}
