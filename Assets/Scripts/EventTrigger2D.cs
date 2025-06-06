using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTrigger2D : MonoBehaviour
{
	[SerializeField] private DialogUnit _dialogUnitA;
	[SerializeField] private DialogUnit _dialogUnitB;
	[SerializeField] private UI_BubbleTest _bubbleA;
	[SerializeField] private UI_BubbleTest _bubbleB;

	private bool _triggered = false;

	private void Update()
	{
		if (_triggered)
			return;

		if (IsVisibleFromCamera(Camera.main))
		{
			_triggered = true;
			TriggerEvent();
		}
	}

	private void TriggerEvent()
	{
		var scene = FindObjectOfType<DongMinTestScene>();

		if (this.gameObject.name == "Page03Trigger")
		{
			_bubbleA.SetDialogUnit(_dialogUnitA);
			_bubbleB.SetDialogUnit(_dialogUnitB);
			scene.HandlePageTrigger(_dialogUnitA, _dialogUnitB);
		}
		else
		{
			_bubbleA.SetDialogUnit(_dialogUnitA);
			scene.HandlePageTrigger(_dialogUnitA);
		}
	}

	private bool IsVisibleFromCamera(Camera cam)
	{
		Vector3 viewPos = cam.WorldToViewportPoint(transform.position);
		return viewPos.x >= 0f && viewPos.x <= 1f &&
			   viewPos.y >= 0f && viewPos.y <= 1f &&
			   viewPos.z >= 0f;
	}

	//private void OnTriggerEnter2D(Collider2D other)
	//{
	//	if (other.CompareTag("Player"))
	//	{
	//		GetComponent<Collider2D>().enabled = false;

	//		var scene = FindObjectOfType<DongMinTestScene>();

	//		if (this.gameObject.name == "Page03Trigger")
	//		{
	//			_bubbleA.SetDialogUnit(_dialogUnitA);
	//			_bubbleB.SetDialogUnit(_dialogUnitB);
	//			scene.HandlePageTrigger(_dialogUnitA, _dialogUnitB);
	//		}
	//		else
	//		{
	//			_bubbleA.SetDialogUnit(_dialogUnitA);
	//			scene.HandlePageTrigger(_dialogUnitA);
	//		}
	//	}
	//}
}
