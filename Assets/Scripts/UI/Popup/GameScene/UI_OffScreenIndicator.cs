using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class OffScreenIndicatorUI : MonoBehaviour
{
    [SerializeField] private GameObject _leftExclamation;
    [SerializeField] private GameObject _rightExclamation;
    [SerializeField] private Transform _target;

    private readonly List<UI_MiniGame> _activeMiniGames = new List<UI_MiniGame>();

    private MiniGameController _miniGameController;

    void Update()
    {
        _activeMiniGames.RemoveAll(item => item == null);

        bool showLeftIndicator = false;
        bool showRightIndicator = false;

        // 현재 활성화된 모든 미니게임을 순회합니다.
        foreach (var miniGame in _activeMiniGames)
        {
            bool isVisible = IsVisibleOnScreen(miniGame.transform.position);

            if (!isVisible)
            {
                if (miniGame.transform.position.x < _target.position.x)
                {
                    showLeftIndicator = true;
                }
                else
                {
                    showRightIndicator = true;
                }
            }
        }

        // 최종적으로 계산된 상태에 따라 느낌표 UI를 업데이트합니다.
        UpdateIndicatorState(_leftExclamation, showLeftIndicator);
        UpdateIndicatorState(_rightExclamation, showRightIndicator);
    }

    public void Init(MiniGameController controller)
    {
        _miniGameController = controller;
    }

    /// <summary>
    /// 월드 좌표를 기준으로 해당 위치가 화면 내에 보이는지 확인합니다.
    /// </summary>
    /// <param name="worldPosition">체크할 대상의 월드 좌표</param>
    /// <returns>화면 안에 있으면 true, 밖에 있으면 false</returns>
    private bool IsVisibleOnScreen(Vector3 worldPosition)
    {
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(worldPosition);

        bool isOnScreen = viewportPoint.z > 0 &&
                            viewportPoint.x > 0 && viewportPoint.x < 1 &&
                            viewportPoint.y > 0 && viewportPoint.y < 1;

        return isOnScreen;
    }

    private void UpdateIndicatorState(GameObject indicator, bool shouldBeActive)
    {
        if (indicator.activeSelf != shouldBeActive)
        {
            if (shouldBeActive)
            {
                Managers.Sound.Play("exclamation_mark", Sound.Effect);
            }
            indicator.SetActive(shouldBeActive);
        }
    }
    public void RegisterMiniGame(UI_MiniGame miniGame)
    {
        if (!_activeMiniGames.Contains(miniGame))
        {
            _activeMiniGames.Add(miniGame);
        }
    }

    // UI_MiniGame의 OnDestroy에서 이 메소드를 호출해주면 더 정확해집니다.
    public void UnregisterMiniGame(UI_MiniGame miniGame)
    {
        _activeMiniGames.Remove(miniGame);
    }
}