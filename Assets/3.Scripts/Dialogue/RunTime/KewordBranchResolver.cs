using System.Linq;
using UnityEngine;

/// <summary>
/// 키워드 기반 간단 분류기.
/// 입력 문자열을 Positive/Negative/Ambiguous로 태그한다.
/// 추후 ai 기반 감정 분석으로 대체할 것.
/// </summary>
public class KeywordBranchResolver : MonoBehaviour, IBranchResolver
{
	[Header("간단 키워드 분류 예시")]
	public string[] positives = new[] { "행복", "좋", "yes", "y", "응", "그래" };
	public string[] negatives = new[] { "불행", "싫", "no", "n", "아니", "아냐" };

	public string Classify(string userInput)
	{
		var s = (userInput ?? "").ToLowerInvariant();
		if (positives.Any(k => s.Contains(k))) return "Positive";
		if (negatives.Any(k => s.Contains(k))) return "Negative";
		return "Ambiguous";
	}
}
