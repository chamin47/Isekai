using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineSelectImage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Material _material;
    public event System.Action<int> OnSelected;

    [SerializeField] private int _index;
    private void Awake()
    {
        Image image = GetComponent<Image>();
        var material = image.material;
        _material = new Material(material);
        image.material = _material;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _material.SetFloat("_AlphaCutoff", 1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _material.SetFloat("_AlphaCutoff", 0f);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnSelected?.Invoke(_index);
    }
}
