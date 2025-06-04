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
	[SerializeField] private Image _fadeImage;
	[SerializeField] private TMPro.TMP_Text _warningText;
	[SerializeField] private List<EventTrigger2D> _pageTriggers;
	[SerializeField] private UI_BubbleTest _bubbleA;
	[SerializeField] private UI_BubbleTest _bubbleB;
	[SerializeField] private PlayerController _playerController;

	private void Awake()
	{
		_bubbleA.OnDialogTriggered += HandleFadeOutCharacter;
		_bubbleB.OnDialogTriggered += HandleFadeOutCharacter;
	}

	private void OnDestroy()
	{
		_bubbleA.OnDialogTriggered -= HandleFadeOutCharacter;
		_bubbleB.OnDialogTriggered -= HandleFadeOutCharacter;
	}

	private IEnumerator Start()
	{
		_playerController.isMoving = true;
		Managers.Sound.Play("realWorldBgm", Sound.Bgm);
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

	// 듀얼 대사
	public void HandlePageTrigger(DialogUnit dialogUnitA, DialogUnit dialogUnitB)
	{
		StartCoroutine(CoHandleDialog(dialogUnitA, dialogUnitB));
	}

	private IEnumerator CoHandleDialog(DialogUnit dialogUnit)
	{
		//_playerController.Stop();
		//Camera.main.GetComponent<FlowCamera>().StopFlow();

		// 말풍선 위치를 캐릭터 위치 기준으로 먼저 지정
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
		while (t < 1f)
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

