using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 각종 세계에 맞는 로딩 UI를 담당
/// 추후 더 많은 확장이 필요할 경우 추상클래스로 추상화 후 상속된 Loading화면 보여주기
/// </summary>
public class UI_Loading : UI_Scene
{
    [SerializeField] private float _fadeTime = 3f;
    public float FadeTime => _fadeTime;
    [SerializeField] private float _waitTimeAfterFade = 1f;
    [SerializeField] private float _waitTimeBeforeFade = 1f;

    [SerializeField] private Image _progressBar;
    [SerializeField] private Image _worldImage;
    [SerializeField] private Image _fadeImage;
    public Image FadeImage => _fadeImage;
    [SerializeField] private Image _glitchPanel;
    [SerializeField] private Image _vinterSubImage;

    [SerializeField] private TMP_Text _tipText;
    [SerializeField] private TMP_Text _worldText;
    [SerializeField] private TMP_Text _warningText;
    [SerializeField] private TMP_Text _vinterWorldText;

    private LoadingSceneData _loadingSceneData;

    public override void Init()
    {
        base.Init();

        StartCoroutine(SettingUI());
    }

    private IEnumerator SettingUI()
    {
        _tipText.text = string.Empty;
        _worldText.text = string.Empty;
        _progressBar.fillAmount = 0f;

        _loadingSceneData = Managers.DB.GetLoadingSceneData(Managers.World.CurrentWorldType);
        _worldImage.sprite = _loadingSceneData.backgroundImage;

        yield return StartCoroutine(StartLoadingSequence());

        Managers.Scene.LoadScene(Scene.GameScene);
    }

    /// <summary>
    /// 현재 게이지에따라 사운드의 유무가 갈라지는데 확장성을 생각하면 추후 하위 클래스에서 구현 요망
    /// </summary>
    
    public IEnumerator FillProgressBar(float target, float duration)
    {
        yield return _progressBar.CoFillImage(target, duration);
        if (target == 1f)
        {
            Managers.Sound.Play("loading_100", Sound.Effect);
        }
    }

    public void SetLoadingTexts()
    {
        _tipText.text = _loadingSceneData.tip;
        _worldText.text = $"[{_loadingSceneData.name}]";
    }

    private IEnumerator StartLoadingSequence()
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

        List<Sprite> sprites = _loadingSceneData.subImages;
        int maxIndex = sprites.Count;
        int currentIndex = 0;
        while (true)
        {
            yield return WaitForSecondsCache.Get(0.5f);
            currentIndex = (currentIndex + 1) % sprites.Count;
            _vinterSubImage.sprite = sprites[currentIndex];
        }
    }

    private IEnumerator DefaultLoadingSequence()
    {
        SetLoadingTexts();
        yield return FillProgressBar(0.8f, 3f);
        yield return FillProgressBar(1f, 3f);
        yield return _fadeImage.CoFadeOut(_fadeTime, 1f);
    }

    private IEnumerator PelmanusLoadingSequence()
    {
        _glitchPanel.gameObject.SetActive(true);
        _worldText.text = $"[{_loadingSceneData.name.GetNRandomMaskedText(3)}]";
        _tipText.text = _loadingSceneData.tip.GetRandomMaskedText();

        yield return _fadeImage.CoFadeOut(_fadeTime);
        yield return WaitForSecondsCache.Get(4f);

        _glitchPanel.gameObject.SetActive(false);
        _fadeImage.color = new Color(0, 0, 0, 1);
        yield return WaitForSecondsCache.Get(2f);

        yield return _warningText.CoTypingEffect(_loadingSceneData.tip, 0.5f);
        yield return WaitForSecondsCache.Get(2f);
    }

    [SerializeField] private TextMeshProUGUI _ganggrilText;
    private IEnumerator GangrilLoadingSequence()
    {
        //        게이지 다 찰때까지 TIP 하단에 글씨 안 나옴(TIP 글씨만 존재)
        //        게이지 다 찬 후에 ‘지금은 0년 0월 0일입니다.’ 출력
        //        2초 유지 후, 글리치로 화면 트렌지션

        _tipText.text = _loadingSceneData.tip;
        _tipText.gameObject.SetActive(false);
        Managers.Sound.Play("system_effect", Sound.Effect);
        yield return FillProgressBar(0.99f, 2f);
        yield return WaitForSecondsCache.Get(1f);
        _tipText.gameObject.SetActive(true);
        //yield return StartCoroutine(_tipText.CoTypingEffect(_loadingSceneData.tip, 0.3f));
        yield return WaitForSecondsCache.Get(3f);
        Color fadeColor = _fadeImage.color;
        fadeColor.a = 1f;
        _fadeImage.color = fadeColor;

        Managers.Sound.Play("system_2", Sound.Effect);
        yield return _ganggrilText.CoFadeOut(2f);
        yield return WaitForSecondsCache.Get(4f);
    }

   
}