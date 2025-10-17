using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class UI_EndingScene : UI_Scene
{
	[SerializeField] private TMP_Text _newsText;       // ���� ��� �ؽ�Ʈ
	[SerializeField] private TMP_Text _finalText;      // ����ȭ�� �ؽ�Ʈ
    [SerializeField] private GameObject _bubbleImage;  // ��ǳ�� �̹���
    [SerializeField] private Image _fadeImage;

    [SerializeField] private VideoPlayer _endingVideoPlayer; // ������ ���� �۸��� ȿ��

    private EndingSceneData _sceneData;
	public override void Init()
	{
		base.Init();
        Canvas canvas = GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceCamera;
        canvas.worldCamera = Camera.main;

        _sceneData = Managers.DB.GetEndingSceneData();

        _newsText.text = "";       // ���� �ؽ�Ʈ �ʱ�ȭ
		_finalText.text = "";      // ���� �ؽ�Ʈ �ʱ�ȭ
        _bubbleImage.SetActive(false); // ��ǳ�� �̹��� ��Ȱ��ȭ

        StartCoroutine(PlayEndingSequence());
	}

    [SerializeField] WebGLVideoPlayer _webGLVideoPlayer; // WebGL���� ���� ����� ���� ������Ʈ
    private IEnumerator PlayEndingSequence()
	{
		yield return new WaitForSeconds(0.5f);
        _bubbleImage.SetActive(true); // ��ǳ�� �̹��� Ȱ��ȭ
        
        _newsText.text = _sceneData.newsDialog[0];
        //ResizeBubbleImage(_sceneData.newsDialog[0]); // ��ǳ�� �̹��� ũ�� ����

        yield return WaitForSecondsCache.Get(1.5f); // 1�� ���

        System.Action cache = () => { Managers.Sound.Play("keyboard_oneshot2", Sound.Effect); };
        // ���� �ؽ�Ʈ ���
        for (int i = 1; i < _sceneData.newsDialog.Count; i++)
        {
            //ResizeBubbleImage(_sceneData.newsDialog[i]);
            yield return _newsText.CoTypeEffectWithRichText(_sceneData.newsDialog[i], 0.1f, cache);
        }

		Managers.Sound.StopSubEffect();

		// ȭ�� fadeOut
		_fadeImage.gameObject.SetActive(true);

        Coroutine c1 = StartCoroutine(Managers.Sound.FadeOutSubEffect(2f));
        Coroutine c2 = StartCoroutine(_fadeImage.CoFadeOut(1f, 0f, 1f));
        yield return c1; yield return c2;
        
        //yield return WaitForSecondsCache.Get(1f); // 1�� ���

        // ���� ȭ����� �ؽ�Ʈ ���
        foreach (var finalDialogue in _sceneData.finalDialog)
        {
            yield return _finalText.CoTypeEffectWithRichText(finalDialogue, 0.1f, cache);
        }

        _finalText.text = "";

        yield return WaitForSecondsCache.Get(2f);
        _endingVideoPlayer.gameObject.SetActive(true); // Ending Video Player Ȱ��ȭ
        _webGLVideoPlayer.PlayOverlayVideo("glitch.mp4"); // WebGL���� ���� ���
        Managers.Sound.Play("titleName", Sound.Effect);
        yield return WaitForSecondsCache.Get(7f); // 0.5�� ���

        // ���� ȭ������ ��ȯ
        Managers.Scene.LoadScene(Scene.TitleScene); // Main Title Scene���� ��ȯ
    }
}
