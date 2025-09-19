using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorCutScene : MonoBehaviour
{
	[Header("Player")]
	[SerializeField] private PlayerController _playerPrefab;

	private readonly Vector3 START_POS = new Vector3(17.89f, -2.68f, 0);
	private readonly Vector3 STOP_POS = new Vector3(20.98f, -2.68f, 0);

	private Image _fadeImage;
	public Image FadeImage { set => _fadeImage = value; }

	private List<string> _lines = new() 
	{ "<b>언제까지 거기 있을 거야?</b>", "<b>그러다 넌 결국</b>", "<b><color=red>갇히게 될 거야.</color></b>" };

	IEnumerator Start()
    {
		Managers.UI.CloseAllPopupUI();

		Animator animator = GetComponent<Animator>();
		animator.speed = 0f;
		yield return _fadeImage.CoFadeIn(2f);
		yield return WaitForSecondsCache.Get(2f);
		animator.speed = 1f;
    }

	public void OpenDoorAnimationEvent()
	{
		StartCoroutine(DoorCutSceneSequence());
	}

	public void OpenDoorSoundEvent()
	{
		Managers.Sound.Play("door_open2", Sound.Effect);
	}

	private IEnumerator DoorCutSceneSequence()
	{
		PlayerController player = Instantiate(_playerPrefab);
		player.enabled = false;
		player.canMove = false;
		player.GetComponent<Collider2D>().enabled = false;
		player.transform.position = START_POS;
		player.gameObject.SetActive(true);
		player.SetLook(1);

		Animator anim = player.GetComponentInChildren<Animator>();

		const float WALK_TIME = 5f;
		const float ANIM_SPEED = 0.33f;

		float elapsed = 0f;

		Vector3 prevPos = START_POS;

		float nextStepCycle = 0.0f;

		while (elapsed < WALK_TIME)
		{
			elapsed += Time.deltaTime;

			float t01 = Mathf.SmoothStep(0f, 1f, elapsed / WALK_TIME);
			Vector3 cur = Vector3.Lerp(START_POS, STOP_POS, t01);
			player.transform.position = cur;

			anim.SetFloat("Speed", ANIM_SPEED);

			AnimatorStateInfo st = anim.GetCurrentAnimatorStateInfo(0);
			if (st.normalizedTime >= nextStepCycle)
			{
				Managers.Sound.Play("all_s_walk2", Sound.Effect);
				nextStepCycle += 1;
			}

			prevPos = cur;
			yield return null;
		}

		//Managers.Sound.Play("all_s_walk2", Sound.Effect);

		anim.SetFloat("Speed", 0f);

		yield return WaitForSecondsCache.Get(2f);

		Vector3 playerPos = player.transform.position;
		playerPos.y = -3.5f;
		player.transform.localScale = new Vector3(4.5f, 4.5f, 1f);
		player.transform.position = playerPos;

		yield return WaitForSecondsCache.Get(1f);

		GameObject go = Managers.Resource.Instantiate("UI/WorldSpace/UI_Bubble");
		BubbleFollow bubbleFollow = go.GetComponent<BubbleFollow>();

		bubbleFollow.SetTarget(player.transform);
		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[0];
		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f;
		Managers.Sound.Play("s1_say_impact2", Sound.Effect);

		yield return WaitForSecondsCache.Get(3f);

		player.transform.localScale = new Vector3(8f, 8f, 1f);
		player.transform.position += new Vector3(0, -1.5f, 0f);
		bubbleFollow.GetComponent<CanvasGroup>().alpha = 0f;

		yield return WaitForSecondsCache.Get(1f);

		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[1];
		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f;
		bubbleFollow.Offset += new Vector3(0f, 3.0f, 0f);
		Managers.Sound.Play("s1_say_impact3", Sound.Effect);

		yield return WaitForSecondsCache.Get(3f);

		playerPos = player.transform.position;
		playerPos.y = -9.69f;
		player.transform.localScale = new Vector3(13f, 13f, 13f);
		player.transform.position = playerPos;
		bubbleFollow.GetComponent<CanvasGroup>().alpha = 0f;

		yield return WaitForSecondsCache.Get(1f);

		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[2];
		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f;
		bubbleFollow.Offset += new Vector3(0f, 4.65f, 0f);
		Managers.Sound.Play("getout_long", Sound.Effect);

		yield return WaitForSecondsCache.Get(3f);
		Managers.Scene.LoadScene(Scene.GameScene);

	}
}