using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameIndicator : MonoBehaviour
{
    private float _originHeight;
    private void Awake()
    {
        _originHeight = transform.position.y;
    }

    private void Update()
    {
        RaycastHit2D hit = Physics2D.BoxCast(
            new Vector2(transform.position.x, _originHeight),
            new Vector2(0.5f, 0.5f),
            0f,
            Vector2.zero,
            0f,
            LayerMask.GetMask("UI")
        );

        if(hit.collider != null)
        {
            transform.position = new Vector3(transform.position.x, hit.collider.gameObject.transform.position.y + 2f, transform.position.z);
        }
        else
        {
            transform.position = new Vector3(transform.position.x, _originHeight, transform.position.z);
        }
    }
}
