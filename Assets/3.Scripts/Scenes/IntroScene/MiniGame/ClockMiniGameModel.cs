using UnityEngine;

public static class ClockMiniGameModel
{
	public static float HourAngle = 0f;
	public static float MinuteAngle = 0f;
	public static bool IsSolved = false;
	public static bool HasSeenIntro = false;
	public static bool HasTouchedHand = false;
	public static bool HasClickedCenter = false;
	public static bool InputLocked = false;

	public static readonly int OutlineThicknessID = Shader.PropertyToID("_OutlineThickness");

	public static void Reset()
	{
		HourAngle = 0f;
		MinuteAngle = 0f;
		IsSolved = false;
		HasTouchedHand = false;
		HasClickedCenter = false;
		HasSeenIntro = false;
		InputLocked = false;
	}
}
