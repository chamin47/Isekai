using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCameraController : MonoBehaviour
{
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private CameraColliderBounds _cameraColliderBounds;
    [SerializeField] float _moveSpeed = 0.5f;
    [SerializeField] private PlayerController _playerController;
    public void EnableCameraUpdate()
    {
        _cameraController.EnableCameraUpdate();
    }

    public void DisableCameraUpdate()
    {
        _cameraController.DisableCameraUpdate();
    }

    public void MoveTo(Vector3 position)
    {
        transform.position = position;
    }
    public void CoMoveTo(Vector3 position)
    {
        StartCoroutine(CoMove(position));
    }

    public void ChangeBoxBounds(BoxCollider2D collider)
    {
        _cameraColliderBounds.boundingShape = collider;
    }

    private IEnumerator CoMove(Vector3 position)
    {
        DisableCameraUpdate();
        _playerController.canMove = false;
        yield return null;
        Vector3 startPos = transform.position;
        Vector3 targetPos = position;
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime * _moveSpeed;
            transform.position = Vector3.Lerp(startPos, targetPos, Mathf.SmoothStep(0f, 1f, elapsed));
            yield return null;
        }
        transform.position = position;
        yield return null;
        _playerController.canMove = true;
        EnableCameraUpdate();
    }
}
