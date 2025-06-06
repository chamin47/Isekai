using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MainCamera"))
        {
            CameraFollowTarget cameraFollow = Camera.main.GetComponent<CameraFollowTarget>();
            if (cameraFollow != null)
            {
                cameraFollow.CanFollow = false;
            }
        }
    }
}
