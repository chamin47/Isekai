using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHoleSystem : MonoBehaviour
{
    [SerializeField] private BlackHoleObject _blackHole;

    // ºí·¢È¦ ÃâÇö
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            _blackHole.Show();
        }
    }

    // ºí·¢È¦ ¼Ò¸í
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _blackHole.Hide();
        }
    }
}
