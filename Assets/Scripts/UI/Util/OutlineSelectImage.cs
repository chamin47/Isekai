using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineSelectImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Material _material;
    private MaterialPropertyBlock materialPropertyBlock;
    public event System.Action OnSelected;
    private void Awake()
    {
        Image image = GetComponent<Image>();
        var material = image.material;
        _material = new Material(material);
        image.material = _material;
    }

    public void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
    {
        _material.SetFloat("_AlphaCutoff", 1f);
    }

    public void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
    {
        _material.SetFloat("_AlphaCutoff", 0f);
    }

    public void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        OnSelected?.Invoke();
    }
}
