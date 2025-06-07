using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UI_PelmanusNoticePopup : UI_NoticePopup
{
    [SerializeField] private TMP_Text _warningText;
    [SerializeField] private Sprite _changeSprite;
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Toggle _toggle;
    // 이미지 pixcel에 맞춘 고정된 값
    private const float X_OFFSET = 70f;
    private const float Y_OFFSET = 50f;

    public override void Init()
    {
        base.Init();        

        // 3번째 팝업 부터는 자동으로 무한 생성된다.
        if(_popupIndex >= 3)
        {
            _canHandle = false;
        }
    }

    protected override void ProcessWorldInteraction()
    {
        if (_popupIndex == 2)
        {
            _libraryScene = Managers.Scene.CurrentScene as LibraryScene;
            MakeInfinityPopup(_position);
        }
        else
        {
            UI_PelmanusNoticePopup popup = Managers.UI.ShowPopupUI<UI_PelmanusNoticePopup>();
            popup.Init(_popupIndex + 1, false, _position + new Vector2(X_OFFSET, -Y_OFFSET));
            _canHandle = false;
            ChangeBackground();
        }
    }

    public void ChangeBackground()
    {
        _backgroundImage.sprite = _changeSprite;
        SetActiveFalse();
    }

    public void ToggleDisable()
    {
        _toggle.isOn = true;
        _toggle.interactable = false;
    }

    

    private float minSpawnTime = 0.1f; // 최소 스폰 속도
    private float startSpawnTime = 1.0f; // 시작 속도
    private float acceleration = 0.8f; // 가속도 (1보다 작으면 점점 빨라짐)
    private float popupWidth, popupHeight;

    private float currentSpawnTime;
    private float spawnX, spawnY;
    private float screenWidth, screenHeight;
    private float leftTopX, leftTopY;

    private float _xOffset = 70;
    private float _yOffset = 50;

    private UI_PelmanusNoticePopup _lastPopup;

    private void MakeInfinityPopup(Vector3 startPosition)
    {
        // 50 -50 씩 아래로 팝업을 무한으로 생성하는데 스크린 범위를 넘어가면 왼쪽 위부터 다시 오른쪽 아래로 생성한다
        // 처음에는 천천히 생성했다가 점점 빠르게 생성되고 다음에는 일정한 속도로 계속 생성한다
        screenWidth = Screen.width;
        screenHeight = Screen.height;

        spawnX = startPosition.x;
        spawnY = startPosition.y;

        currentSpawnTime = startSpawnTime;

        popupWidth = _backgroundParent.GetComponent<RectTransform>().rect.width;
        popupHeight = _backgroundParent.GetComponent<RectTransform>().rect.height;

        GetFirstPosition();

        StartCoroutine(InfinityPopupCoroutine());
    }

    /// <summary>
    /// 초기 위치 초기화
    /// </summary>
    private void GetFirstPosition()
    {
        leftTopX = -popupWidth / 2;
        leftTopY = popupHeight / 2;

        while (leftTopX >= -(screenWidth / 2) && leftTopY <= screenHeight / 2)
        {
            leftTopX -= _xOffset;
            leftTopY += _yOffset;
        }

        leftTopX += _xOffset;
        leftTopY -= _yOffset;
        leftTopX += popupWidth / 2;
        leftTopY -= popupHeight / 2;
    }

    private IEnumerator InfinityPopupCoroutine()
    {
        Managers.Sound.StopBGM();
        int spawnCount = 0;
        int maxSpawnCount = 20;

        while (true)
        {
            _libraryScene.StopColorConversion();
            _libraryScene.ColorConversion(Mathf.Max(currentSpawnTime * 0.5f, 0.15f));
            Managers.Sound.Play("window_screen", Sound.Effect);
            SpawnPopup();

            yield return new WaitForSeconds(currentSpawnTime);

            // 가속 적용 (최소 속도보다 크면 계속 감소)
            if (currentSpawnTime > minSpawnTime)
            {
                currentSpawnTime *= acceleration;
                if (currentSpawnTime < minSpawnTime)
                    currentSpawnTime = minSpawnTime;
            }

            spawnCount++;
            if (spawnCount >= maxSpawnCount)
            {
                break;
            }
        }

        StartCoroutine(_lastPopup.BlackOutAndSetText());
    }

    private IEnumerator BlackOutAndSetText()
    {
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.color = new Color(0, 0, 0, 1);
        yield return WaitForSecondsCache.Get(2f);

        yield return StartCoroutine(_warningText.CoTypingEffectPerChar("거기서 당장 나", 0.5f, true, "getout_short",true));
        yield return StartCoroutine(_warningText.CoTypingEffectPerChar("와!", 0.5f, true, "getout_long", true, false));
        yield return WaitForSecondsCache.Get(2f);

        Managers.Scene.LoadScene(Scene.GameScene);
    }

    /// <summary>
    /// 팝업 생성
    /// 특정 위치에 생성
    /// </summary>
    private void SpawnPopup()
    {
        Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
        UI_PelmanusNoticePopup popup = Managers.UI.ShowPopupUI<UI_PelmanusNoticePopup>();
        popup.ToggleDisable();
        // 다음 위치 계산
        spawnX += _xOffset;
        spawnY -= _yOffset;
        if (spawnX + (popupWidth / 2) > screenWidth / 2 || spawnY + -(popupHeight / 2) > screenHeight / 2)
        {
            _lastPopup.ChangeSprite();

            _lastPopup = null;
            spawnX = leftTopX;
            spawnY = leftTopY;
        }

        popup.Init(_popupIndex + 1, false, new Vector3(spawnX, spawnY));

        if (_lastPopup != null)
        {
            _lastPopup.ChangeBackground();
        }

        _lastPopup = popup;
    }

    private void ChangeSprite()
    {
        _checkToggle.onValueChanged.RemoveAllListeners();
        _checkToggle.isOn = true;
    }
}