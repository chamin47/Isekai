using UnityEngine;

public static class IntroRuntime
{
	public static string PlayerName { get; private set; } = "";

	// 공백 제거 + 1~4자 제한
	public static void SetName(string raw)
	{
		if (raw == null) raw = "";
		raw = raw.Trim();
		if (raw.Length > 4) raw = raw.Substring(0, 4);
		PlayerName = raw;
		PlayerPrefs.SetString("Intro_PlayerName", PlayerName);
		PlayerPrefs.Save();
	}

	public static void LoadIfEmptyFromPrefs()
	{
		if (!string.IsNullOrEmpty(PlayerName)) return;
		if (PlayerPrefs.HasKey("Intro_PlayerName"))
			PlayerName = PlayerPrefs.GetString("Intro_PlayerName");
	}

	public static bool IsValid => !string.IsNullOrEmpty(PlayerName);
}