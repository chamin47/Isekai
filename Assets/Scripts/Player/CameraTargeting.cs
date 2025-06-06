using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTargeting : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("CameraTargetMoving"))
        {
            CameraFollowTarget cameraFollow = Camera.main.GetComponent<CameraFollowTarget>();
            if (cameraFollow != null)
            {
                cameraFollow.CanFollow = true;
            }
        }
    }
}
