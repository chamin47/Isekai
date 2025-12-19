using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiaryObject : MonoBehaviour
{
    [SerializeField] private OutlineSelectSprite _outlineSelectSprite;
    [SerializeField] private GameObject _diaryUI;
    private void Start()
    {
        _outlineSelectSprite.OnSelected += ShowDiaryUI;
    }

    private void ShowDiaryUI(int obj)
    {
        _diaryUI.SetActive(true);
    }
}
