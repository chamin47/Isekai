using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowCamera : MonoBehaviour
{
    [SerializeField] private float _flowSpeed = 0.1f;
    bool canMove = false;
    void Update()
    {
        if(canMove)
            this.transform.position += new Vector3(Time.deltaTime * _flowSpeed, 0, 0);
    }

    public void StartFlow()
    {
        canMove = true;
    }

    public void StopFlow()
    {
        Debug.Log("FlowCamera StopFlow");
        canMove = false;
    }

    public void SetFollowSpeed(float _speed)
    {
        _flowSpeed = _speed;
    }
}
