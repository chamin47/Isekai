using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ �� ���� ��ȭ �� ����.
/// Ư�� ��ȭ ID ���� ���� ���͹ڽ�/ĳ���� �̵�/å ��ȣ�ۿ� ���� Ʈ�����Ѵ�.
/// </summary>
public class LibraryIDHooks : MonoBehaviour, IDialogueHookProvider
{
	[Header("Scene refs")]
	[SerializeField] private LibraryScene _library;
	[SerializeField] private PlayerController _player;
	[SerializeField] private Transform _librarian;
	[SerializeField] private HappinessHUD _hud;

	[Header("Anchors")]
	[SerializeField] private Vector3 _pos2 = new Vector3(22.23f, -4.86f, 0);
	[SerializeField] private Vector3 _pos3 = new Vector3(25.32f, -4.86f, 0);               

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

				//float baseH = Screen.height * 0.1f;
				//float overshoot = baseH;
				//float settle = baseH * 0.85f;        

				//yield return _letterbox.OpenOvershoot(settle, overshoot, 250f);

				yield break;

			case "2001004":
				if (_librarian != null)
				{
					if (_moveCo != null)
						StopCoroutine(_moveCo);

					_moveCo = StartCoroutine(CoMoveTo(_librarian, _pos2, _walkSpeed, _arriveEps));
				}
				yield break;

			case "2001005":
				if (_moveCo != null)
					yield return _moveCo;
				yield break;


			case "2001024":
				_library.AllBooksEnableFinger();
				yield break;

			case "2001025":
				_library.AllBooksDisableFinger();
				yield break;

			case "2001026":
				{
					_library.EnableBooks();
					WorldType currentWorldType = Managers.World.CurrentWorldType;

					int bookIndex = (int)currentWorldType;
					LibraryBook book = _library.Books[bookIndex].GetComponent<LibraryBook>(); // ���͹�Ʈ
					book.gameObject.SetActive(true);               // ���͹�Ʈ ������Ʈ

					book.EnableFinger();
				}
				yield break;

			case "2001029":
				if (_hud != null)
					_hud.gameObject.SetActive(true);
				{
					WorldType currentWorldType = Managers.World.CurrentWorldType;

					int bookIndex = (int)currentWorldType;
					LibraryBook book = _library.Books[bookIndex].GetComponent<LibraryBook>(); // ���͹�Ʈ
					book.gameObject.SetActive(true);               // ���͹�Ʈ ������Ʈ

					book.EnableClick();

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
}
