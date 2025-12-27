using UnityEngine;

[RequireComponent(typeof(OutlineSelectSprite))]
public class TvObject : MonoBehaviour
{
	[SerializeField] private TvBubbleRunner _runner;

	private OutlineSelectSprite _outlineSelectSprite;

	private void Awake()
	{
		_outlineSelectSprite = GetComponent<OutlineSelectSprite>();
		_outlineSelectSprite.OnSelected += OnPointerClick;
	}

	public void OnPointerClick(int index)
	{
		_runner.Play("1001001");
	}

	private void OnDestroy()
	{
		_outlineSelectSprite.OnSelected -= OnPointerClick;
	}
}
