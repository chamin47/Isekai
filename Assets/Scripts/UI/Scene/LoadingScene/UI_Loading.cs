using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� ���迡 �´� �ε� UI�� ���
/// ���� �� ���� Ȯ���� �ʿ��� ��� �߻�Ŭ������ �߻�ȭ �� ��ӵ� Loadingȭ�� �����ֱ�
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
    /// ���� ������������ ������ ������ �������µ� Ȯ�强�� �����ϸ� ���� ���� Ŭ�������� ���� ���
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
        //        ������ �� �������� TIP �ϴܿ� �۾� �� ����(TIP �۾��� ����)
        //        ������ �� �� �Ŀ� �������� 0�� 0�� 0���Դϴ�.�� ���
        //        2�� ���� ��, �۸�ġ�� ȭ�� Ʈ������

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