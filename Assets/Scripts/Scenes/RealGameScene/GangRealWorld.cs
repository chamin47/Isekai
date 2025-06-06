using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;


public class GangRealWorld : MonoBehaviour
{
    [SerializeField] private Transform _endCameraPos;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private PlayableDirector _startTimeLine;
    [SerializeField] private PlayerController _player;
    [SerializeField] private GiantController _giant;
    public void Start()
    {
        Managers.Sound.Play("bgm_real_world", Sound.Bgm);

        _player.canMove = false;
        Managers.Happy.Happiness = 50f;

        _startTimeLine.Play();
        _startTimeLine.stopped += OnStartTimelineStopped;

    }

    private void OnStartTimelineStopped(PlayableDirector director)
    {
        CameraFollowTarget cameraFollow = Camera.main.GetComponent<CameraFollowTarget>();
        cameraFollow.Init(_player.transform, Vector3.zero);
        cameraFollow.CanFollow = true;
        _player.canMove = true;
        _giant.ChangeState(GiantController.GiantState.Walk);
    }

}
