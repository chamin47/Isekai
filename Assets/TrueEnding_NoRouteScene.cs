using System.Collections;
using TMPro;                    
using UnityEngine;

/// <summary>
/// NO 선택 시 2초 암전 → 주인공 등장 → 말풍선 → 타이틀 귀환
/// </summary>
public class TrueEnding_NoRouteScene : BaseScene
{
	[Header("Screen‑Fade")]
	[SerializeField] CanvasGroup _fadeGroup;            

	[Header("Player")]
	[SerializeField] PlayerController _playerPrefab;    // Player 프리팹 (SetActive false)

	[Header("Bubble")]
	[SerializeField] CanvasGroup _bubbleGroup;          // 말풍선 CanvasGroup (α 0)
	[SerializeField] TMP_Text _bubbleText;

	readonly Vector3 START_POS = new(-9.18f, -5f);        // 화면 왼쪽 밖
	readonly Vector3 STOP_POS = new(0f, -5f);         // 화면 중앙쯤
	const float WALK_TIME = 2f;                    // 걷는 시간
	const string NO_BUBBLE = "이세계에 갇힌 나를 구해줘.";

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
		player.enabled = false;                      // 컷신 전용(입력 차단)
		player.gameObject.SetActive(true);

		player.transform.position = START_POS;
		player.SetLook(+1);                         

		Animator anim = player.GetComponentInChildren<Animator>();

		// START → STOP 을 2초 동안 보간 이동
		float dist = Vector3.Distance(START_POS, STOP_POS);
		Vector3 dir = (STOP_POS - START_POS).normalized;
		float walkT = 0f;
		int step = 0;

		const float WALK_TIME = 2f;
		const float WALK_SPEED = 5f;  

		while (walkT < WALK_TIME)
		{
			float dt = Time.deltaTime;
			walkT += dt;

			float t = Mathf.Clamp01(walkT / WALK_TIME);            // 0 → 1
			player.transform.position = START_POS + dir * dist * t;

			anim.SetFloat("Speed", WALK_SPEED);

			// 발자국 5등분
			if (walkT >= (step + 1) * (WALK_TIME / 5f))
			{
				player.PlayFootSound();
				step++;
			}
			yield return null;
		}

		anim.SetFloat("Speed", 0f);

		yield return WaitForSecondsCache.Get(3f);

		_bubbleGroup.alpha = 1;
		_bubbleText.text = "";
		foreach (char ch in NO_BUBBLE)
		{
			_bubbleText.text += ch;

			if (ch == ' ' || ch == '\n')
				continue;

			Managers.Sound.Play("intro_type_short", Sound.Effect);
			yield return WaitForSecondsCache.Get(1f);
		}
		yield return WaitForSecondsCache.Get(3f);

		yield return FadeCanvas(_bubbleGroup, 0, 1f);
		yield return FadeCanvas(_fadeGroup, 1, 1f);
		Managers.Scene.LoadScene(Scene.TitleScene);
	}

	IEnumerator FadeCanvas(CanvasGroup cg, float to, float time)
	{
		float from = cg.alpha;
		float t = 0f;
		while (t < time)
		{
			t += Time.deltaTime;
			cg.alpha = Mathf.Lerp(from, to, t / time);
			yield return null;
		}
		cg.alpha = to;
	}
}
