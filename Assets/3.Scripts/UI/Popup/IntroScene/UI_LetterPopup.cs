using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_LetterPopup : UI_Popup
{
	[SerializeField] private TMP_Text _letterText;
	[SerializeField] private Image _letterImage;

	private HappinessHUD _hud;
    private PlayerAnimator _playerAnim;

	private Coroutine _endCoroutine;
	private bool _isEnded = false;
	private float _currentTime = 0f;
	private const float _skipTime = 1.0f;
    private const float _endTime = 5.0f;

    public override void Init()
	{
		base.Init();

		GetComponent<Canvas>().worldCamera = Camera.main;

		_hud = FindAnyObjectByType<HappinessHUD>();
        _playerAnim = FindAnyObjectByType<PlayerAnimator>();

		_hud?.gameObject.SetActive(false);

        _endCoroutine = StartCoroutine(WaitUntilProductionEnd());
	}

    private IEnumerator WaitUntilProductionEnd()
    {
        yield return WaitForSecondsCache.Get(_skipTime);

        float remain = _endTime - _skipTime;
        while (remain > 0f)
        {
            if (Input.GetMouseButtonDown(0))
            {
                EndSequence();
                yield break;
            }

            remain -= Time.deltaTime;
            yield return null;
        }

        EndSequence();
    }

    private void EndSequence()
	{
        _isEnded = true;

        _playerAnim.gameObject.GetComponent<Animator>().Play("cutscene2_player");
		Managers.UI.ShowPopupUI<UI_CutScene2Popup>();
        _hud.gameObject.SetActive(true);
        gameObject.SetActive(false);
    }
}
