using System.Collections;
using UnityEngine;

public class DialogueTextPresenter : MonoBehaviour, ITextPresenter
{
	[Header("Resolver")]
	[SerializeField] MonoBehaviour anchorResolverBehaviour; // 드래그로 주입
	ISpeakerAnchorResolver _resolver;

	public float charSpeed = 0.03f;

	public IActorDirector actor; // Runner에서 주입

	void Awake() => _resolver = anchorResolverBehaviour as ISpeakerAnchorResolver;

	public IEnumerator ShowText(string speaker, string text, string animName)
	{
		if (actor != null && !string.IsNullOrEmpty(animName))
			actor.SetPose(speaker, animName);

		var anchor = ResolveAnchorSafe(speaker);

		var balloon = Managers.UI.MakeWorldSpaceUI<UI_DialogueBalloon>();
		balloon.Init(anchor);
		yield return balloon.CoPresent(text ?? "", charSpeed);
	}

	Transform ResolveAnchorSafe(string speaker)
	{
		// 우선 순위 1: 외부에서 주입된 리졸버
		if (_resolver != null)
		{
			var t = _resolver.ResolveAnchor(speaker);
			if (t) return t;
		}
		// acotr director가 리졸버도 겸한다면 그걸 사용
		if (actor is ISpeakerAnchorResolver a2)
		{
			var t = a2.ResolveAnchor(speaker);
			if (t) return t;
		}
		// 최후: 카메라 중심 근처 등, 프로젝트 규약에 맞는 기본 앵커
		return Camera.main ? Camera.main.transform : transform;
	}
}
