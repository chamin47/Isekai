using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalendarHintController
{
	private readonly Image _smallLeft;
	private readonly Image _smallRight;
	private readonly Image _bigLeft;
	private readonly Image _bigRight;

	private bool _paused;

	public void Pause() => _paused = true;
	public void Resume() => _paused = false;

	public CalendarHintController(Image smallLeft, Image smallRight, Image bigLeft, Image bigRight)
	{
		_smallLeft = smallLeft;
		_smallRight = smallRight;
		_bigLeft = bigLeft;
		_bigRight = bigRight;
	}

	public IEnumerator CoHintLoop()
	{
		// 1분 대기
		yield return new WaitForSeconds(60f);

		while (!CalendarInputModel.IsSolved)
		{
			if (_paused)
			{
				yield return null;
				continue;
			}

			// 2와 0만 점멸 (총 4회)
			for (int i = 0; i < 4; i++)
			{
				if (CalendarInputModel.IsSolved)    // 해결 시 즉시 종료
					yield break;

				SetVisible(false);
				Managers.Sound.Play("mini_calendar_hint_flicker", Sound.Effect);
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
		c = _smallLeft.color;
		c.a = alpha;
		_smallLeft.color = c;

		c = _smallRight.color;
		c.a = alpha;
		_smallRight.color = c;

		c = _bigLeft.color;
		c.a = alpha;
		_bigLeft.color = c;

		c = _bigRight.color;
		c.a = alpha;
		_bigRight.color = c;
	}

	public void Reset()
	{
		
	}
}
