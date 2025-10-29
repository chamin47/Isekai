using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click_Lady : MonoBehaviour
{
    [SerializeField] private OutlineSelectSprite _lady1;
    [SerializeField] private OutlineSelectSprite _lady2;
    [SerializeField] private OutlineSelectSprite _lady3;

    public event System.Action<int> OnClicked;

    bool isClicked = false;
    private int _clickedIndex = -1;
    private void Awake()
    {
        
        _lady1.OnSelected += OnClickLady;
        _lady2.OnSelected += OnClickLady;
        _lady3.OnSelected += OnClickLady;
    }

    private void OnDestroy()
    {
        _lady1.OnSelected -= OnClickLady;
        _lady2.OnSelected -= OnClickLady;
        _lady3.OnSelected -= OnClickLady;
    }

    private void OnClickLady(int index)
    {
        isClicked = true;
        OnClicked.Invoke(index);
        _clickedIndex = index;
    }

    public IEnumerator ClickLady(System.Action<int> onClicked)
    {
        _lady1.enabled = true;
        _lady2.enabled = true;
        _lady3.enabled = true;
        yield return new WaitUntil(() => isClicked);
        Debug.Log("Clicked Lady Index: " + _clickedIndex);
        onClicked?.Invoke(_clickedIndex);
        _lady1.enabled = false;
        _lady2.enabled = false;
        _lady3.enabled = false;
    }
}
