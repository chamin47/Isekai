using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Click_Flower : UI_Popup
{
    [SerializeField] private OutlineSelectImage flower1;
    [SerializeField] private OutlineSelectImage flower2;

    bool isClicked = false;
    int selectedIndex = -1;
    private void Awake()
    {
        flower1.OnSelected += OnClickFlower;
        flower2.OnSelected += OnClickFlower;
    }

    private void OnDestroy()
    {
        flower1.OnSelected -= OnClickFlower;
        flower2.OnSelected -= OnClickFlower;
    }

    private void OnClickFlower(int index)
    {
        isClicked = true;
        selectedIndex = index;
    }

    public IEnumerator ClickFlower(Action<int> onClicked)
    {
        yield return new WaitUntil(() => isClicked);
        onClicked.Invoke(selectedIndex);
        Destroy(gameObject);
    }
}
