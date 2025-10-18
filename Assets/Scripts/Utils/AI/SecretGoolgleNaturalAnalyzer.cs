using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;

public class SentimentAnalysisManager : MonoBehaviour
{
    public string TestText = "";

    // 예제 테스트용
    async void Start()
    {
        // 씬이 시작될 때 "I love this game!" 텍스트 분석 시도
        await AnalyzeText("I love this game!", "en");

        // 씬이 시작될 때 "이 게임 정말 싫어." 텍스트 분석 시도
        await AnalyzeText(TestText, "ko");
    }


    // 테스트용 UI에서 호출할 수 있도록 public으로 선언
    public async Task AnalyzeText(string textToAnalyze, string languageCode)
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await InitializeAndSignInAsync();
            }

            Debug.Log($"분석 시작 '{textToAnalyze}'");

            var args = new Dictionary<string, object>
            {
                { "text", textToAnalyze },
                { "language", languageCode }
            };

            string responseJson = await CloudCodeService.Instance.CallModuleEndpointAsync<string>(
                "Language",
                "AnalyzeSentiment",
                args
            );

            if (string.IsNullOrEmpty(responseJson))
            {
                Debug.LogWarning("어딘가 이상하니 서버를 확인해 보자");
            }
            else
            {
                Debug.Log($"데이터 얻기 성공");

                SentimentResponse responseData = JsonUtility.FromJson<SentimentResponse>(responseJson);
                Debug.Log($"Sentiment Score: {responseData.documentSentiment.score}, Magnitude: {responseData.documentSentiment.magnitude}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"분석 실패: {e.Message}");
        }
    }

    private async Task InitializeAndSignInAsync()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("익명 로그인");
        }
    }
}