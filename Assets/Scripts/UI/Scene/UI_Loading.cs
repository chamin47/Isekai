using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_Scene
{
    [SerializeField] private float _fadeTime = 3f;
    [SerializeField] private float _waitTimeAfterFade = 1f;
    [SerializeField] private float _waitTimeBeforeFade = 1f;

    [SerializeField] private Image _progressBar;
    [SerializeField] private Image _worldImage;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private Image _glitchPanel;
    [SerializeField] private Image _vinterSubImage;

    [SerializeField] private TMP_Text _tipText;
    [SerializeField] private TMP_Text _worldText;
    [SerializeField] private TMP_Text _warningText;
    [SerializeField] private TMP_Text _vinterWorldText;

    private LoadingSceneData _loadingSceneData;

    private IEnumerator Start()
    {
        _tipText.text = string.Empty;
        _worldText.text = string.Empty;
        _progressBar.fillAmount = 0f;

        _loadingSceneData = Managers.DB.GetLoadingSceneData(Managers.World.CurrentWorldType);
        _worldImage.sprite = _loadingSceneData.backgroundImage;

        yield return StartCoroutine(GetLoadingSequence());

        Managers.Scene.LoadScene(Scene.GameScene);
    }

    private IEnumerator GetLoadingSequence()
    {
        switch (_loadingSceneData.worldType)
        {
            case WorldType.Gang:
                yield return GangrilLoadingSequence();
                break;
            case WorldType.Pelmanus:
                yield return PelmanusLoadingSequence();
                break;
            case WorldType.Vinter:
                StartCoroutine(ChangeSpriteRoutine());
                yield return DefaultLoadingSequence();
                break;
            default:
                yield return DefaultLoadingSequence();
                break;
        }
    }

    private IEnumerator ChangeSpriteRoutine()
    {
        _vinterSubImage.gameObject.SetActive(true);
        _vinterWorldText.gameObject.SetActive(true);
        Sprite[] sprites = _loadingSceneData.subImages.ToArray();
        int maxIndex = sprites.Length;
        int currentIndex = 0;
        while (true)
        {
            yield return WaitForSecondsCache.Get(0.5f);
            currentIndex = (currentIndex + 1) % sprites.Length;
            _vinterSubImage.sprite = sprites[currentIndex];
        }
    }

    private IEnumerator DefaultLoadingSequence()
    {
        SetLoadingTexts();
        yield return FillProgressBar(0.8f, 3f);
        yield return FillProgressBar(1f, 3f);
        yield return StartCoroutine(_fadeImage.CoFadeOut(_fadeTime, 1f));
    }

    private IEnumerator PelmanusLoadingSequence()
    {
        _glitchPanel.gameObject.SetActive(true);
        _worldText.text = $"[{_loadingSceneData.name.GetNRandomMaskedText(3)}]";
        _tipText.text = _loadingSceneData.tip.GetRandomMaskedText();

        yield return StartCoroutine(_fadeImage.CoFadeOut(_fadeTime));
        yield return new WaitForSeconds(4f);

        _glitchPanel.gameObject.SetActive(false);
        _fadeImage.color = new Color(0, 0, 0, 1);
        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(_warningText.CoTypingEffect(_loadingSceneData.tip, 0.5f));
        yield return new WaitForSeconds(2f);
    }

    private IEnumerator GangrilLoadingSequence()
    {
        //        게이지 다 찰때까지 TIP 하단에 글씨 안 나옴(TIP 글씨만 존재)
        //        게이지 다 찬 후에 ‘지금은 0년 0월 0일입니다.’ 출력
        //        2초 유지 후, 글리치로 화면 트렌지션

        _tipText.gameObject.SetActive(false);
        yield return FillProgressBar(1f, 3f);
        yield return WaitForSecondsCache.Get(2f);
        _tipText.gameObject.SetActive(true);
        yield return StartCoroutine(_tipText.CoTypingEffect(_loadingSceneData.tip, 0.3f));
        yield return WaitForSecondsCache.Get(2f);
    }

    private IEnumerator FillProgressBar(float target, float duration)
    {
        yield return StartCoroutine(_progressBar.CoFillImage(target, duration));
        if(target == 1f)
        {
            Managers.Sound.Play("loading_100", Sound.Effect);
        }
    }

    private void SetLoadingTexts()
    {
        _tipText.text = _loadingSceneData.tip;
        _worldText.text = $"[{_loadingSceneData.name}]";
    }
}