using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class VinterRealWorld : MonoBehaviour
{
    [SerializeField] private RealGameFactory _realGameFactory;
    [SerializeField] private PlayableDirector _timeline;
    [SerializeField] private PlayableDirector _enterTimeline;
    [SerializeField] private Transform _player;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private TMPro.TMP_Text _warningText;
    [SerializeField] private GameObject _diary;
    private IEnumerator Start()
    {
        yield return PlayEnterTimeline();

        yield return WaitForSecondsCache.Get(0.3f);

        _warningText.gameObject.SetActive(true);
        yield return _warningText.CoBlinkText(3, 0.5f, "warning");
        _warningText.gameObject.SetActive(false);

        yield return WaitForSecondsCache.Get(0.5f);
        // 게임시작
        RealWorldInfo realWorldInfo = GetRealWorldInfo();

        Managers.Sound.Play("bgm_real_world", Sound.Bgm);

        Init(realWorldInfo);
    }

    private RealWorldInfo GetRealWorldInfo()
    {
        switch (Managers.World.CurrentWorldType)
        {
            case WorldType.Gang:
                return new SecondWorldInfo();
            case WorldType.Vinter:
                return new FirstWorldInfo();
            case WorldType.Chaumm:
                return new ThirdWorldInfo();
            default:
                return new FirstWorldInfo();
        }
    }

    private IEnumerator PlayEnterTimeline()
    {
        if (_enterTimeline != null)
        {
            _enterTimeline.Play();
            yield return new WaitUntil(() => _enterTimeline.state != PlayState.Playing);
        }
    }

    public void Init(RealWorldInfo realWorldInfo)
    {
        _player.GetComponent<PlayerController>().canMove = true;
        _realGameFactory.Init(realWorldInfo);
        _realGameFactory.OnGameEnd += RealGameFactory_OnGameEnd;


        RealGameSceneData realGameSceneData = Managers.DB.GetRealGameSceneData(Managers.World.CurrentWorldType);

        Camera.main.GetComponent<FlowCamera>().SetFollowSpeed(realGameSceneData.cameraSpeed);
        Camera.main.GetComponent<FlowCamera>().StartFlow();
    }

    private void RealGameFactory_OnGameEnd(bool isWin)
    {
        Camera.main.GetComponent<FlowCamera>().StopFlow();
        Debug.Log("EndGame");
        //주인공은 오른쪽으로 이동하고 카메라는 고정
        //화면은 천천히 faded out되고 fade in 되면서 도서관 씬으로 이동
        //상관없이 도서관 씬으로 이동
        PlayerController playerController = _player.GetComponent<PlayerController>();
        playerController.canMove = false;

        _diary.SetActive(false);
        StartCoroutine(CoWaitAndStart());
    }

    private void AdjustTimelinePosition()
    {
        StartCoroutine(CoFadeOut());
        Vector3 startPos = _player.position;

        foreach (var track in _timeline.playableAsset.outputs)
        {
            if (track.streamName == "PlayerPosition")
            {
                AnimationTrack animationTrack = (AnimationTrack)track.sourceObject;
                animationTrack.position = startPos;
            }
        }
    }

    private IEnumerator CoWaitAndStart()
    {
        yield return WaitForSecondsCache.Get(2.0f);
        AdjustTimelinePosition();
        _player.GetComponent<PlayerController>().SetLook(1);
        _timeline.Play();
    }

    private IEnumerator CoFadeOut()
    {
        yield return WaitForSecondsCache.Get(3.0f);

        float fadeOutTime = 1.0f;
        float currentTime = 0.0f;
        Color color = _fadeImage.color;

        while (currentTime < fadeOutTime)
        {
            currentTime += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, currentTime / fadeOutTime);
            _fadeImage.color = color;
            yield return null;
        }

        yield return WaitForSecondsCache.Get(1f);

        Managers.World.MoveNextWorld();

        Managers.Scene.LoadScene(Scene.LibraryScene);

        yield return null;
    }
}
