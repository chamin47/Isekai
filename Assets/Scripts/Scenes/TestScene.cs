using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : BaseScene
{
    [SerializeField] private PlayerController _player;
    [SerializeField] private Portal _portal;

    void Start()
    {
        Managers.Happy.OnHappinessChanged += MakePortal;
        Managers.Sound.Play("bgm_isekai_gang", Sound.Bgm);
        Managers.Happy.Happiness = 50f;
    }

    private void MakePortal(float happiness)
    {
        if(happiness >= 100f)
        {
            _portal.gameObject.SetActive(true);
            _portal.SetPortalPosition(Scene.LoadingScene);
            if(_player.transform.position.x < 0f)
                _portal.transform.position = new Vector3(_player.transform.position.x + 3f, -1.9f, 0f);
            else
                _portal.transform.position = new Vector3(_player.transform.position.x - 3f, -1.9f, 0f);

            Managers.Happy.OnHappinessChanged -= MakePortal;
        }
    }

    private void OnDestroy()
    {
        Managers.Happy.OnHappinessChanged -= MakePortal;
    }

    protected override void Init()
    {
        SceneType = Scene.TestScene;
    }

    public override void Clear()
    {
       
    }
}
