using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// NO 선택 시 2 초 암전 → 주인공 등장 → 말풍선 → 타이틀 귀환
/// </summary>
public class TrueEnding_NoRouteScene : BaseScene
{
	[Header("Screen‑Fade")]
	[SerializeField] private CanvasGroup _fadeGroup;

	[Header("Player")]
	[SerializeField] private PlayerController _playerPrefab;   // Player 프리팹 (SetActive false)

	[Header("Bubble")]
	[SerializeField] private CanvasGroup _bubbleGroup;         // 말풍선 CanvasGroup (α 0)
	[SerializeField] private TMP_Text _bubbleText;
	[SerializeField] private BubbleFollow _bubbleFollow; 

	private readonly Vector3 START_POS = new(-9.18f, -5f);     // 화면 왼쪽 밖
	private readonly Vector3 STOP_POS = new(0f, -5f);     // 화면 중앙

	private const string NO_BUBBLE = "이세계에 갇힌 나를 구해줘.";

	// 컷신 파라미터 
	private const float WALK_TIME = 5f;          // 걷는 컷신 길이
	private const float ANIM_SPEED = 0.33f;       // Walk1 블렌드‑트리 값
	
	private const float MIN_VOL = 0.55f;       // 첫 걸음 볼륨
	private const float MAX_VOL = 1.00f;       // 마지막 볼륨까지 선형 상승

	protected override void Init()
	{
		SceneType = Scene.TrueEnding_NoRouteScene;
		StartCoroutine(Co_NoRouteSequence());
	}

	public override void Clear() { }

	IEnumerator Co_NoRouteSequence()
	{
		yield return FadeCanvas(_fadeGroup, 0, 1f);

		PlayerController player = Instantiate(_playerPrefab);
		player.enabled = false;      // 입력 차단
		player.canMove = false;
		player.transform.position = START_POS;
		player.gameObject.SetActive(true);
		player.SetLook(+1);

		_bubbleFollow.SetTarget(player.transform);

		Animator anim = player.GetComponentInChildren<Animator>();

		const float WALK_TIME = 5f;      // 컷신 걷기 총시간
		const float ANIM_SPEED = 0.33f;   // Walk1 속도
		
		const float MIN_VOL = 0.55f;   // 첫 걸음 볼륨
		const float MAX_VOL = 1.00f;   // 마지막 걸음 볼륨

		Vector3 dir = (STOP_POS - START_POS).normalized;
		float fullDist = Vector3.Distance(START_POS, STOP_POS);

		float elapsed = 0f;
		
		Vector3 prevPos = START_POS;

		float nextStepCycle = 0.0f;

		while (elapsed < WALK_TIME)
		{
			float dt = Time.deltaTime;
			elapsed += dt;

			float t01 = Mathf.SmoothStep(0f, 1f, elapsed / WALK_TIME);
			Vector3 cur = Vector3.Lerp(START_POS, STOP_POS, t01);
			player.transform.position = cur;

			anim.SetFloat("Speed", ANIM_SPEED);

			AnimatorStateInfo st = anim.GetCurrentAnimatorStateInfo(0);
			if (st.normalizedTime >= nextStepCycle)
			{
				float vol = Mathf.Lerp(MIN_VOL, MAX_VOL, t01);
				Managers.Sound.Play("all_s_walk2", Sound.Effect, vol);
				nextStepCycle += 1f;       // 한 사이클마다 한 번
			}

			prevPos = cur;
			yield return null;
		}

		Managers.Sound.Play("all_s_walk2", Sound.Effect, MAX_VOL);

		anim.SetFloat("Speed", 0f);

		yield return WaitForSecondsCache.Get(3f);

		_bubbleGroup.alpha = 1f;
		_bubbleText.text = "";

		foreach (char ch in NO_BUBBLE)
		{
			_bubbleText.text += ch;
			if (ch == ' ' || ch == '\n') continue;

			Managers.Sound.Play("intro_type_short", Sound.Effect);
			yield return WaitForSecondsCache.Get(0.5f);
		}
		yield return WaitForSecondsCache.Get(3f);

		Coroutine c1 = StartCoroutine(FadeCanvas(_bubbleGroup, 0f, 1f));
		Coroutine c2 = StartCoroutine(FadeCanvas(_fadeGroup, 1f, 1f));
		yield return c1; yield return c2;

		Managers.Scene.LoadScene(Scene.TitleScene);
	}

	private IEnumerator FadeCanvas(CanvasGroup cg, float to, float time)
	{
		float from = cg.alpha;
		for (float t = 0f; t < time; t += Time.deltaTime)
		{
			cg.alpha = Mathf.Lerp(from, to, t / time);
			yield return null;
		}
		cg.alpha = to;
	}
}
