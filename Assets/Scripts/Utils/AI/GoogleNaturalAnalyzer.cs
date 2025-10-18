using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

using System;

// Google API ��û ������ ���� Ŭ����
[Serializable]
public class SentimentRequest
{
    public Document document;
    public EncodingType encodingType;
}

[Serializable]
public class Document
{
    public string type;
    public string content;
    public string languageCode;
}

public enum EncodingType
{
    NONE,
    UTF8,
    UTF16,
    UTF32
}

// Google API ������ �ޱ� ���� Ŭ����
[Serializable]
public class SentimentResponse
{
    public DocumentSentiment documentSentiment;
}

[Serializable]
public class DocumentSentiment
{
    public float score;
    public float magnitude;
}

public class GoogleNaturalAnalyzer : MonoBehaviour
{
    // UI ��� ����
    public TMP_InputField chatInputField;
    public TMP_Text resultText;
    public TMP_Text scoreText;

    private string apiKey = "AIzaSyA9HG7PWq93f7pFs69FRfhRULQhjQZaAgs"; // 1�ܰ迡�� �߱޹��� API Ű
    private string apiUrl = "https://language.googleapis.com/v2/documents:analyzeSentiment?key=";
    private int playerScore = 0;

    private string currentLanguage = "ko";

    public string TestText = "���� �ʹ� �ູ��";

    void Start()
    {
        // Enter Ű�� ������ �� �̺�Ʈ�� �Լ� ����
        chatInputField.onSubmit.AddListener(OnSubmitChat);
        UpdateScoreText();
    }

    private void OnSubmitChat(string chatText)
    {
        if (string.IsNullOrEmpty(chatText)) return;

        // �ڷ�ƾ�� ���� API ��û ����
        StartCoroutine(AnalyzeSentiment(chatText));
        chatInputField.text = ""; // �Է� �ʵ� �ʱ�ȭ
    }

    private IEnumerator AnalyzeSentiment(string text)
    {
        // 1. ��û ������ ����
        SentimentRequest requestData = new SentimentRequest
        {
            encodingType = EncodingType.UTF8,
            document = new Document
            {
                type = "PLAIN_TEXT",
                content = text,
                languageCode = currentLanguage // ���� ��� ����
            }
        };

        string jsonData = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // 2. UnityWebRequest ���� �� ����
        using (UnityWebRequest request = new UnityWebRequest(apiUrl + apiKey, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            resultText.text = "�м� ��...";

            // 3. API ��û ����
            yield return request.SendWebRequest();

            // 4. ��� ó��
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                SentimentResponse responseData = JsonUtility.FromJson<SentimentResponse>(responseJson);

                ProcessSentimentResult(responseData.documentSentiment);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                resultText.text = "���� �߻�: " + request.error;
            }
        }
    }

    private void ProcessSentimentResult(DocumentSentiment sentiment)
    {
        float score = sentiment.score;
        float magnitude = sentiment.magnitude;

        string result = $"���� ����: {score:F2}, ����: {magnitude:F2}\n";

        // ���� ����(score)�� ���� ���� �ο�
        if (score > 0.25f)
        {
            result += "���: ������ ä��! (+10��)";
            playerScore += 10;
        }
        else if (score < -0.25f)
        {
            result += "���: ������ ä��! (-5��)";
            playerScore -= 5;
        }
        else
        {
            result += "���: �߸��� ä��. (���� ���� ����)";
        }

        resultText.text = result;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "�÷��̾� ����: " + playerScore;
    }
}