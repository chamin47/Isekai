using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

using System;

// Google API 요청 본문을 위한 클래스
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

// Google API 응답을 받기 위한 클래스
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
    // UI 요소 연결
    public TMP_InputField chatInputField;
    public TMP_Text resultText;
    public TMP_Text scoreText;

    private string apiKey = "AIzaSyA9HG7PWq93f7pFs69FRfhRULQhjQZaAgs"; // 1단계에서 발급받은 API 키
    private string apiUrl = "https://language.googleapis.com/v2/documents:analyzeSentiment?key=";
    private int playerScore = 0;

    private string currentLanguage = "ko";

    public string TestText = "나는 너무 행복해";

    void Start()
    {
        // Enter 키를 눌렀을 때 이벤트에 함수 연결
        chatInputField.onSubmit.AddListener(OnSubmitChat);
        UpdateScoreText();
    }

    private void OnSubmitChat(string chatText)
    {
        if (string.IsNullOrEmpty(chatText)) return;

        // 코루틴을 통해 API 요청 시작
        StartCoroutine(AnalyzeSentiment(chatText));
        chatInputField.text = ""; // 입력 필드 초기화
    }

    private IEnumerator AnalyzeSentiment(string text)
    {
        // 1. 요청 데이터 생성
        SentimentRequest requestData = new SentimentRequest
        {
            encodingType = EncodingType.UTF8,
            document = new Document
            {
                type = "PLAIN_TEXT",
                content = text,
                languageCode = currentLanguage // 현재 언어 설정
            }
        };

        string jsonData = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

        // 2. UnityWebRequest 생성 및 설정
        using (UnityWebRequest request = new UnityWebRequest(apiUrl + apiKey, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            resultText.text = "분석 중...";

            // 3. API 요청 전송
            yield return request.SendWebRequest();

            // 4. 결과 처리
            if (request.result == UnityWebRequest.Result.Success)
            {
                string responseJson = request.downloadHandler.text;
                SentimentResponse responseData = JsonUtility.FromJson<SentimentResponse>(responseJson);

                ProcessSentimentResult(responseData.documentSentiment);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                resultText.text = "오류 발생: " + request.error;
            }
        }
    }

    private void ProcessSentimentResult(DocumentSentiment sentiment)
    {
        float score = sentiment.score;
        float magnitude = sentiment.magnitude;

        string result = $"감정 점수: {score:F2}, 강도: {magnitude:F2}\n";

        // 감정 점수(score)에 따라 점수 부여
        if (score > 0.25f)
        {
            result += "결과: 긍정적 채팅! (+10점)";
            playerScore += 10;
        }
        else if (score < -0.25f)
        {
            result += "결과: 부정적 채팅! (-5점)";
            playerScore -= 5;
        }
        else
        {
            result += "결과: 중립적 채팅. (점수 변동 없음)";
        }

        resultText.text = result;
        UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "플레이어 점수: " + playerScore;
    }
}