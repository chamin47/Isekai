using System.Collections;
using UnityEngine;

public class DialogueTextPresenter : MonoBehaviour, ITextPresenter
{
	[Header("Resolver")]
	[SerializeField] MonoBehaviour anchorResolverBehaviour; // �巡�׷� ����
	ISpeakerAnchorResolver _resolver;

	public float charSpeed = 0.03f;

	public IActorDirector actor; // Runner���� ����

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
		// �켱 ���� 1: �ܺο��� ���Ե� ������
		if (_resolver != null)
		{
			var t = _resolver.ResolveAnchor(speaker);
			if (t) return t;
		}
		// acotr director�� �������� ���Ѵٸ� �װ� ���
		if (actor is ISpeakerAnchorResolver a2)
		{
			var t = a2.ResolveAnchor(speaker);
			if (t) return t;
		}
		// ����: ī�޶� �߽� ��ó ��, ������Ʈ �Ծ࿡ �´� �⺻ ��Ŀ
		return Camera.main ? Camera.main.transform : transform;
	}
}
