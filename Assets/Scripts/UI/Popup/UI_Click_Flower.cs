using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Click_Flower : UI_Popup
{
    [SerializeField] private OutlineSelectImage flower1;
    [SerializeField] private OutlineSelectImage flower2;

    bool isClicked = false;

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

    private void OnClickFlower()
    {
        isClicked = true;
    }

    public IEnumerator ClickFlower()
    {
        yield return new WaitUntil(() => isClicked);
        Destroy(gameObject);
    }
}
