using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingImage : MonoBehaviour
{
    [Tooltip("�������� ������ �귯����")]
    [SerializeField] private float _floatingSpeed = 0.05f;

    private Material _material;

    private void Awake()
    {
        _material = GetComponent<SpriteRenderer>().material;
    }
    private void Update()
    {
        _material.SetTextureOffset("_MainTex", Vector2.left * _floatingSpeed * Time.deltaTime);
    }

    private void Reset()
    {
        // ���� �ٸ� ������Ʈ���� �ٸ� ���� �����ϰ���� ��
        _material = GetComponent<SpriteRenderer>().material;
        _material.SetTextureOffset("_MainTex", Vector2.zero);
    }
}
