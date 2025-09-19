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
	{ "<b>�������� �ű� ���� �ž�?</b>", "<b>�װ����� ���� ������ �ʴ´ٸ�</b>", "<b>�� �ᱹ</b>", "<b><color=red>!@($#@!!@�ϰ� �� �ž�.</color></b>" };

	IEnumerator Start()
    {	
		Managers.UI.CloseAllPopupUI();
		FindAnyObjectByType<HappinessHUD>().gameObject.SetActive(false);

		Animator animator = GetComponent<Animator>();
		animator.speed = 0f;
		yield return _fadeImage.CoFadeIn(0f);
		yield return WaitForSecondsCache.Get(2f);	// ȭ�� �� ����� �� �ش� ȭ�� 2�� ����
		animator.speed = 1f;						// ���� ������ �ִϸ��̼� & ���� (����� �ִϸ��̼� �̺�Ʈ)
    }

	public void OpenDoorAnimationEvent()            // ���� ������ ������ ����(�ִϸ��̼� ������ ����)�� �߻��ϴ� �ִϸ��̼� �̺�Ʈ
	{
		StartCoroutine(DoorCutSceneSequence());
	}

	public void OpenDoorSoundEvent()                // ���� ������ ���� �߻��ϴ� �̺�Ʈ
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

		// �ȴ� �Ҹ��� ���鼭 ���ΰ��� ���ʿ��� �ɾ��
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

		anim.SetFloat("Speed", 0f);                          // ���ΰ��� �� �߾ӿ� ��ġ�ϸ� �̵��� ����

		yield return WaitForSecondsCache.Get(2f);            // 2�ʵ��� �� ���� ���� �� ���� ȭ��

		GameObject go = Managers.Resource.Instantiate("UI/WorldSpace/UI_Bubble");
		BubbleFollow bubbleFollow = go.GetComponent<BubbleFollow>();

		bubbleFollow.SetTarget(player.transform);
		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[0];
		Managers.Sound.Play("s1_say_impact2", Sound.Effect);
		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f;  // ��ǳ�� �ƿ�

		yield return WaitForSecondsCache.Get(3f);  // ��ǳ�� �ƿ��ǰ� 3�� ���� ��

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 0f; // ��簡 ������� ���ÿ�
		Vector3 playerPos = player.transform.position;
		playerPos.y = -3.5f;
		player.transform.localScale = new Vector3(4.5f, 4.5f, 1f);
		player.transform.position = playerPos;

		yield return WaitForSecondsCache.Get(1f); // �׸��� ��¦ �� �ְ�

		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[1];
		bubbleFollow.Offset += new Vector3(0f, 3.0f, 0f);
		Managers.Sound.Play("s1_say_impact2", Sound.Effect);
		bubbleFollow.transform.localScale *= 1.4f;


		Image bubbleImage = bubbleFollow.GetComponentInChildren<Image>();
		RectTransform rectTransform = bubbleImage.GetComponent<RectTransform>();
		rectTransform.sizeDelta = new Vector2(2.88f, rectTransform.sizeDelta.y);

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f; // ��� �ƿ�

		yield return WaitForSecondsCache.Get(3f); // ��ǳ�� �ƿ��ǰ� 3�� ���� ��

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 0f; // ��簡 ������� ���ÿ�
		player.transform.localScale = new Vector3(8f, 8f, 1f); // ĳ���� Ŀ����
		player.transform.position += new Vector3(0, -1.5f, 0f); // �������

		yield return WaitForSecondsCache.Get(1f); // �׸��� ��¦ �� �ְ�

		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[2];
		bubbleFollow.Offset += new Vector3(0f, 2.7f, 0f);
		Managers.Sound.Play("s1_say_impact2", Sound.Effect);
		bubbleFollow.transform.localScale *= 1.4f;

		rectTransform.sizeDelta = new Vector2(1.18f, rectTransform.sizeDelta.y);

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f; // ��� �ƿ�

		yield return WaitForSecondsCache.Get(3f); // ��ǳ�� �ƿ��ǰ� 3�� ���� ��

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 0f; // ��簡 ������� ���ÿ�
		player.transform.localScale = new Vector3(13f, 13f, 13f); // ĳ���Ͱ� Ŀ����
		playerPos = player.transform.position;
		playerPos.y = -9.69f;
		player.transform.position = playerPos; // �������

		yield return WaitForSecondsCache.Get(1f); // ��¦ ��

		bubbleFollow.GetComponentInChildren<TMP_Text>().text = _lines[3];
		bubbleFollow.Offset += new Vector3(0f, 4.3f, 0f);
		Managers.Sound.Play("getout_long", Sound.Effect);
		bubbleFollow.transform.localScale *= 1.2f;

		rectTransform.sizeDelta = new Vector2(2.4f, rectTransform.sizeDelta.y);

		bubbleFollow.GetComponent<CanvasGroup>().alpha = 1f; // ��� �ƿ�

		yield return WaitForSecondsCache.Get(3f);
		Managers.Scene.LoadScene(Scene.GameScene);
	}
}