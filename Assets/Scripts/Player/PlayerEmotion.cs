using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 플레이어 좌절 이모션 관리
/// </summary>
public class PlayerEmotion : MonoBehaviour
{
    [SerializeField] List<Sprite> emotionSprites;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Animator animator;
    public void Frusted(int index)
    {
        animator.enabled = false;
        spriteRenderer.sprite = emotionSprites[index];
    }
}
