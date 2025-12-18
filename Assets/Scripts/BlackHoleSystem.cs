using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSystem : MonoBehaviour
{
    [SerializeField] private BlackHole _blackHole;
    [SerializeField] private Transform _target;

    // ºí·¢È¦ ÃâÇö
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _blackHole.StartChase();
        }
    }

    // ºí·¢È¦ ¼Ò¸í
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
        }
    }
}
