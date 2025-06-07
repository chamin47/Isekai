using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

public class DongMinTestScene : BaseScene
{
	[SerializeField] private PlayableDirector _timeline;
	[SerializeField] private PlayableDirector _enterTimeline;
	[SerializeField] private Transform _player;
	[SerializeField] private SpriteRenderer _fadeImage;
	[SerializeField] private TMPro.TMP_Text _warningText;
	[SerializeField] private List<EventTrigger2D> _pageTriggers;
	[SerializeField] private UI_BubbleTest _bubbleA;
	[SerializeField] private UI_BubbleTest _bubbleB;
	[SerializeField] private PlayerController _playerController;
	[SerializeField] private CameraStopCollider _cameraStopCollider;
	[SerializeField] private DialogUnit _endingUnit;
	[SerializeField] private Transform _playerTargetPosition;
	[SerializeField] private Animator _playerAnimator;
    private void Awake()
	{
		_bubbleA.OnDialogTriggered += HandleFadeOutCharacter;
		_bubbleB.OnDialogTriggered += HandleFadeOutCharacter;
		_cameraStopCollider.OnCameraStopped += OnCameraStop;

    }

	private void OnDestroy()
	{
		_bubbleA.OnDialogTriggered -= HandleFadeOutCharacter;
		_bubbleB.OnDialogTriggered -= HandleFadeOutCharacter;
        _cameraStopCollider.OnCameraStopped -= OnCameraStop;
    }

	bool isEndingScene = false;
	private void OnCameraStop()
	{
        if (isEndingScene)
            return;

        isEndingScene = true;

        _bubbleA.gameObject.GetComponent<Collider2D>().enabled = false;
        _bubbleA.SetDialogUnit(_endingUnit);
        StartCoroutine(EndingSequence());
    }

    private IEnumerator EndingSequence()
    {
		_playerController.canMove = false;
		_playerController.SetLook(1);
		_playerController.enabled = false;
        bool isMoveEnd = false;
        _playerAnimator.SetFloat("Speed", 1f);
        _playerController.transform.DOMoveX(_playerTargetPosition.position.x, 2f).SetEase(Ease.Linear).OnComplete(() => { isMoveEnd = true; });
		Coroutine coroutine = StartCoroutine(PlayFootsteps(2f));
        while (!isMoveEnd)
		{
            yield return null;
        }
        StopCoroutine(coroutine);
        _playerAnimator.SetFloat("Speed", 0f);

		yield return WaitForSecondsCache.Get(2f);
		_bubbleA.SetPosition(_endingUnit.characterObject.transform.position);
        yield return StartCoroutine(_bubbleA.ShowText(0.5f));
		
		StartCoroutine(CoFadeOutCharacter(_endingUnit.characterObject, 2f));
        yield return StartCoroutine(_bubbleA.FadeAll(2f));

        yield return StartCoroutine(FadeToBlack());

        _playerAnimator.SetFloat("Speed", 1f);
        coroutine = StartCoroutine(PlayFootsteps(3f));
        _playerController.transform.DOMoveX(_playerController.transform.position.x + 12f, 4f).SetEase(Ease.Linear).OnComplete(() => { isMoveEnd = true; });

        while (!isMoveEnd)
        {
            yield return null;
        }
        StopCoroutine(coroutine);
        _playerAnimator.SetFloat("Speed", 0f);

        Managers.World.MoveNextWorld();
        Managers.Scene.LoadScene(Scene.LibraryScene);
    }

    private IEnumerator PlayFootsteps(float duration)
    {
        float timer = 0f;
        float interval = 0.7f; // �߼Ҹ� ����
        while (timer < duration)
        {
            Managers.Sound.Play("all_s_walk2", Sound.Effect);
            yield return new WaitForSeconds(interval);
            timer += interval;
        }
    }

