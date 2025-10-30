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
        _material.SetFloat("_Thickness", 0.02f);
    }

    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        _material.SetFloat("_Thickness", 0f);
    }

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        OnSelected?.Invoke(_index);
        _material.SetFloat("_Thickness", 0f);
        Managers.Sound.Play("duke_choice", Sound.Effect);
    }
}
