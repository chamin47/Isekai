using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UI_Loading : MonoBehaviour
{
    [Tooltip("FadeTest")]
    [SerializeField] private float _fadeTime = 2f;
    [SerializeField] private float _waitTimeAfterFade = 1f;
    [SerializeField] private float _waitTimeBeforeFade = 1.0f;

    [SerializeField] private Image _progressBar;
    [SerializeField] private Image _worldImage;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private Image _glitchPanel;

    [SerializeField] private TMP_Text _tipText;
    [SerializeField] private TMP_Text _worldText;
    [SerializeField] private TMP_Text _warningText;

    private LoadingSceneData _loadingSceneData;

    private IEnumerator Start()
    {
        _worldText.text = "";
        _tipText.text = "";

        _loadingSceneData = Managers.DB.GetLoadingSceneData(Managers.World.CurrentWorldType);
        
       
        if(_loadingSceneData.worldType == WorldType.Gang)
        {
            yield return StartCoroutine(GangrilLoadingSequence());
        }
        else if (_loadingSceneData.worldType == WorldType.Pelmanus)
        {
            yield return StartCoroutine(PelmanusLoadingSequence());
        }
        else
        {
            yield return StartCoroutine(LoadingSequence());
        }

        Managers.Scene.LoadScene(Scene.GameScene);
    }

    private IEnumerator LoadingSequence()
    {
        _tipText.text = $"{_loadingSceneData.tip}";
        _worldText.text = $"[{_loadingSceneData.name}]";

        yield return StartCoroutine(_progressBar.CoFillImage(0.8f, 2));
        yield return StartCoroutine(_progressBar.CoFillImage(1f, 2));

        yield return StartCoroutine(_fadeImage.CoFadeOut(_fadeTime));
    }

    private IEnumerator PelmanusLoadingSequence()
    {
        _glitchPanel.gameObject.SetActive(true);
        _worldText.text = _loadingSceneData.name.GetNRandomMaskedText(3);
        _worldText.text = $"[{_worldText.text}]";
        _tipText.text = _loadingSceneData.tip.GetRandomMaskedText();

        yield return StartCoroutine(_fadeImage.CoFadeIn(_fadeTime));
        // �������� 0 �λ��¿��� ȭ�� ������� 4�ʰ� ���ӵȴ�
        // �̶� ���� �̸��� ���� �κ��� ������ũó���ϰ� �� �κе� ������ũ�� ����� �ش� 
        yield return new WaitForSeconds(4f);

        // 4�ʵ� ����� ������ ����ȭ���� �ȴ�
        // ����ȭ�鿡�� ��縦 ���������� ����ϰ� 2�ʰ� ����Ѵ�
        _glitchPanel.gameObject.SetActive(false);
        _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 1f);
        yield return new WaitForSeconds(2f);

        yield return StartCoroutine(_warningText.CoTypeingEffect(_loadingSceneData.tip, 0.5f));
        yield return new WaitForSeconds(2f);

      
    }

   

    private IEnumerator GangrilLoadingSequence()
    {
        // �ε��� 2�ʵ��� 50% ���
        yield return StartCoroutine(_progressBar.CoFillImage(0.5f, 3f));
        yield return new WaitForSeconds(1f);
        // �ؽ�Ʈ Ÿ���� ȿ��
        yield return StartCoroutine(_tipText.CoTypeingEffect(_loadingSceneData.tip, 0.3f));
        // �ε��� �����κ� 100%���� 1�� ���
        yield return StartCoroutine(_progressBar.CoFillImage(1f, 1f));
        // �� �ؽ�Ʈ 3�� �����̱�
        yield return StartCoroutine(_tipText.BlinkTipText(3, 0.2f));
        // ������ ȿ�� + ����ȯ
        yield return StartCoroutine(NoiseEffect());

    }

    private IEnumerator NoiseEffect()
    {
        yield return new WaitForSeconds(2f);

    }
}
