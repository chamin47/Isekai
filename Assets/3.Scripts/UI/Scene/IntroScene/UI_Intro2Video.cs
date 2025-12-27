using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class UI_Intro2Video : UI_Scene
{
	[SerializeField] private RawImage _screen;            // RenderTexture 표시용
	[SerializeField] private VideoPlayer _vp;             // Render Mode: RenderTexture
	[SerializeField] private string _fileName = "서코_인트로.mp4"; // StreamingAssets
	[SerializeField] private Image _fade;                 // 화면 페이드
	[SerializeField] private Button _skipButton;

	private Coroutine _playFlowCo;
	private bool _isSkipped = false;

    #region 초기화&구독
	private void Awake()
	{
		if (_screen != null)
			_screen.enabled = false;

		_vp.playOnAwake = false;
		_vp.waitForFirstFrame = true;   
		_vp.isLooping = false;

		if (_skipButton != null)
			_skipButton.onClick.AddListener(Skip);
	}
    
	private void OnEnable()
	{
		_vp.loopPointReached += OnVideoEnd;
	}
	#endregion

	#region 구독해제
	private void OnDisable()
	{
		_vp.loopPointReached -= OnVideoEnd;
	}

	private void OnDestroy()
	{
		if (_skipButton != null)
			_skipButton.onClick.RemoveListener(Skip);
	}
	#endregion

	public override void Init()
	{
		base.Init();
		_playFlowCo = StartCoroutine(PlayFlow());
	}

	private IEnumerator PlayFlow()
	{
		_vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, _fileName);
		_vp.Prepare();

		while (!_vp.isPrepared)
			yield return null;

		if (_screen != null) 
			_screen.enabled = true;

		_vp.Play();
	}

	private void OnVideoEnd(VideoPlayer _)
	{
		if (_isSkipped)
			return;

		StartCoroutine(ExitToTitle());
	}

	private void Skip()
	{
		if (_isSkipped)
			return;

		_isSkipped = true;

		_vp.loopPointReached -= OnVideoEnd;

		if (_vp.isPlaying)
			_vp.Stop();

		if (_playFlowCo != null)
			StopCoroutine(_playFlowCo);

		StartCoroutine(ExitToTitle());
	}

	private IEnumerator ExitToTitle()
	{
		if (_fade != null) 
			yield return _fade.CoFadeOut(2f, 1f);

		Managers.UI.ShowSceneUI<UI_TitleScene>();
		gameObject.SetActive(false);
	}
}
