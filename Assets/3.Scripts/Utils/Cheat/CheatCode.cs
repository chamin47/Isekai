using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatCode : MonoBehaviour
{
    public CheatManager cheatManager;

    private void Awake()
    {
        if(FindObjectsOfType<CheatCode>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftBracket) && Input.GetKey(KeyCode.RightBracket))
        {
            cheatManager.gameObject.SetActive(!cheatManager.gameObject.activeSelf);
        }
    }
}
