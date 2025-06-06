using UnityEngine;

public class CameraFollowTarget: MonoBehaviour
{
	[SerializeField] private Transform _target;
    public bool CanFollow = false;
	private Vector3 _offset;
    [SerializeField] private float _smooth = 5f;
    private void LateUpdate()
	{
		if(CanFollow && _target != null)
        {
            Vector3 targetPosition = new Vector3( _target.position.x, transform.position.y, transform.position.z) + _offset;
            if(Mathf.Abs(targetPosition.x - transform.position.x) > 0.25f)
            {
                targetPosition = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * _smooth);
            }

            transform.position = targetPosition;


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
