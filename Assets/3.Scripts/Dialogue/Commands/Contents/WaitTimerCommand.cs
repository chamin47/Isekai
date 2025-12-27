using System.Collections;
using UnityEngine;

public class WaitTimerCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        var time = ParamParser.ToFloat(row.eventParam?.Trim(), 0f);
        if (time > 0)
            yield return WaitForSecondsCache.Get(time);

        context.NextID = row.nextID;
    }
}