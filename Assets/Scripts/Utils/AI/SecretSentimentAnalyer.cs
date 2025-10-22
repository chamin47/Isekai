using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.CloudCode;
using Unity.Services.Core;
using UnityEngine;

internal class SecretSentimentAnalyer
{
    public static readonly float WARNING_SCORE = -100f;
    public string TestText = "";

    // UI에서 호출할 수 있도록 public으로 선언
    public async Task<float> AnalyzeText(string textToAnalyze, string languageCode)
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

            SentimentResponse responseJson = await CloudCodeService.Instance.CallModuleEndpointAsync<SentimentResponse>(
                "Language",
                "AnalyzeSentiment",
                args
            );

            if (responseJson == null)
            {
                Debug.LogWarning("어딘가 이상하니 서버를 확인해 보자");
                return WARNING_SCORE;
            }
            
            Debug.Log($"데이터 얻기 성공");
            Debug.Log($"Sentiment Score: {responseJson.documentSentiment.score}, Magnitude: {responseJson.documentSentiment.magnitude}");
            
            return responseJson.documentSentiment.score;
        }
        catch (Exception e)
        {
            Debug.LogError($"분석 실패: {e.Message}");
            return WARNING_SCORE;
        }
    }

    private async Task InitializeAndSignInAsync()
    {
        // TODO: 로그인 기능이 생길시 확장
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("익명 로그인");
        }
    }
}