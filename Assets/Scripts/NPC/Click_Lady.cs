using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Click_Lady : MonoBehaviour
{
    [SerializeField] private List<OutlineSelectSprite> _ladies;

    public event System.Action<int> OnClicked;

    bool isClicked = false;
    private int _clickedIndex = -1;
    private void Awake()
    {
        for (int i = 0; i < _ladies.Count; i++)
        {
            _ladies[i].OnSelected += OnClickLady;
        }

        ClickDisable();
    }

    private void OnDestroy()
    {
        for (int i = 0; i < _ladies.Count; i++)
        {
            _ladies[i].OnSelected -= OnClickLady;
        }
    }

    private void ClickEnable()
    {
        for (int i = 0; i < _ladies.Count; i++)
        {
            _ladies[i].enabled = true;
        }
    }

    private void ClickDisable()
    {
        for (int i = 0; i < _ladies.Count; i++)
        {
            _ladies[i].enabled = false;
        }
    }

    private void OnClickLady(int index)
    {
        isClicked = true;
        OnClicked.Invoke(index);
        _clickedIndex = index;
    }

    public IEnumerator ClickLady(System.Action<int> onClicked)
    {
        ClickEnable();
        yield return new WaitUntil(() => isClicked);
        Debug.Log("Clicked Lady Index: " + _clickedIndex);
        onClicked?.Invoke(_clickedIndex);

        List<Coroutine> fadeOutCoroutines = new List<Coroutine>();
        for(int i = 0; i < _ladies.Count; i++)
        {
            if (i != _clickedIndex)
            {
                fadeOutCoroutines.Add(StartCoroutine(_ladies[i].FadeOut()));
            }
        }

        foreach(var coroutine in fadeOutCoroutines)
        {
            yield return coroutine;
        }

        ClickDisable();
    }
}
