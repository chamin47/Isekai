using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 월드별 미니게임 특별 규칙을 정의하기 위한 인터페이스입니다.
/// </summary>
public interface IMiniGameRuleStrategy
{
    /// <summary>
    /// 새로 생성된 미니게임에 월드별 규칙을 적용합니다.
    /// </summary>
    public void ApplyRulesTo(UI_MiniGame miniGame, MiniGameController controller);

    /// <summary>
    /// 미니게임 성공 시 호출될 로직입니다.
    /// </summary>
    public void OnMiniGameSucceeded(MiniGameController controller);

    /// <summary>
    /// 게임 종료 조건이 충족되었는지 확인합니다.
    /// </summary>
    public bool HasMetGameEndCondition(MiniGameController controller);
}

public class DefaultRuleStrategy : IMiniGameRuleStrategy
{
    public void ApplyRulesTo(UI_MiniGame miniGame, MiniGameController manager) { }

    public void OnMiniGameSucceeded(MiniGameController manager) { }

    public bool HasMetGameEndCondition(MiniGameController manager) => false;
}

public class PelmanusRuleStrategy : IMiniGameRuleStrategy
{
    private const int SUCCESS_COUNT_TO_WIN = 5;

    public void ApplyRulesTo(UI_MiniGame miniGame, MiniGameController controller)
    {
        // 펠마누스 월드에서는 미니게임 성공 시 OnMiniGameSucceeded 메소드가 호출되도록 이벤트를 구독합니다.
        miniGame.onMiniGameSucced += () => OnMiniGameSucceeded(controller);
    }

    public void OnMiniGameSucceeded(MiniGameController controller)
    {
        controller.SuccessCount++;

        if (Managers.Scene.CurrentScene is GameScene scene)
        {
            scene.SetPostProcessing(controller.SuccessCount);
        }

        if (HasMetGameEndCondition(controller))
        {
            controller.EndGame(true); // 게임 종료 조건 충족 시 controller 알림
        }
    }

    public bool HasMetGameEndCondition(MiniGameController controller)
    {
        return controller.SuccessCount >= SUCCESS_COUNT_TO_WIN;
    }
}

public class MiniGameController : MonoBehaviour
{
    [Header("Component Dependencies")]
    [SerializeField] private MiniGameSpawner _spawner;
    [SerializeField] private OffScreenIndicatorUI _indicatorUI;

    private readonly List<UI_MiniGame> _activeMiniGames = new List<UI_MiniGame>();
    public IReadOnlyList<UI_MiniGame> ActiveMiniGames => _activeMiniGames;

    public int SuccessCount { get; set; } = 0;

    private bool _isGameEnd = false;
    private IMiniGameRuleStrategy _currentRule;
    public event Action<bool> OnGameEnd;

    public void Init()
    {
        // 월드 타입에 맞는 규칙을 생성/선택합니다.
        _currentRule = CreateRuleStrategy(Managers.World.CurrentWorldType);

        //if (_spawner != null) _spawner.OnMiniGameSpawned += HandleMiniGameSpawned;
        //Managers.Happy.OnHappinessChanged += CheckHappinessEndCondition;

        _indicatorUI?.Init(this); 
        _spawner?.BeginSpawning();
    }

    private void HandleMiniGameSpawned(UI_MiniGame miniGame)
    {
        _activeMiniGames.Add(miniGame);
        _currentRule.ApplyRulesTo(miniGame, this);

        miniGame.onMiniGameSucced += () => _activeMiniGames.Remove(miniGame);
    }

    private void CheckHappinessEndCondition(float happiness)
    {
        if (_isGameEnd) return;

        if (happiness <= 0 || happiness >= 100)
        {
            EndGame(happiness >= 100);
        }
    }

    public void EndGame(bool isSuccess)
    {
        if (_isGameEnd) return;

        _isGameEnd = true;

        _spawner?.StopSpawning();
        OnGameEnd?.Invoke(isSuccess);

        foreach(UI_MiniGame miniGame in _activeMiniGames)
        {
            if(miniGame != null)
            {
                miniGame.gameObject.SetActive(false);
            }
        }
        Debug.Log($"게임 종료! 결과: {(isSuccess ? "성공" : "실패")}");
    }

    private IMiniGameRuleStrategy CreateRuleStrategy(WorldType worldType)
    {
        switch (worldType)
        {
            case WorldType.Pelmanus:
                return new PelmanusRuleStrategy();
            // case WorldType.Gangril:
            //     return new GangrilRuleStrategy(); 
            default:
                return new DefaultRuleStrategy();
        }
    }

    private void OnDestroy()
    {
        //if (_spawner != null) _spawner.OnMiniGameSpawned -= HandleMiniGameSpawned;
       
        //Managers.Happy.OnHappinessChanged -= CheckHappinessEndCondition;
    }
}

