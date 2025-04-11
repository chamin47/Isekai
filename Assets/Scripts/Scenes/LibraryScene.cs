using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LibraryScene : BaseScene
{
    [SerializeField] private Material _bgMaterial;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private GameObject _bookParent;
    [SerializeField] private GameObject[] _books;

	[SerializeField] private PlayableDirector _startTimeLine;
    [SerializeField] private ColorAdjustments _noticePopupVolume;
    [SerializeField] private Volume _volume;
    protected override void Init()
	{
		base.Init();

		SceneType = Scene.LibraryScene;

        // ������������ �÷��̾� �̵��ӵ� ����
		Managers.DB.SetPlayerData(
			new PlayerData
			{
				moveSpeed = new List<float> { 1f, 1f, 1f }
			});


        _startTimeLine.stopped += OnTimeLineEnd;
        _volume.profile.TryGet(out _noticePopupVolume);

    }

    // å ���� �����ֱ�
    public void OnTimeLineEnd(PlayableDirector director)
    {
        // ��� �� ����ȭ
        meshRenderer.material = _bgMaterial;

        // Ư�� å �� ���ֱ�
        BookSwitch();
    }

    public void DisableBookSelect()
    {
        _bookParent.SetActive(false);
    } 
    public void EnableBookSelect()
    {
        _bookParent.SetActive(true);
    }
    //������忡 �ش��ϴ� å�� ���ش�
    private void BookSwitch()
    {
        EnableBookSelect();

        WorldType currentWorldType = Managers.World.CurrentWorldType;

        int bookIndex = (int)currentWorldType;

        _books[bookIndex].GetComponent<LibraryBook>().StartFingerBlink();
        _books[bookIndex].GetComponent<BoxCollider2D>().enabled = true;
    }

    // �÷��̾� ������ �ʱ�ȭ
    public override void Clear()
	{
		Managers.DB.ResetPlayerData();
        _startTimeLine.stopped -= OnTimeLineEnd;
    }


    /// <summary>
    /// Gangril ���忡�� noticepopup�� ������ ȿ�� �ֱ�
    /// </summary>
    private Coroutine _colorConversionCoroutine;
    public void ColorConversion(float blinkTime)
    {
        _colorConversionCoroutine =  StartCoroutine(CoColorConversion(blinkTime));
    }

    public void StopColorConversion()
    {
        if (_colorConversionCoroutine != null)
        {
            _noticePopupVolume.colorFilter.value = originColor;
            StopCoroutine(_colorConversionCoroutine);
            _colorConversionCoroutine = null;
        }
    }

    private Color originColor = new Color(1f, 1f, 1f);
    private IEnumerator CoColorConversion(float blinkTime)
    {

        Color targetColor = new Color(140/255f, 0f, 0f);
        Color originColor = _noticePopupVolume.colorFilter.value;
      
        _noticePopupVolume.colorFilter.value = targetColor;
        yield return WaitForSecondsCache.Get(blinkTime);
        _noticePopupVolume.colorFilter.value = originColor;
        yield return WaitForSecondsCache.Get(blinkTime);
    }
}
