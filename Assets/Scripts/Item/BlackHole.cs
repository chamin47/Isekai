using System;
using System.Collections.Generic;
using UnityEngine;

public enum BlackHoleState
{
    Disabled,
    Chasing,
    Fading
}


public class BlackHole : MonoBehaviour
{
    [SerializeField] private float _chaseSpeed = 5f;
    private Transform _target;


    public void Init(Transform target)
    {
        _target = target;
    }


    public void StartChase()
    {

    }

    public void StopChase()
    {

    }
}
