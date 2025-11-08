using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingImage : MonoBehaviour
{
    [Tooltip("높을수록 빠르게 흘러간다")]
    [SerializeField] private float _floatingSpeed = 0.05f;

    private Material _material;
    private Vector2 _offset = Vector2.zero;
    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }
    private void Update()
    {
        _offset += Vector2.left * _floatingSpeed * Time.deltaTime;
        _material.SetTextureOffset("_MainTex", _offset);
    }

    private void Reset()
    {
        // 서로 다른 오브젝트에서 다른 값을 적용하고싶을 때
        _material = GetComponent<SpriteRenderer>().material;
        _material.SetTextureOffset("_MainTex", Vector2.zero);
    }
}
