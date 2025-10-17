//using UnityEngine;
//using Unity.Services.Core;
//using Unity.Services.Authentication;
//using Unity.Services.CloudCode;
//using System.Threading.Tasks;
//using System;

//public class SentimentAnalysisManager : MonoBehaviour
//{
//    // 테스트용 UI에서 호출할 수 있도록 public으로 선언
//    public async void AnalyzeText(string textToAnalyze, string languageCode)
//    {
//        try
//        {
//            // 1. UGS가 초기화되지 않았다면 초기화 및 익명 로그인
//            if (UnityServices.State != ServicesInitializationState.Initialized)
//            {
//                await InitializeAndSignInAsync();
//            }

//            // 2. Cloud Code에 전달할 파라미터 준비
//            //    Cloud Code 함수의 파라미터 이름(text, language)과 정확히 일치해야 합니다.
//            var args = new
//            {
//                text = textToAnalyze,
//                language = languageCode
//            };

//            Debug.Log($"Calling AnalyzeSentiment with text: '{textToAnalyze}'");

//            // 3. Cloud Code 엔드포인트 호출
//            //    C# 모듈의 반환 타입이 string(JSON)이므로 제네릭 타입으로 <string>을 사용합니다.
//            string responseJson = await CloudCodeService.Instance.CallEndpointAsync<string>(
//                "AnalyzeSentiment", // [CloudCodeFunction("...")]에 지정한 이름
//                args
//            );

//            // 4. 결과 처리
//            if (string.IsNullOrEmpty(responseJson))
//            {
//                Debug.LogWarning("Cloud Code returned null or empty. Check Cloud Code logs for errors (e.g., API key issue).");
//            }
//            else
//            {
//                Debug.Log($"Successfully received sentiment data (Raw JSON):\n{responseJson}");

//                // TODO: 이 responseJson을 파싱하여 실제 데이터(감정 점수 등)를 사용합니다.
//                // (아래 "다음 단계: 응답 파싱" 참고)
//            }
//        }
//        catch (CloudCodeException e)
//        {
//            // Cloud Code 실행 자체에서 발생한 예외 (예: 함수를 못 찾음, 타임아웃 등)
//            Debug.LogError($"Cloud Code Error: {e.Message}\nDetails: {e.Reason}");
//        }
//        catch (Exception e)
//        {
//            // 기타 일반 예외 (네트워크 문제 등)
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

//    // 예제 테스트용
//    void Start()
//    {
//        // 씬이 시작될 때 "I love this game!" 텍스트 분석 시도
//        AnalyzeText("I love this game!", "en");

//        // 씬이 시작될 때 "이 게임 정말 싫어." 텍스트 분석 시도
//        AnalyzeText("이 게임 정말 싫어.", "ko");
//    }
//}