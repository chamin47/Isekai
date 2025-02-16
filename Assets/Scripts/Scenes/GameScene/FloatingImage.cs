using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingImage : MonoBehaviour
{
    [SerializeField] private Material _material;
    [SerializeField] private float _floatingSpeed = 0.05f;
    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }
    private void Update()
    {
        _material.SetTextureOffset("_MainTex", Vector2.left * _floatingSpeed * Time.time);
    }

    private void Reset()
    {
        // ���� �ٸ� ������Ʈ���� �ٸ� ���� �����ϰ���� ��
        _material = GetComponent<SpriteRenderer>().material;

        // ���� �ٸ� ������Ʈ���� ���� ���� �����ϰ���� ��
        // _material = GetComponent<SpriteRenderer>().sharedMaterial;

        _material.SetTextureOffset("_MainTex", Vector2.zero);
    }
}
