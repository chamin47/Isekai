using System;
using System.Globalization;
using System.Linq;

public static class ParamParser
{
	static readonly CultureInfo Invariant = CultureInfo.InvariantCulture;

	public static string[] Split(string s)
	{
		if (string.IsNullOrWhiteSpace(s))
			return Array.Empty<string>();

		return s
			.Split(',')
			.Select(x => x.Trim())
			.Where(x => x.Length > 0)
			.ToArray();
	}

	public static float ToFloat(string s, float def = 0f)
	{
		if (float.TryParse(s, NumberStyles.Float, Invariant, out var v))
			return v;

		return def;
	}

	public static int ToInt(string s, int def = 0)
	{
		if (int.TryParse(s, NumberStyles.Integer, Invariant, out var v))
			return v;

		return def;
	}

	public static (float a, float b) Floats2(string param, float da = 0, float db = 0)
	{
		var p = Split(param);
		var a = p.Length > 0 ? ToFloat(p[0], da) : da;
		var b = p.Length > 1 ? ToFloat(p[1], db) : db;
		return (a, b);
	}
}
