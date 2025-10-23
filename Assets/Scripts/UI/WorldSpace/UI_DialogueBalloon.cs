using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DialogueBalloon : UI_Base
{
	[SerializeField] RectTransform root;   // 말풍선 루트
	[SerializeField] TMP_Text label;
	[SerializeField] CanvasGroup cg;
	[SerializeField] Vector2 screenOffset = new Vector2(0, 80f);

	Transform _anchor;

	public void Init(Transform anchor)
	{
		_anchor = anchor;
		cg.alpha = 0f;
	}

	void LateUpdate()
	{
		if (_anchor == null) return;
		var cam = Camera.main;
		var baseWorld = cam.ScreenToWorldPoint(Vector3.zero);
		var offWorld = cam.ScreenToWorldPoint(new Vector3(screenOffset.x, screenOffset.y, 0f)) - baseWorld;

		var pos = _anchor.position + offWorld;
		pos.z = _anchor.position.z; // 2D면 z 고정
		root.position = pos;
	}


	public IEnumerator CoPresent(string text, float typeSpeed = 0.03f)
	{
		// 페이드 인
		yield return cg.FadeCanvas(1f, 0.15f);

		// 간단 타자 효과 (원하면 Febucci 붙여도 됨)
		label.text = "";
		foreach (var ch in text)
		{
			label.text += ch;
			yield return WaitForSecondsCache.Get(typeSpeed);
			// 클릭시 스킵
			if (Input.GetMouseButtonDown(0)) { label.text = text; break; }
		}

		// 전체 출력 후 클릭/Space 대기
		while (!Input.GetMouseButtonDown(0) && !Input.GetKeyDown(KeyCode.Space))
			yield return null;

		// 페이드 아웃
		yield return cg.FadeCanvas(0f, 0.12f);
	}

	public override void Init()
	{
		
	}
}
