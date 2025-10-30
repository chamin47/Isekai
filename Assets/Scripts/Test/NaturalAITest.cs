using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NaturalAITest : MonoBehaviour
{
    private SecretSentimentAnalyer _analyzer;
    [SerializeField] private TextMeshProUGUI _resultText;
    [SerializeField] private TMP_InputField _inputField;
    [SerializeField] private Button _analyzeButton;

    private void Start()
    {
        _analyzer = new SecretSentimentAnalyer();

        // 버튼 클릭 시 분석 시작
        _analyzeButton.onClick.AddListener(OnAnalyzeClicked);
    }

    private async void OnAnalyzeClicked()
    {
        string input = _inputField.text;
        if (string.IsNullOrEmpty(input))
        {
            _resultText.text = "텍스트를 입력하세요.";
            return;
        }

        _resultText.text = "분석 중...";
        float score = await _analyzer.AnalyzeText(input, "ko");

        if (score == SecretSentimentAnalyer.WARNING_SCORE)
        {
            _resultText.text = "서버 오류 또는 감정 분석 실패!";
        }
        else
        {
            string sentiment;
            if (score > 0.2f) sentiment = "긍정";
            else if (score < -0.2f) sentiment = "부정";
            else sentiment = "중립";

            _resultText.text = $"감정 점수: {score:F2}\n결과: {sentiment}";
        }
    }
}
