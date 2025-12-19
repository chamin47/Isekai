using Febucci.UI.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TvBubbleRunner : MonoBehaviour
{
	private DialogueDatabaseRuntime _database;

	private Coroutine _runCo;

	[SerializeField] private List<OutlineSelectSprite> _outlines = new();

	private void Awake()
	{		
		_database = gameObject.AddComponent<DialogueDatabaseRuntime>();

		_database.LoadAllFromResources();
	}

	public void Play(string startId)
	{
		if (_runCo != null)
			StopCoroutine(_runCo);

		_runCo = StartCoroutine(CoPlay(startId));
	}

	private IEnumerator CoPlay(string id)
	{
		SetOutlineEnabled(false);

		var _bubblePrefab = Resources.Load<UI_TvBubble>("Prefabs/UI/SubItem/UI_TvBubble");

		var bubble = Instantiate(_bubblePrefab, transform);

		while (!string.IsNullOrWhiteSpace(id) || id != "null")
		{
			if (!_database.TryGet(id, out var row))
				break;

			if (row.eventName == "ShowText")
			{
				yield return bubble.Show(row.script);
			}

			id = row.nextID;
		}

		bubble.FadeOutAndDestroy();

		while (bubble != null)
			yield return null;

		SetOutlineEnabled(true);
	}

	private void SetOutlineEnabled(bool enabled)
	{
		foreach (OutlineSelectSprite sprite in _outlines)
		{
			sprite.enabled = enabled;
		}
	}
}
