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

	public static (float scale, float dur, string anchor) Zoom3(string param, float dScale, float dDur, string dAnchor)
	{
		if (string.IsNullOrWhiteSpace(param)) return (dScale, dDur, dAnchor);
		// NBSP 제거 및 트림
		var clean = param.Replace('\u00A0', ' ').Trim();
		var parts = clean.Split(',');
		float s = dScale, t = dDur;
		string a = dAnchor;

		if (parts.Length >= 1 && float.TryParse(parts[0].Trim(), NumberStyles.Float, Invariant, out var s0)) s = s0;
		if (parts.Length >= 2 && float.TryParse(parts[1].Trim(), NumberStyles.Float, Invariant, out var t0)) t = t0;
		if (parts.Length >= 3) a = parts[2].Trim();

		return (s, t, a);
	}

	public static float? NullableFloat(string param)
	{
		if (string.IsNullOrWhiteSpace(param)) 
			return null;

		// NBSP 제거 + 트림
		var s = param.Replace('\u00A0', ' ').Trim();

		// "null" 문자열은 명시적 null
		if (s.Equals("null", StringComparison.OrdinalIgnoreCase)) 
			return null;

		// 단일 값만 파싱
		if (float.TryParse(s, NumberStyles.Float, Invariant, out var v))
			return v;

		return null;
	}
}
