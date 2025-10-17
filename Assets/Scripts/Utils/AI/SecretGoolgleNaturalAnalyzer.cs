//using UnityEngine;
//using Unity.Services.Core;
//using Unity.Services.Authentication;
//using Unity.Services.CloudCode;
//using System.Threading.Tasks;
//using System;

//public class SentimentAnalysisManager : MonoBehaviour
//{
//    // �׽�Ʈ�� UI���� ȣ���� �� �ֵ��� public���� ����
//    public async void AnalyzeText(string textToAnalyze, string languageCode)
//    {
//        try
//        {
//            // 1. UGS�� �ʱ�ȭ���� �ʾҴٸ� �ʱ�ȭ �� �͸� �α���
//            if (UnityServices.State != ServicesInitializationState.Initialized)
//            {
//                await InitializeAndSignInAsync();
//            }

//            // 2. Cloud Code�� ������ �Ķ���� �غ�
//            //    Cloud Code �Լ��� �Ķ���� �̸�(text, language)�� ��Ȯ�� ��ġ�ؾ� �մϴ�.
//            var args = new
//            {
//                text = textToAnalyze,
//                language = languageCode
//            };

//            Debug.Log($"Calling AnalyzeSentiment with text: '{textToAnalyze}'");

//            // 3. Cloud Code ��������Ʈ ȣ��
//            //    C# ����� ��ȯ Ÿ���� string(JSON)�̹Ƿ� ���׸� Ÿ������ <string>�� ����մϴ�.
//            string responseJson = await CloudCodeService.Instance.CallEndpointAsync<string>(
//                "AnalyzeSentiment", // [CloudCodeFunction("...")]�� ������ �̸�
//                args
//            );

//            // 4. ��� ó��
//            if (string.IsNullOrEmpty(responseJson))
//            {
//                Debug.LogWarning("Cloud Code returned null or empty. Check Cloud Code logs for errors (e.g., API key issue).");
//            }
//            else
//            {
//                Debug.Log($"Successfully received sentiment data (Raw JSON):\n{responseJson}");

//                // TODO: �� responseJson�� �Ľ��Ͽ� ���� ������(���� ���� ��)�� ����մϴ�.
//                // (�Ʒ� "���� �ܰ�: ���� �Ľ�" ����)
//            }
//        }
//        catch (CloudCodeException e)
//        {
//            // Cloud Code ���� ��ü���� �߻��� ���� (��: �Լ��� �� ã��, Ÿ�Ӿƿ� ��)
//            Debug.LogError($"Cloud Code Error: {e.Message}\nDetails: {e.Reason}");
//        }
//        catch (Exception e)
//        {
//            // ��Ÿ �Ϲ� ���� (��Ʈ��ũ ���� ��)
//            Debug.LogError($"An error occurred: {e.Message}");
//        }
//    }

//    private async Task InitializeAndSignInAsync()
//    {
//        await UnityServices.InitializeAsync();
//        if (!AuthenticationService.Instance.IsSignedIn)
//        {
//            await AuthenticationService.Instance.SignInAnonymouslyAsync();
//            Debug.Log("UGS Initialized and Signed In Anonymously.");
//        }
//    }

//    // ���� �׽�Ʈ��
//    void Start()
//    {
//        // ���� ���۵� �� "I love this game!" �ؽ�Ʈ �м� �õ�
//        AnalyzeText("I love this game!", "en");

//        // ���� ���۵� �� "�� ���� ���� �Ⱦ�." �ؽ�Ʈ �м� �õ�
//        AnalyzeText("�� ���� ���� �Ⱦ�.", "ko");
//    }
//}