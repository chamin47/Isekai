using System.Linq;
using UnityEngine;

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
