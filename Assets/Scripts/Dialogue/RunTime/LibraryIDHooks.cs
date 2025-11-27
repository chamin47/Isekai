using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 도서관 씬 전용 대화 훅 집합.
/// 특정 대화 ID 진입 전에 레터박스/캐릭터 이동/책 상호작용 등을 트리거한다.
/// </summary>
public class LibraryIDHooks : MonoBehaviour, IDialogueHookProvider
{
	[Header("Scene refs")]
	[SerializeField] private LibraryScene _library;
	[SerializeField] private PlayerController _player;
	[SerializeField] private Transform _librarian;
	[SerializeField] private HappinessHUD _hud;

	[Header("Anchors")]
	[SerializeField] private Vector3 _pos2 = new Vector3(22.23f, -3.45f, 0);
	[SerializeField] private Vector3 _pos3 = new Vector3(24.54f, -3.45f, 0);               

	[Header("Move settings")]
	[SerializeField] private float _walkSpeed = 2.2f;
	[SerializeField] private float _arriveEps = 0.05f;

	private UILetterboxOverlay _letterbox;
	private bool _startTimelineEnded;
	private Coroutine _moveCo;

	private void Awake()
	{
		if (_library == null) 
			_library = FindAnyObjectByType<LibraryScene>();
			
		if (_player == null) 
			_player = FindAnyObjectByType<PlayerController>();

		_letterbox = UILetterboxOverlay.GetOrCreate();

		if (_library != null)
			_library.onStartTimeLineEnd += () => _startTimelineEnded = true;
	}

	public IEnumerator OnPreEnter(string id)
	{
		switch (id)
		{
			case "2001001":

				_librarian.GetComponentInChildren<Animator>().CrossFade("Library_ch_sit_idle", 0.01f);

				yield return new WaitUntil(() => _startTimelineEnded);

				if (_player != null)
					_player.canMove = false;

				if (_hud != null)
					_hud.gameObject.SetActive(false);


				yield break;

			case "2001002":

				Managers.Sound.Play("librarian1", Sound.Effect);

				yield break;

			case "2001004":
				if (_librarian != null)
				{
					if (_moveCo != null)
						StopCoroutine(_moveCo);

					_moveCo = StartCoroutine(CoMoveTo(_librarian, _pos2, _walkSpeed, _arriveEps));

					StartCoroutine(CoLibrarianFootSound());

				}
				yield break;

			case "2001005":
				if (_moveCo != null)
					yield return _moveCo;

				var anim = _librarian.GetComponentInChildren<Animator>();
				anim.CrossFade("Library_ch_idle", 0.05f);

				{
					float baseH = Screen.height * 0.1f;
					float overshoot = baseH;
					float settle = baseH * 0.85f;

					StartCoroutine(_letterbox.OpenOvershoot(settle, overshoot, 250f));
				}

				yield return new WaitForSeconds(1f);
				yield break;

			case "2001009":

				Managers.Sound.Play("isekai_library", Sound.Effect);

				yield break;

			case "2001024":
				//_library.AllBooksEnableFinger();
				yield break;

			case "2001025":
				//_library.AllBooksDisableFinger();
				yield break;

			case "2001026":
				{
					_library.EnableBooks();
					WorldType currentWorldType = Managers.World.CurrentWorldType;

					int bookIndex = (int)currentWorldType;
					LibraryBook book = _library.Books[bookIndex].GetComponent<LibraryBook>(); // 빈터발트
					book.gameObject.SetActive(true);               // 빈터발트 오브젝트

					StartCoroutine(book.FadeInMouse());
				}
				yield break;


			case "2001027":

				Managers.Sound.Play("positive", Sound.Effect);

				yield break;
			case "2001029":
				if (_hud != null)
					_hud.gameObject.SetActive(true);
				{
					WorldType currentWorldType = Managers.World.CurrentWorldType;

					int bookIndex = (int)currentWorldType;
					LibraryBook book = _library.Books[bookIndex].GetComponent<LibraryBook>(); // 빈터발트
					book.gameObject.SetActive(true);               // 빈터발트 오브젝트

					book.EnableClick();

					float baseH = Screen.height * 0.1f;  
					float overshoot = baseH;
					float settle = baseH * 0.85f;        // 10 → 8.5 느낌으로 85%

					yield return _letterbox.CloseOvershoot(settle, overshoot, 170f);
				}
				
				yield break;

			default:
				yield break;
		}
	}

	IEnumerator CoMoveTo(Transform who, Vector3 target, float speed, float eps)
	{
		if (who == null) 
			yield break;

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

	private IEnumerator CoLibrarianFootSound()
	{
		const float STEP_INTERVAL = 0.63f;   // 걸음 간격
		const int STEP_COUNT = 3;           // 총 발걸음 횟수
		float curVolume = 0.55f;             // 시작 볼륨
		const float VOL_STEP = 0.2f;        // 한 걸음당 볼륨 증가량

		for (int i = 0; i < STEP_COUNT; i++)
		{
			Managers.Sound.Play("all_s_walk2", Sound.Effect, curVolume);
			curVolume = Mathf.Min(curVolume + VOL_STEP, 1f); 
			yield return WaitForSecondsCache.Get(STEP_INTERVAL);
		}
	}

}
