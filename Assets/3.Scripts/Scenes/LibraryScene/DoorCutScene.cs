using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DoorCutScene : MonoBehaviour
{
	[Header("Player")]
	[SerializeField] private PlayerController _playerPrefab;

	private readonly Vector3 START_POS = new Vector3(19.48f, -2.68f, 0);
	private readonly Vector3 STOP_POS = new Vector3(20.98f, -2.68f, 0);

	private Image _fadeImage;
	public Image FadeImage { set => _fadeImage = value; }

	private List<string> _lines = new() 
	{ "<b>언제까지 거기 있을 거야?</b>", "<b>그곳에서 당장 나오지 않는다면</b>", "<b>넌 결국</b>", "<b><color=red>!@($#@!!@하게 될 거야.</color></b>" };

	IEnumerator Start()
    {	
		Managers.UI.CloseAllPopupUI();
		FindAnyObjectByType<HappinessHUD>().gameObject.SetActive(false);

		Animator animator = GetComponent<Animator>();
		animator.speed = 0f;
		yield return _fadeImage.CoFadeIn(0f);
		yield return WaitForSecondsCache.Get(2f);	// 화면 다 밝아진 후 해당 화면 2초 유지
		animator.speed = 1f;						// 문이 열리는 애니메이션 & 사운드 (사운드는 애니메이션 이벤트)
    }

	public void OpenDoorAnimationEvent()            // 문이 완전히 열리는 시점(애니메이션 끝나는 시점)에 발생하는 애니메이션 이벤트
	{
		StartCoroutine(DoorCutSceneSequence());
	}

	public void OpenDoorSoundEvent()                // 문이 열리는 순간 발생하는 이벤트
	{
		Managers.Sound.Play("door_open2", Sound.Effect);
	}

	public void KnockSoundEvent()
	{
		Managers.Sound.Play("Knock", Sound.Effect);
	}

	private IEnumerator DoorCutSceneSequence()
	{
		yield return WaitForSecondsCache.Get(2f);

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

		// 걷는 소리가 나면서 주인공이 왼쪽에서 걸어옴
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

		anim.SetFloat("Speed", 0f);                          // 주인공이 문 중앙에 위치하면 이동을 멈춤

		yield return WaitForSecondsCache.Get(2f);            // 2초동안 현 상태 유지 후 다음 화면

		GameObject go = Managers.Resource.Instantiate("UI/WorldSpace/UI_Bubble");
		BubbleFollow bubbleFollow = go.GetComponent<BubbleFollow>();

		bubbleFollow.SetTarget(player.transform);
		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[0];
		Managers.Sound.Play("s1_say_impact2", Sound.Effect);
		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f;  // 말풍선 아웃

		yield return WaitForSecondsCache.Get(3f);  // 말풍선 아웃되고 3초 유지 후

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 0f; // 대사가 사라짐과 동시에
		Vector3 playerPos = player.transform.position;
		playerPos.y = -3.5f;
		player.transform.localScale = new Vector3(4.5f, 4.5f, 1f);
		player.transform.position = playerPos;

		yield return WaitForSecondsCache.Get(1f); // 그리고 살짝 텀 주고

		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[1];
		bubbleFollow.Offset += new Vector3(0f, 3.0f, 0f);
		Managers.Sound.Play("s1_say_impact2", Sound.Effect);
		bubbleFollow.transform.localScale *= 1.4f;


		Image bubbleImage = bubbleFollow.GetComponentInChildren<Image>();
		RectTransform rectTransform = bubbleImage.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(2.88f, rectTransform.sizeDelta.y);

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f; // 대사 아웃

		yield return WaitForSecondsCache.Get(3f); // 말풍선 아웃되고 3초 유지 후

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 0f; // 대사가 사라짐과 동시에
		player.transform.localScale = new Vector3(8f, 8f, 1f); // 캐릭터 커지고
		player.transform.position += new Vector3(0, -1.5f, 0f); // 가까워짐

		yield return WaitForSecondsCache.Get(1f); // 그리고 살짝 텀 주고

		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[2];
		bubbleFollow.Offset += new Vector3(0f, 2.7f, 0f);
		Managers.Sound.Play("s1_say_impact2", Sound.Effect);
		bubbleFollow.transform.localScale *= 1.4f;

		rectTransform.sizeDelta = new Vector2(1.18f, rectTransform.sizeDelta.y);

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f; // 대사 아웃

		yield return WaitForSecondsCache.Get(3f); // 말풍선 아웃되고 3초 유지 후

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 0f; // 대사가 사라짐과 동시에
		player.transform.localScale = new Vector3(13f, 13f, 13f); // 캐릭터가 커지고
		playerPos = player.transform.position;
		playerPos.y = -9.69f;
		player.transform.position = playerPos; // 가까워짐

		yield return WaitForSecondsCache.Get(1f); // 살짝 텀

		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[3];
		bubbleFollow.Offset += new Vector3(0f, 4.3f, 0f);
		Managers.Sound.Play("getout_long", Sound.Effect);
		bubbleFollow.transform.localScale *= 1.2f;

		rectTransform.sizeDelta = new Vector2(2.4f, rectTransform.sizeDelta.y);

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f; // 대사 아웃

		yield return WaitForSecondsCache.Get(3f);
		Managers.Scene.LoadScene(Scene.GameScene);
	}
}