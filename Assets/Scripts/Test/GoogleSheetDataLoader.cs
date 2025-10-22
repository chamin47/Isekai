using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class GoogleSheetDataLoader : EditorWindow
{
    // 1단계에서 복사한 구글 시트 CSV 게시 링크를 여기에 붙여넣으세요.
    private const string SHEET_URL = "https://docs.google.com/spreadsheets/d/e/2PACX-1vSu3maz3r9J5g6g_Zu-i_YbVZ44nTIdZV7oHSUad8-sTfwtsrgUXmb4_IsbcncK-kMXd50_3rX49osN/pub?output=csv";

    // 데이터를 저장할 경로
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
                // 웹에서 CSV 데이터 다운로드
                string csvData = await client.GetStringAsync(SHEET_URL);

                // 데이터 파싱 및 ScriptableObject 생성
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
        // 저장 경로가 없으면 생성
        if (!Directory.Exists(SAVE_PATH))
        {
            Directory.CreateDirectory(SAVE_PATH);
        }

        // CSV 데이터를 줄 단위로 나눔
        string[] lines = csvData.Split(new[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

        // 첫 번째 줄(헤더)은 건너뛰고 두 번째 줄부터 시작
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = ParseCsvLine(lines[i]);

            // 데이터가 충분하지 않은 줄은 건너뜀
            if (values.Length < 8) continue;

            // ScriptableObject 인스턴스 생성
            DialogueData data = ScriptableObject.CreateInstance<DialogueData>();

            // 각 변수에 데이터 할당
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

			// 파일 이름은 고유한 ID로 지정.
			string assetPath = $"{SAVE_PATH}{data.id}.asset";
            AssetDatabase.CreateAsset(data, assetPath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Data import complete! {lines.Length - 1} assets created at {SAVE_PATH}");
    }

    // 쉼표로 구분된 라인을 파싱하는 간단한 함수 (따옴표 안에 쉼표가 있는 경우 처리)
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