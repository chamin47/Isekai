using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParelBackground : MonoBehaviour
{
    private CameraColliderBounds _mainCamera;
    [SerializeField] private PlayerController _playerController;

    [SerializeField] private GameObject[] _backgrounds;
    private Material[] _backgroundMaterials;
    [SerializeField][Range(0, 0.002f)] private float[] _backgroundSpeeds;

    private Vector2 _currentOffset = Vector2.zero;

    private void Awake()
    {
        _mainCamera = Camera.main.GetComponent<CameraColliderBounds>();

        int backgroundCount = _backgrounds.Length;

        _backgroundMaterials = new Material[backgroundCount];

        for(int i = 0; i < backgroundCount; i++)
        {
            _backgroundMaterials[i] = _backgrounds[i].GetComponent<Renderer>().material;
        }

        _playerController.OnPlayerMove += MoveBackgound;
    }

    private void MoveBackgound(Vector2 dir, float moveSpeed)
    {
        if (_mainCamera != null && !_mainCamera.IsCameraWithInBounds())
            return;

        for(int i = 0; i < _backgroundMaterials.Length; i++)
        {
            _currentOffset += -dir * _backgroundSpeeds[i] * Time.deltaTime * moveSpeed;
            _backgroundMaterials[i].SetTextureOffset("_MainTex", _currentOffset);
        }
    }


}
