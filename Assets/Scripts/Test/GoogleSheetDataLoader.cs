using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class GoogleSheetDataLoader : EditorWindow
{
    // 1�ܰ迡�� ������ ���� ��Ʈ CSV �Խ� ��ũ�� ���⿡ �ٿ���������.
    private const string SHEET_URL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSu3maz3r9J5g6g_Zu-i_YbVZ44nTIdZV7oHSUad8-sTfwtsrgUXmb4_IsbcncK-kMXd50_3rX49osN/pub?output=csv";

    // �����͸� ������ ���
    private const string SAVE_PATH = "Assets/Resources/DB";

    [MenuItem("Loader/Import Dialogue Data")]
    public static void ShowWindow()
    {
        GetWindow<GoogleSheetDataLoader>("Dialogue Importer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Import dialogue data from Google Sheet", EditorStyles.boldLabel);
        if (GUILayout.Button("Import"))
        {
            ImportData();
        }
    }

    private static async void ImportData()
    {
        Debug.Log("Starting data import...");

        using (HttpClient client = new HttpClient())
        {
            try
            {
                // ������ CSV ������ �ٿ�ε�
                string csvData = await client.GetStringAsync(SHEET_URL);

                // ������ �Ľ� �� ScriptableObject ����
                CreateScriptableObjects(csvData);
            }
            catch (HttpRequestException e)
            {
                Debug.LogError($"Error fetching data: {e.Message}");
            }
        }
    }

    private static void CreateScriptableObjects(string csvData)
    {
        // ���� ��ΰ� ������ ����
        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
        }

        // CSV �����͸� �� ������ ����
        string[] lines = csvData.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // ù ��° ��(���)�� �ǳʶٰ� �� ��° �ٺ��� ����
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = ParseCsvLine(lines[i]);

            // �����Ͱ� ������� ���� ���� �ǳʶ�
            if (values.Length < 8) continue;

            // ScriptableObject �ν��Ͻ� ����
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();

            // �� ������ ������ �Ҵ�
            data.id = values[0];
            data.speaker = values[1];
            data.animName = values[2];
            data.eventName = values[3];
            data.eventParam = values[4];
            data.nextID = values[5];
            //data.nextFalseID = values[6];
            //data.script = values[7];
            data.script = values[6];
			data.script = values[7];

			// ���� �̸��� ������ ID�� ����.
			string assetPath = $"{SAVE_PATH}{data.id}.asset";
            AssetDatabase.CreateAsset(data, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Data import complete! {lines.Length - 1} assets created at {SAVE_PATH}");
    }

    // ��ǥ�� ���е� ������ �Ľ��ϴ� ������ �Լ� (����ǥ �ȿ� ��ǥ�� �ִ� ��� ó��)
    private static string[] ParseCsvLine(string line)
    {
        var parts = new System.Collections.Generic.List<string>();
        var currentPart = new System.Text.StringBuilder();
        bool inQuotes = false;

        foreach (char c in line)
        {
            if (c == '\"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == ',' && !inQuotes)
            {
                parts.Add(currentPart.ToString());
                currentPart.Clear();
            }
            else
            {
                currentPart.Append(c);
            }
        }
        parts.Add(currentPart.ToString());
        return parts.ToArray();
    }
}