using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryIDHooks : MonoBehaviour, IDialogueHookProvider
{
	[Header("Scene refs")]
	public LibraryScene library;        // (���� �ִ� �� ������Ʈ)
	public PlayerController player;     // �÷��̾�
	public Transform librarian;         // �缭 Transform

	[Header("Anchors")]
	public Vector3 pos2 = new Vector3(22.23f, -4.46f, 0);              // ����ȭ ��ġ(2)��
	public Vector3 pos3 = new Vector3(25.32f, -4.46f, 0);               // ��å�� ��ġ(3)��

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
			// 2001001: Ÿ�Ӷ���(=�÷��̾� 1�� ��ġ ����)���� ��� �� ���͹ڽ� Out + �̵����
			case "2001001":
				// Ȥ�� �� �Է� ����
				if (player != null) 
					player.canMove = false;

				librarian.GetComponentInChildren<Animator>().CrossFade("Library_ch_sit_idle", 0.01f);

				// Ÿ�Ӷ��� �Ϸ� ���
				yield return new WaitUntil(() => _startTimelineEnded);

				float baseH = Screen.height * 0.1f;
				float overshoot = baseH;
				float settle = baseH * 0.85f;        // 10 �� 7 �������� 70%

				yield return _letterbox.OpenOvershoot(settle, overshoot, 250f);

				yield break;

			// 2001004: �缭�� 3��2�� ���� �̵�(�ȴ� ���� ���� �� ���� ������ ������)
			case "2001004":
				// �ȱ� ����: 3(���� ��ġ ����) -> 2��
				if (librarian != null)
				{
					// �̹� ���ư��� ������ ����
					if (_moveCo != null) 
						StopCoroutine(_moveCo);

					_moveCo = StartCoroutine(CoMoveTo(librarian, pos2, walkSpeed, arriveEps));
				}
				yield break;

			// 2001005: ������ ������ �̵��� ���� ������ ���
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

		// Z ����(2D)
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
