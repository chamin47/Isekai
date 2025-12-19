using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutlineSelectSprite : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [SerializeField] private int _index;
    private Material _material;    
    public event System.Action<int> OnSelected;

    [SerializeField] private float _fadeDuration = 0.3f;

    private void Awake()
    {
        SpriteRenderer image = GetComponent<SpriteRenderer>();
        var material = image.material;
        _material = new Material(material);
        image.material = _material;
    }

    public IEnumerator FadeIn()
    {
        this.gameObject.SetActive(true);

        float duration = _fadeDuration;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            _material.SetFloat("_Alpha", alpha);
            yield return null;
        }
        _material.SetFloat("_Alpha", 1f);
    }

    public IEnumerator FadeOut()
    {
        float duration = _fadeDuration;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = 1f - Mathf.Clamp01(elapsed / duration);
            _material.SetFloat("_Alpha", alpha);
            yield return null;
        }
        _material.SetFloat("_Alpha", 0f);

        this.gameObject.SetActive(false);
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
    }
}
