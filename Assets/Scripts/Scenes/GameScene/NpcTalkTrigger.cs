using UnityEngine;

[DisallowMultipleComponent]
public class NpcTalkTrigger : MonoBehaviour
{
	public bool RequireShift = true;
	public string PlayerTag = "Player";

	private UI_MiniGame _mini;
	private bool _playerInside;
	private bool _used;

	public void Bind(UI_MiniGame mini)
	{
		_mini = mini;
		_used = false;
		_playerInside = false;
		enabled = true;
	}

	private void Update()
	{
		if (_used || !_playerInside || _mini == null || !_mini.IsDialogueMode) return;

		bool pressed = !RequireShift ||
					   Input.GetKeyDown(KeyCode.LeftShift) ||
					   Input.GetKeyDown(KeyCode.RightShift);

		if (pressed)
		{
			_used = true;
			_mini.StartDialogue();  // 레터박스 IN → 말풍선/텍스트 → 배경 클릭 시 종료(아웃)
			var col = GetComponent<Collider2D>();
			if (col) col.enabled = false; // 재진입 방지(테스트 끝나면 옵션화 가능)
			enabled = false;
		}
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag(PlayerTag)) _playerInside = true;
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag(PlayerTag)) _playerInside = false;
	}
}
