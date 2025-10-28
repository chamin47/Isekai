using System.Linq;
using UnityEngine;

/// <summary>
/// Ű���� ��� ���� �з���.
/// �Է� ���ڿ��� Positive/Negative/Ambiguous�� �±��Ѵ�.
/// ���� ai ��� ���� �м����� ��ü�� ��.
/// </summary>
public class KeywordBranchResolver : MonoBehaviour, IBranchResolver
{
	[Header("���� Ű���� �з� ����")]
	public string[] positives = new[] { "�ູ", "��", "yes", "y", "��", "�׷�" };
	public string[] negatives = new[] { "����", "��", "no", "n", "�ƴ�", "�Ƴ�" };

	public string Classify(string userInput)
	{
		var s = (userInput ?? "").ToLowerInvariant();
		if (positives.Any(k => s.Contains(k))) return "Positive";
		if (negatives.Any(k => s.Contains(k))) return "Negative";
		return "Ambiguous";
	}
}
