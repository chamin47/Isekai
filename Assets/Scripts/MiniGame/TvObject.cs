using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(OutlineSelectSprite))]
public class TvObject : MonoBehaviour, IPointerClickHandler
{
	[SerializeField] private TvBubbleRunner _runner;

	public void OnPointerClick(PointerEventData eventData)
	{
		if (TvBubbleRunner.InputLocked)
			return;

		_runner.Play("1001001");
	}
}
