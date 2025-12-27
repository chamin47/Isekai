using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "ChoiceTable", menuName = "Dialogue/ChoiceTable")]
public class ChoiceTable : ScriptableObject
{
	[System.Serializable] 
	public class Option 
	{ 
		public string text; 
		public string nextID; 
	}

	[System.Serializable]
	public class ChoiceRow
	{
		public string ChoiceID;
		public List<Option> options = new(); // 몇 개든 확장 가능
	}

	public List<ChoiceRow> choices = new();

	public ChoiceRow Get(string id)
	{
		return choices.FirstOrDefault(c => c.ChoiceID == id);
	}
}
