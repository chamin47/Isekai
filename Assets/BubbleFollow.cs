using UnityEngine;

public class BubbleFollow : MonoBehaviour
{
	[SerializeField] private Transform target;          
	[SerializeField] private Vector3 offset = new(0, 1.8f, 0);
	public Vector3 Offset { get => offset; set => offset = value; }

	public void SetTarget(Transform newTarget) => target = newTarget;

	void LateUpdate()
	{
		if (!target) return;

		transform.position = target.position + offset;
		transform.forward = Camera.main.transform.forward;   
	}
}
