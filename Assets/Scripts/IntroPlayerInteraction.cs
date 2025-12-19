using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPlayerInteraction : MonoBehaviour
{
    [SerializeField] private Transform _realWorldPosition;
    [SerializeField] private Transform _homeWorldPositoin;
    [SerializeField] private IntroCameraController _introCameraController;
    [SerializeField] private BoxCollider2D _realWorldCollider;  
    [SerializeField] private BoxCollider2D _homeWorldCollider;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Home"))
        {
            if (collision.transform.position.x < transform.position.x)
            {
                _introCameraController.ChangeBoxBounds(_homeWorldCollider);
                _introCameraController.CoMoveTo(_homeWorldPositoin.position);
            }
        }
        else if(collision.CompareTag("RealWorld"))
        {
            if (collision.transform.position.x > transform.position.x)
            {
                _introCameraController.ChangeBoxBounds(_realWorldCollider);
                _introCameraController.CoMoveTo(_realWorldPosition.position);
            }
        }
    }


}
