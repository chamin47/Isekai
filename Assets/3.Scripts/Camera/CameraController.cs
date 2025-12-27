using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IRuntimeCamera
{
    public bool UpdateCamera { get; set; }
}


public class CameraController : MonoBehaviour
{
    private List<IRuntimeCamera> _runtimeCameras = new List<IRuntimeCamera>();

    private void Awake()
    {
        var components = GetComponents<IRuntimeCamera>(); 
        foreach (var comp in components)
        {
            RegisterRuntimeCamera(comp);
        }
    }

    private void RegisterRuntimeCamera(IRuntimeCamera runtimeCamera)
    {
        _runtimeCameras.Add(runtimeCamera);   
    }

    private void SetUpdateCamera(bool update)
    {
        foreach (var runtimeCamera in _runtimeCameras)
        {
            runtimeCamera.UpdateCamera = update;
        }
    }

    public void EnableCameraUpdate()
    {
        SetUpdateCamera(true);
    }

    public void DisableCameraUpdate()
    {
        SetUpdateCamera(false);
    }
}
