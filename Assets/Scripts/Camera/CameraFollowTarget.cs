using UnityEngine;
using static ClickTargetTable;

public class CameraFollowTarget: MonoBehaviour, IRuntimeCamera
{
	[SerializeField] private Transform _target;
    public bool CanFollow = false;
	private Vector3 _offset;
    [SerializeField] private float _smooth = 5f;

    [Tooltip("ī�޶� ������� �ӵ� (���� �������� ����)")]
    public float smoothTime = 0.3f;
    private Vector3 _velocity = Vector3.zero;

    public bool UpdateCamera { get; set; } = true;
    private void LateUpdate()
	{
        if(UpdateCamera == false)
            return;

        if (CanFollow && _target != null)
        {
            Vector3 targetPosition = new Vector3( _target.position.x, transform.position.y, transform.position.z) + _offset;
            //if(Mathf.Abs(targetPosition.x - transform.position.x) > 0.25f)
            //{
            //    targetPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _smooth);
            //}
            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref _velocity,
                smoothTime
            );
            //transform.position = targetPosition;
        }
    }

	public void StartFollow()
	{
        CanFollow = true;
    }

    public void StopFollow()
    {
        CanFollow = false;
    }

    public void Init(Transform target, Vector3 offset)
    {
        _target = target;
        _offset = offset;
    }
}
