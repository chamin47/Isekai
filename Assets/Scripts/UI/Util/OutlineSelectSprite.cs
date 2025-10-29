using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineSelectSprite : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private int _index;
    private Material _material;    
    public event System.Action<int> OnSelected;

    private void Awake()
    {
        SpriteRenderer image = GetComponent<SpriteRenderer>();
        var material = image.material;
        _material = new Material(material);
        image.material = _material;
    }

    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Debug.Log("OutlineSelectSprite Pointer Enter: " + _index);
        _material.SetFloat("_AlphaCutoff", 1f);
    }

    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        _material.SetFloat("_AlphaCutoff", 0f);
    }

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        Debug.Log("OutlineSelectSprite Clicked: " + _index);
        OnSelected?.Invoke(_index);
    }
}
