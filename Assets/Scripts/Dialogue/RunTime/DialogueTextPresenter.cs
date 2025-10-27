using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 대사 표시 담당 프레젠터.
/// 말풍선 생성·표시, 스택형 말풍선의 누적 배치/해제, 안전한 앵커 해석을 수행한다.
/// </summary>
public class DialogueTextPresenter : MonoBehaviour, ITextPresenter
{
	public float _charSpeed = 0.03f;

	public IActorDirector actor; // Runner에서 주입

	const float StackSpacingPx = 150f;

	readonly Dictionary<Transform, List<UI_DialogueBalloon>> _stacked = new();

	public IEnumerator ShowText(string speaker, string text, string animName)
	{
		if (actor != null && !string.IsNullOrEmpty(animName))
			StartCoroutine(actor.PlayAnim(speaker, animName));

		var anchor = ResolveAnchorSafe(speaker);

		var balloon = Managers.UI.MakeWorldSpaceUI<UI_DialogueBalloon>();
		balloon.Init(anchor);
		yield return balloon.CoPresent(text ?? "", _charSpeed);
	}

	public IEnumerator ShowTextStacked(string speaker, string text, string animName)
	{
		if (actor != null && !string.IsNullOrEmpty(animName))
			StartCoroutine(actor.PlayAnim(speaker, animName));

		var anchor = ResolveAnchorSafe(speaker);

		if (!_stacked.TryGetValue(anchor, out var list))
		{
			list = new List<UI_DialogueBalloon>();
			_stacked[anchor] = list;
		}

		for (int i = 0; i < list.Count; i++)
			if (list[i]) list[i].AddStackOffset(StackSpacingPx);

		var balloon = Managers.UI.MakeWorldSpaceUI<UI_DialogueBalloon>();
		balloon.Init(anchor);

		yield return balloon.CoPresentStacked(text ?? "", _charSpeed);

		list.Add(balloon);
	}

	public IEnumerator ClearAllStacked(float fadeOut = 0.12f)
	{
		foreach (var kv in _stacked)
		{
			var list = kv.Value;
			for (int i = 0; i < list.Count; i++)
				if (list[i]) yield return list[i].FadeOutAndDestroy(fadeOut);
		}
		_stacked.Clear();
	}

	private Transform ResolveAnchorSafe(string speaker)
	{		
		// acotr director가 리졸버도 겸한다면 그걸 사용
		if (actor is ISpeakerAnchorResolver a2)
		{
			var transform = a2.ResolveAnchor(speaker);
			return transform;
		}

		return Camera.main ? Camera.main.transform : transform;
	}
}
