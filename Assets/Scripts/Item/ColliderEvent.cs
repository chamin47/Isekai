using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 추후 여러 colliding에 대한 event를 전달하기 위한 클래스.
/// </summary>
public class ColliderEvent : MonoBehaviour
{
    private bool IsColliding = false;
    public bool IsCollidingWithPlayer => IsColliding;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("Player has entered the collider area.");
            IsColliding = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            IsColliding = false;
        }
    }
}
