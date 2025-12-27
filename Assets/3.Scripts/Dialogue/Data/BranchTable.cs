using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "BranchTable", menuName = "Dialogue/BranchTable")]
public class BranchTable : ScriptableObject
{
	[System.Serializable]
	public class Row 
	{ 
		public string BranchID; 
		public string InputType; 
		public string ResultDialogueID; 
	}

	public List<Row> rows = new();

	public string Resolve(string branchId, string inputType)
	{
		return rows.LastOrDefault(r => r.BranchID == branchId && r.InputType == inputType)?.ResultDialogueID;
	}
}
