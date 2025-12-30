using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_AIResponse : MonoBehaviour
{
    private SecretSentimentAnalyer analyzer;
    [SerializeField] private TextMeshProUGUI resultText;
    [SerializeField] private TMP_InputField inputField;

    private void Start()
    {
        analyzer = new SecretSentimentAnalyer();
        inputField.onSubmit.AddListener(Response);
    }

    public async void Response(string input)
    {
        resultText.text = "분석중....";
        float result = await analyzer.AnalyzeText(input, "ko");
        
        Debug.Log("분석 완료");
        if (result == SecretSentimentAnalyer.WARNING_SCORE)
        {
            resultText.text = "음 그렇구나";
        }
        else
        {
            string autoResponse = AutoResponse.GetResponse(result);
            resultText.text = autoResponse;
        }
    }
}

