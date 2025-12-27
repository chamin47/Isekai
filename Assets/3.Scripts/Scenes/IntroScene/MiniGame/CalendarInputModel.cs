public static class CalendarInputModel
{
	// 0000 00 00
	public static int[] Digits = new int[8];

	public static bool IsSolved = false;

	public static string GetDateString()
	{
		return $"{Digits[0]}{Digits[1]}{Digits[2]}{Digits[3]}." +
			   $"{Digits[4]}{Digits[5]}." +
			   $"{Digits[6]}{Digits[7]}";
	}

	public static bool IsCorrect()
	{
		return GetDateString() == "2035.12.24";
	}
}
