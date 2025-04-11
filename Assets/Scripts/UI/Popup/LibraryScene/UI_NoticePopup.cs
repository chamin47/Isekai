using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public class UI_NoticePopup : UI_Popup
{
    [SerializeField] protected Toggle _checkToggle;
    [SerializeField] protected Image _fadeImage;
    [SerializeField] protected RectTransform _backgroundParent;
    [SerializeField] protected TMP_Text _noticeText;

    protected Canvas _canvas;
    protected LibraryScene _libraryScene;
    protected LibraryBook _book;

    protected int _popupIndex = 0;
    protected bool _canHandle = true;
    protected Vector2 _position;

    public override void Init()
    {
        base.Init();
        _libraryScene = Managers.Scene.CurrentScene as LibraryScene;


        _libraryScene.DisableBookSelect();
        

        _checkToggle.onValueChanged.AddListener(OnCheckToggleIsOn);

        // postprocessing ȿ���� ���� ī�޶� ����
        _canvas = GetComponent<Canvas>();
        _canvas.renderMode = RenderMode.ScreenSpaceCamera;
        _canvas.worldCamera = Camera.main;
    }

    public virtual void Init(LibraryBook book)
    {
        _book = book;
    }

    public virtual void Init(int index, bool canHandle, Vector2 position)
    {
        _canHandle = canHandle;
        _popupIndex = index;
        _position = position;
        _backgroundParent.anchoredPosition = position;
    }

    protected virtual void Update()
    {
        if (!_canHandle) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClosePopup();
        }
    }

    protected virtual void ClosePopup()
    {
        _libraryScene.EnableBookSelect();
        if (_book != null)
        {
            _book.SetCanClicked();
            _book.StartFingerBlink();
        }
        Managers.UI.ClosePopupUI(this);
    }

    public virtual void OnCheckToggleIsOn(bool isOn)
    {
        if (!isOn) return;

        // �� ���� Ÿ�Ժ� ó���� ���� Ŭ�������� ����
        ProcessWorldInteraction();
    }

    protected virtual void ProcessWorldInteraction()
    {
        _libraryScene.PlayEndTimeLine();
        Managers.UI.CloseAllPopupUI();
        Managers.Scene.LoadScene(Scene.GameScene);
    }

    public void SetActiveFalse()
    {
        _noticeText.gameObject.SetActive(false);
        _checkToggle.gameObject.SetActive(false);
    }
 //   private void GangrilSequence()
 //   {
 //       _libraryScene.PlayEndTimeLine();
 //       // 1. ȭ���� ���� �������
 //       // 2. �ȭ�鿡 ������ 1�ʵڿ� ���ο� Noticeâ�� �����ȴ�
 //       // 3. Noticeâ�� ũ��� 0.7, 1.0, 1.3 ������ �Ǿ������� 3������ Noticeâ�� �����ȴ�
 //       // 3��°�� üũǥ�ÿ��� ���׸� ���迡 �����ϰ� �ȴ�.
 //       Sequence sequence = DOTween.Sequence();
 //       sequence.AppendInterval(2f);
 //       sequence.AppendCallback(() => _fadeImage.gameObject.SetActive(true));
 //       sequence.Append(_fadeImage.DOFade(1, 2f).SetEase(Ease.Linear));
 //       sequence.AppendInterval(1f);
 //       sequence.OnComplete(() =>
 //       {
 //           Managers.UI.ClosePopupUI(this);
 //           Managers.UI.MakeSubItem<UI_GangrilNotice>().Init(1);
 //       });
 //       sequence.Play();
 //   }

 //   private float minSpawnTime = 0.1f; // �ּ� ���� �ӵ�
 //   private float startSpawnTime = 1.0f; // ���� �ӵ�
 //   private float acceleration = 0.8f; // ���ӵ� (1���� ������ ���� ������)
 //   private float popupWidth, popupHeight;

 //   private float currentSpawnTime;
	//private float spawnX, spawnY;
 //   private float screenWidth, screenHeight;
 //   private float leftTopX, leftTopY;

 //   private float _xOffset = 70;
 //   private float _yOffset = 50;

 //   private UI_NoticePopup _lastPopup;
 //   private void MakeInfinityPopup(Vector3 startPosition)
 //   {
 //       // 50 -50 �� �Ʒ��� �˾��� �������� �����ϴµ� ��ũ�� ������ �Ѿ�� ���� ������ �ٽ� ������ �Ʒ��� �����Ѵ�
 //       // ó������ õõ�� �����ߴٰ� ���� ������ �����ǰ� �������� ������ �ӵ��� ��� �����Ѵ�
 //       screenWidth = Screen.width;
 //       screenHeight = Screen.height;

 //       spawnX = startPosition.x;
 //       spawnY = startPosition.y;

 //       currentSpawnTime = startSpawnTime;

 //       popupWidth = _backgroundParent.GetComponent<RectTransform>().rect.width;
 //       popupHeight = _backgroundParent.GetComponent<RectTransform>().rect.height;

 //       GetFirstPosition();

 //       StartCoroutine(InfinityPopupCoroutine());
 //   }

 //   /// <summary>
 //   /// �ʱ� ��ġ �ʱ�ȭ
 //   /// </summary>
 //   private void GetFirstPosition()
 //   {
 //       leftTopX = -popupWidth / 2;
 //       leftTopY = popupHeight / 2;

 //       while (leftTopX >= -(screenWidth / 2) && leftTopY <= screenHeight / 2)
 //       {
 //           leftTopX -= _xOffset;
 //           leftTopY += _yOffset;
 //       }

 //       leftTopX += _xOffset;
 //       leftTopY -= _yOffset;
 //       leftTopX += popupWidth / 2;
 //       leftTopY -= popupHeight / 2;
 //   }

 //   private IEnumerator InfinityPopupCoroutine()
 //   {
 //       int spawnCount = 0;
 //       int maxSpawnCount = 20;

 //       while (true)
 //       {
 //           LibraryScene scene = Managers.Scene.CurrentScene as LibraryScene;
 //           scene.StopColorConversion();
 //           scene.ColorConversion(Mathf.Max(currentSpawnTime * 0.5f, 0.15f));
 //           SpawnPopup();

 //           yield return new WaitForSeconds(currentSpawnTime);

 //           // ���� ���� (�ּ� �ӵ����� ũ�� ��� ����)
 //           if (currentSpawnTime > minSpawnTime)
 //           {
 //               currentSpawnTime *= acceleration;
 //               if (currentSpawnTime < minSpawnTime)
 //                   currentSpawnTime = minSpawnTime;
 //           }

 //           spawnCount++;
 //           if (spawnCount >= maxSpawnCount)
 //           {
 //               break;
 //           }
 //       }

 //       StartCoroutine(_lastPopup.BlackOutAndSetText());
 //   }

 //   public void SetActiveFalse()
 //   {
 //       _backgroundParent.gameObject.GetComponent<Image>().sprite = _changeSprite;
 //       _noticeText.gameObject.SetActive(false);
 //       _checkToggle.gameObject.SetActive(false);
 //   }
 //   private IEnumerator BlackOutAndSetText()
 //   {
 //       _fadeImage.gameObject.SetActive(true);
 //       _fadeImage.color = new Color(0, 0, 0, 1);
 //       yield return new WaitForSeconds(2f);

 //       yield return StartCoroutine(_warningText.CoTypingEffect("�ű⼭ ���� ����", 0.5f, true));
 //       yield return new WaitForSeconds(2f);

 //       Managers.Scene.LoadScene(Scene.GameScene);
 //   }

 //   /// <summary>
 //   /// �˾� ����
 //   /// Ư�� ��ġ�� ����
 //   /// </summary>
 //   private void SpawnPopup()
 //   {
 //       Vector3 spawnPosition = new Vector3(spawnX, spawnY, 0);
 //       UI_NoticePopup popup = Managers.UI.ShowPopupUI<UI_NoticePopup>();
      
 //       // ���� ��ġ ���
 //       spawnX += _xOffset;
 //       spawnY -= _yOffset;
 //       if (spawnX + (popupWidth / 2) > screenWidth / 2 || spawnY + -(popupHeight / 2) > screenHeight / 2)
 //       {
 //           _lastPopup.ChangeSprite();

 //           _lastPopup = null;
 //           spawnX = leftTopX;
 //           spawnY = leftTopY;
 //       }

 //       popup.Init(_popupIndex + 1, false, new Vector3(spawnX, spawnY));

 //       if (_lastPopup != null)
 //       {
 //           _lastPopup.SetActiveFalse();
 //       }

 //       _lastPopup = popup;
 //   }

 //   private void ChangeSprite()
 //   {
 //       _checkToggle.onValueChanged.RemoveAllListeners();
 //       _checkToggle.isOn = true;
 //   }
}
