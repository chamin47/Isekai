using TMPro;
using System.Collections;
using UnityEngine;

public class CalendarHintController
{
	private readonly TMP_Text _left;
	private readonly TMP_Text _right;

	public CalendarHintController(TMP_Text left, TMP_Text right)
	{
		_left = left;
		_right = right;
	}

	public IEnumerator CoHintLoop()
	{
		while (true)
		{
			// 1분 대기
			yield return new WaitForSeconds(60f);

			// 2와 0만 점멸 (총 4회)
			for (int i = 0; i < 4; i++)
			{
				SetVisible(false);
				yield return new WaitForSeconds(1f);
				SetVisible(true);
				yield return new WaitForSeconds(1f);
			}

			// 30초 대기 후 다시 힌트
			yield return new WaitForSeconds(30f);
		}
	}

	private void SetVisible(bool visible)
	{
		float alpha = visible ? 1f : 0f;

		Color c;

		c = _left.color;
		c.a = alpha;
		_left.color = c;

		c = _right.color;
		c.a = alpha;
		_right.color = c;
	}

	public void Reset()
	{
		SetVisible(false);
	}
}
