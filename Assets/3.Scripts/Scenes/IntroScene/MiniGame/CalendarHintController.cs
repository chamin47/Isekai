using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CalendarHintController
{
	private readonly Image _left;
	private readonly Image _right;

	private readonly Material _leftMat;
	private readonly Material _rightMat;

	private const float OUTLINE_THICKNESS = 4f;
	private readonly Color OUTLINE_COLOR = new Color(1f, 1f, 1f, 1f);

	public CalendarHintController(Image left, Image right)
	{
		_left = left;
		_right = right;

		_leftMat  = Object.Instantiate(_left.material);
		_rightMat = Object.Instantiate(_right.material);

		_left.material  = _leftMat;
		_right.material = _rightMat;

		DisableOutline();
	}

	public IEnumerator CoHintLoop()
	{
		// 1분 대기
		yield return new WaitForSeconds(60f);

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
			yield return new WaitForSeconds(30f);
		}
	}

	private void EnableOutline()
	{
		SetOutline(_leftMat, true);
		SetOutline(_rightMat, true);
	}

	private void DisableOutline()
	{
		SetOutline(_leftMat, false);
		SetOutline(_rightMat, false);
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
