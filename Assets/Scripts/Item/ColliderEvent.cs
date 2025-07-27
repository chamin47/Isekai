using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ���� colliding�� ���� event�� �����ϱ� ���� Ŭ����.
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
