using System.Collections;
using UnityEngine;

public class WaitForInputCommand : IDialogueCommand
{
    private bool _useAIInput;
    private SecretSentimentAnalyer _secretSentimentAnalyer;
    public WaitForInputCommand(bool useAIInput = false)
    {
        _useAIInput = useAIInput;
        if (_useAIInput)
        {
            _secretSentimentAnalyer = new SecretSentimentAnalyer();
        }
    }

    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        string userText = "";
        yield return context.InputPrompt?.Prompt(row.script, s => userText = s);

        string branchType = "Ambiguous";

        if (_useAIInput && !string.IsNullOrWhiteSpace(userText))
        {
            float sentimentScore = 0f;

            context.TextPresenter.ClearAllStacked();

            var ballon = context.TextPresenter.ShowTextTemp(row.speaker, "...", "");

            float checkTime = Time.time;

            // Task를 코루틴으로 래핑해서 대기
            yield return CoroutineUtil.RunTask(
                _secretSentimentAnalyer.AnalyzeText(userText + " " + userText, "ko"),
                result => sentimentScore = result
            );

            float elapsedTime = Time.time - checkTime;
            if(elapsedTime < 2f)
            {
                yield return WaitForSecondsCache.Get(2f - elapsedTime);
            }

            UnityEngine.GameObject.Destroy(ballon.gameObject);

            
            if (sentimentScore == SecretSentimentAnalyer.WARNING_SCORE)
            {
                branchType = "Ambiguous";
            }
            else if (sentimentScore > 0.5f) branchType = "Positive";
            else if (sentimentScore < -0.5f) branchType = "Negative";
            else branchType = "Ambiguous";
        }

        var nextID = context.BranchTable?.Resolve(row.eventParam?.Trim(), branchType);
        
        context.TextPresenter.ClearAllStacked();

        context.NextID = nextID;
    }
}