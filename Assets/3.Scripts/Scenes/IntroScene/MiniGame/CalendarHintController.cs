using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalendarHintController
{
	private readonly Image _smallLeft;
	private readonly Image _smallRight;
	private readonly Image _bigLeft;
	private readonly Image _bigRight;

	private readonly Material _smallLeftMat;
	private readonly Material _smallRightMat;

	private readonly Material _bigLeftMat;
	private readonly Material _bigRightMat;

	private const float OUTLINE_THICKNESS = 3f;
	private readonly Color OUTLINE_COLOR = new Color(1f, 1f, 1f, 1f);

	public CalendarHintController(Image smallLeft, Image smallRight, Image bigLeft, Image bigRight)
	{
		_smallLeft = smallLeft;
		_smallRight = smallRight;
		_bigLeft = bigLeft;
		_bigRight = bigRight;

		_smallLeftMat  = Object.Instantiate(_smallLeft.material);
		_smallRightMat = Object.Instantiate(_smallRight.material);
		_bigLeftMat = Object.Instantiate(_bigLeft.material);
		_bigRightMat = Object.Instantiate(_bigRight.material);

		_smallLeft.material  = _smallLeftMat;
		_smallRight.material = _smallRightMat;
		_bigLeft.material = _bigLeftMat;
		_bigRight.material = _bigRightMat;

		DisableOutline();
	}

	public IEnumerator CoHintLoop()
	{
		// 1분 대기
		yield return new WaitForSeconds(10f);

		while (!CalendarInputModel.IsSolved)
		{		
			// 2와 0만 점멸 (총 4회)
			for (int i = 0; i < 4; i++)
			{
				if (CalendarInputModel.IsSolved)    // 해결 시 즉시 종료
					yield break;

				EnableOutline();
				Managers.Sound.Play("mini_calendar_hint_flicker", Sound.Effect);
				yield return new WaitForSeconds(1f);
				DisableOutline();
				yield return new WaitForSeconds(1f);
			}

			// 30초 대기 후 다시 힌트
			yield return new WaitForSeconds(5f);
		}
	}

	private void EnableOutline()
	{
		SetOutline(_smallLeftMat, true);
		SetOutline(_smallRightMat, true);
		SetOutline(_bigLeftMat, true);
		SetOutline(_bigRightMat, true);
	}

	private void DisableOutline()
	{
		SetOutline(_smallLeftMat, false);
		SetOutline(_smallRightMat, false);
		SetOutline(_bigLeftMat, false);
		SetOutline(_bigRightMat, false);
	}

	private void SetOutline(Material mat, bool enable)
	{
		if (enable)
		{
			mat.SetFloat("_OutlineThickness", OUTLINE_THICKNESS);
			mat.SetColor("_OutlineColor", OUTLINE_COLOR);
		}
		else
		{
			mat.SetFloat("_OutlineThickness", 0f);
		}
	}

	public void Reset()
	{
		DisableOutline();
	}
}
