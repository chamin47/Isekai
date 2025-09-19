using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScene : BaseScene
{
	private Animator _mainCameraAni;

    [SerializeField] private GameObject _noiseImage;
    [SerializeField] private GameObject _newsImage;

    // 2�� ����� ī�޶� ����
    // ī�޶� ���� �Ϸ��� 1�� �� ���� �̹��� Ȱ��ȭ
    // �����̹��� Ȱ��ȭ 1�� �� ī�޶� �� �ƿ�
    // ī�޶� �� �ƿ� 1�� �� EndingSceneUI Ȱ��ȭ
    private IEnumerator EndingSceneSequence()
    {
        Managers.Sound.Play("tv_noise", Sound.SubEffect);
        yield return WaitForSecondsCache.Get(2f);
        _mainCameraAni.SetTrigger("CloseIn");
        yield return WaitForSecondsCache.Get(2f);

        Managers.Sound.StopSubEffect();

        Managers.Sound.PlaySubEffect("tv_noise", 0.2f);

        _noiseImage.GetComponent<Animator>().enabled = false;        
        _newsImage.SetActive(true);
        yield return WaitForSecondsCache.Get(2f);
        _mainCameraAni.SetTrigger("CloseOut");
        yield return WaitForSecondsCache.Get(2f);
        
        Managers.UI.ShowSceneUI<UI_EndingScene>();
    }

    protected override void Init()
	{
		
		SceneType = Scene.EndingScene;

        _noiseImage.SetActive(true);
        _newsImage.SetActive(false);

        _mainCameraAni = Camera.main.GetComponent<Animator>();

        // ���� ������ ����
        Managers.Game.IsShowEnding = true;

        StartCoroutine(EndingSceneSequence());
    }

	public override void Clear()
	{

	}
}
