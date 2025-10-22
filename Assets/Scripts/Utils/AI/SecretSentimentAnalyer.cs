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

    // UI���� ȣ���� �� �ֵ��� public���� ����
    public async Task<float> AnalyzeText(string textToAnalyze, string languageCode)
    {
        try
        {
            if (UnityServices.State != ServicesInitializationState.Initialized)
            {
                await InitializeAndSignInAsync();
            }

            Debug.Log($"�м� ���� '{textToAnalyze}'");

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
                Debug.LogWarning("��� �̻��ϴ� ������ Ȯ���� ����");
                return WARNING_SCORE;
            }
            
            Debug.Log($"������ ��� ����");
            Debug.Log($"Sentiment Score: {responseJson.documentSentiment.score}, Magnitude: {responseJson.documentSentiment.magnitude}");
            
            return responseJson.documentSentiment.score;
        }
        catch (Exception e)
        {
            Debug.LogError($"�м� ����: {e.Message}");
            return WARNING_SCORE;
        }
    }

    private async Task InitializeAndSignInAsync()
    {
        // TODO: �α��� ����� ����� Ȯ��
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("�͸� �α���");
        }
    }
}