using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_Intro2Video : UI_Scene
{
	[SerializeField] private RawImage _screen;            // RenderTexture 표시용
	[SerializeField] private VideoPlayer _vp;             // Render Mode: RenderTexture
	[SerializeField] private string _fileName = "인트로_이미지수정.mp4"; // StreamingAssets
	[SerializeField] private Image _fade;                 // 화면 페이드(검정)

	private void Awake()
	{
		if (_screen) _screen.enabled = false;

		_vp.playOnAwake = false;
		_vp.waitForFirstFrame = true;   
		_vp.isLooping = false;
	}

	public override void Init()
	{
		base.Init();
		StartCoroutine(PlayFlow());
	}

	private IEnumerator PlayFlow()
	{
		_vp.loopPointReached += OnVideoEnd;

		_vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, _fileName);
		_vp.Prepare();

		while (!_vp.isPrepared)
			yield return null;

		if (_screen) _screen.enabled = true;
		_vp.Play();
	}

	private void OnVideoEnd(VideoPlayer _)
	{
		StartCoroutine(ExitToTitle());
	}

	private IEnumerator ExitToTitle()
	{
		if (_fade) 
			yield return _fade.CoFadeOut(2f, 1f);
		Managers.UI.ShowSceneUI<UI_TitleScene>();
		gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		_vp.loopPointReached -= OnVideoEnd;
	}
}
