using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScene : BaseScene
{
	private Animator _mainCameraAni;

    [SerializeField] private GameObject _noiseImage;
    [SerializeField] private GameObject _newsImage;

    // 2초 대기후 카메라 줌인
    // 카메라 줌인 완료후 1초 뒤 뉴스 이미지 활성화
    // 뉴스이미지 활성화 1초 후 카메라 줌 아웃
    // 카메라 줌 아웃 1초 후 EndingSceneUI 활성화
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
        Managers.Sound.StopSubEffect();
        yield return WaitForSecondsCache.Get(2f);
        Managers.UI.ShowSceneUI<UI_EndingScene>();
    }

    protected override void Init()
	{
		
		SceneType = Scene.EndingScene;

        _noiseImage.SetActive(true);
        _newsImage.SetActive(false);

        _mainCameraAni = Camera.main.GetComponent<Animator>();

        // 엔딩 데이터 저장
        Managers.Game.IsShowEnding = true;

        StartCoroutine(EndingSceneSequence());
    }

	public override void Clear()
	{

	}
}
