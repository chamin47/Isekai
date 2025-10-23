using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryIDHooks : MonoBehaviour, IDialogueHookProvider
{
	[Header("Scene refs")]
	public LibraryScene library;        // (씬에 있는 그 컴포넌트)
	public PlayerController player;     // 플레이어
	public Transform librarian;         // 사서 Transform

	[Header("Anchors")]
	public Vector3 pos2 = new Vector3(22.23f, -4.46f, 0);              // “대화 위치(2)”
	public Vector3 pos3 = new Vector3(25.32f, -4.46f, 0);               // “책상 위치(3)”

	[Header("Move settings")]
	public float walkSpeed = 2.2f;
	public float arriveEps = 0.05f;

	UILetterboxOverlay _letterbox;
	bool _startTimelineEnded;
	Coroutine _moveCo;

	void Awake()
	{
		if (library == null) 
			library = FindAnyObjectByType<LibraryScene>();

		if (player == null) 
			player = FindAnyObjectByType<PlayerController>();

		_letterbox = UILetterboxOverlay.GetOrCreate();

		if (library != null)
			library.onStartTimeLineEnd += () => _startTimelineEnded = true;
	}

	public IEnumerator OnPreEnter(string id)
	{
		switch (id)
		{
			// 2001001: 타임라인(=플레이어 1번 위치 도착)까지 대기 후 레터박스 Out + 이동잠금
			case "2001001":
				// 혹시 모를 입력 방지
				if (player != null) 
					player.canMove = false;

				librarian.GetComponentInChildren<Animator>().CrossFade("Library_ch_sit_idle", 0.01f);

				// 타임라인 완료 대기
				yield return new WaitUntil(() => _startTimelineEnded);

				float baseH = Screen.height * 0.1f;
				float overshoot = baseH;
				float settle = baseH * 0.85f;        // 10 → 7 느낌으로 70%

				yield return _letterbox.OpenOvershoot(settle, overshoot, 250f);

				yield break;

			// 2001004: 사서가 3→2로 실제 이동(걷는 동안 다음 줄 들어가기 전까지 붙잡음)
			case "2001004":
				// 걷기 시작: 3(현재 위치 가정) -> 2로
				if (librarian != null)
				{
					// 이미 돌아가고 있으면 정리
					if (_moveCo != null) 
						StopCoroutine(_moveCo);

					_moveCo = StartCoroutine(CoMoveTo(librarian, pos2, walkSpeed, arriveEps));
				}
				yield break;

			// 2001005: 위에서 시작한 이동이 끝날 때까지 대기
			case "2001005":
				if (_moveCo != null) 
					yield return _moveCo;
				yield break;

			default:
				yield break;
		}
	}

	IEnumerator CoMoveTo(Transform who, Vector3 target, float speed, float eps)
	{
		if (who == null) 
			yield break;

		// Z 고정(2D)
		target.z = who.position.z;

		while (true)
		{
			var pos = who.position;
			var dir = (target - pos);
			dir.z = 0f;
			float dist = dir.magnitude;
			if (dist <= eps) break;

			if (dist > 0.0001f)
			{
				var step = Mathf.Min(dist, speed * Time.deltaTime);
				who.position = pos + dir.normalized * step;
			}
			yield return null;
		}
	}
}
