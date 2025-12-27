using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopCollider : MonoBehaviour
{
    public event Action OnCameraStopped;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("MainCamera"))
        {
            CameraFollowTarget cameraFollow = Camera.main.GetComponent<CameraFollowTarget>();
            if (cameraFollow != null)
            {
                cameraFollow.CanFollow = false;
            }

            FlowCamera cameraFlow = Camera.main.GetComponent<FlowCamera>();
            if (cameraFlow != null)
            {
                cameraFlow.StopFlow();
            }
            OnCameraStopped?.Invoke();
        }
    }
}
