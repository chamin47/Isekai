using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 1. ȭ�� FadeIn
/// 2. BlamePopup ����
/// </summary>
public class UI_IntroScene : UI_Scene
{
	[SerializeField] private Image _fadeImage;

	public override void Init()
	{
		base.Init();

		StartCoroutine(StartIntroSequence());
	}

	private IEnumerator StartIntroSequence()
	{
        yield return StartCoroutine(_fadeImage.CoFadeIn(2f, 3f, 0f));
        Managers.UI.ShowPopupUI<UI_BlamePopup>();
    }
}
