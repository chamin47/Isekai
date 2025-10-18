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

    // ���� �׽�Ʈ��
    async void Start()
    {
        // ���� ���۵� �� "I love this game!" �ؽ�Ʈ �м� �õ�
        await AnalyzeText("I love this game!", "en");

        // ���� ���۵� �� "�� ���� ���� �Ⱦ�." �ؽ�Ʈ �м� �õ�
        await AnalyzeText(TestText, "ko");
    }


    // �׽�Ʈ�� UI���� ȣ���� �� �ֵ��� public���� ����
    public async Task AnalyzeText(string textToAnalyze, string languageCode)
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

            string responseJson = await CloudCodeService.Instance.CallModuleEndpointAsync<string>(
                "Language",
                "AnalyzeSentiment",
                args
            );

            if (string.IsNullOrEmpty(responseJson))
            {
                Debug.LogWarning("��� �̻��ϴ� ������ Ȯ���� ����");
            }
            else
            {
                Debug.Log($"������ ��� ����");

                SentimentResponse responseData = JsonUtility.FromJson<SentimentResponse>(responseJson);
                Debug.Log($"Sentiment Score: {responseData.documentSentiment.score}, Magnitude: {responseData.documentSentiment.magnitude}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"�м� ����: {e.Message}");
        }
    }

    private async Task InitializeAndSignInAsync()
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log("�͸� �α���");
        }
    }
}