    private IEnumerator Start()
	{
		_playerController.canMove = true;
		//Managers.Sound.Play("realWorldBgm", Sound.Bgm);
		yield return PlayEnterTimeline();

		yield return new WaitForSeconds(0.3f);
		yield return _warningText.CoBlinkText(3, 0.5f);
		yield return new WaitForSeconds(0.5f);

		Managers.Sound.Play("bgm_real_world", Sound.Bgm);

		yield return null;

		float actualSpeed = _playerController.GetCurrentXSpeed();
		Camera.main.GetComponent<FlowCamera>().SetFollowSpeed(2f);
		Debug.Log(actualSpeed);
		Camera.main.GetComponent<FlowCamera>().StartFlow();
	}

	private IEnumerator PlayEnterTimeline()
	{
		if (_enterTimeline != null)
		{
			_enterTimeline.Play();
			yield return new WaitUntil(() => _enterTimeline.state != PlayState.Playing);
		}
	}

	public void HandlePageTrigger(DialogUnit dialogUnit)
	{
		StartCoroutine(CoHandleDialog(dialogUnit));
	}

	// ��� ���
	public void HandlePageTrigger(DialogUnit dialogUnitA, DialogUnit dialogUnitB)
	{
		StartCoroutine(CoHandleDialog(dialogUnitA, dialogUnitB));
	}

	private IEnumerator CoHandleDialog(DialogUnit dialogUnit)
	{
		//_playerController.Stop();
		//Camera.main.GetComponent<FlowCamera>().StopFlow();

		// ��ǳ�� ��ġ�� ĳ���� ��ġ �������� ���� ����
		_bubbleA.SetPosition(dialogUnit.characterObject.transform.position);

		if (dialogUnit.useTyping)
		{
			yield return _bubbleA.CoTyping(dialogUnit.dialogs);
		}

		//_bubbleA.FadeOut();
		//StartCoroutine(CoFadeOutCharacter(dialogUnit.characterObject));

		yield return new WaitForSeconds(1f);

		if (dialogUnit.isFinal)
		{
			yield return CoFinalTransition();
		}
		else
		{
			Camera.main.GetComponent<FlowCamera>().StartFlow();
		}
	}

	private IEnumerator CoHandleDialog(DialogUnit dialogUnitA, DialogUnit dialogUnitB)
	{
		_bubbleA.SetPosition(dialogUnitA.characterObject.transform.position);
		yield return _bubbleA.CoTyping(dialogUnitA.dialogs);
		
		_bubbleB.SetPosition(dialogUnitB.characterObject.transform.position + new Vector3(5.5f, 0, 0));
		yield return _bubbleB.CoTyping(dialogUnitB.dialogs);

		//_bubbleA.FadeOut();
		//_bubbleB.FadeOut();
		yield return new WaitForSeconds(1f);

		Camera.main.GetComponent<FlowCamera>().StartFlow();
	}

	private IEnumerator CoFinalTransition()
	{
		yield return new WaitForSeconds(1.5f);
		yield return FadeToBlack();

		yield return new WaitForSeconds(2f);

		Managers.Scene.LoadScene(Scene.LibraryScene);
	}

	public IEnumerator FadeToBlack()
	{
		float t = 0f;
		Color color = _fadeImage.color;
		while (t < 2f)
		{
			t += Time.deltaTime;
			color.a = Mathf.Lerp(0, 1, t);
			_fadeImage.color = color;
			yield return null;
		}
	}

	public IEnumerator CoFadeOutCharacter(GameObject obj, float duration = 1f)
	{
		float t = 0f;

		var sr = obj.GetComponentInChildren<SpriteRenderer>();
		if (sr != null)
		{
			Color originalColor = sr.color;

			while (t < duration)
			{
				t += Time.deltaTime;
				float alpha = Mathf.Lerp(1, 0, t / duration);
				sr.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
				yield return null;
			}
		}
	}

	public void HandleFadeOutCharacter(GameObject obj)
	{
		StartCoroutine(CoFadeOutCharacter(obj));
	}

	protected override void Init()
	{
		SceneType = Scene.DongMinTestScene;
	}

	public override void Clear() { }
}

[System.Serializable]
public class DialogUnit
{
	public List<string> dialogs;
	public GameObject characterObject;
	public bool useTyping = true;
	public bool isFinal = false;
	public int happinessDelta = -20;
	public bool fadeCharacter = true;
	public Vector2 bubbleSize;
}

