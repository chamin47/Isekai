using System.Collections;
using UnityEngine;

public class ModifyHappyGaugeCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        float amount = ParamParser.ToFloat(row.eventParam?.Trim(), 0f);
        if (context.Happiness != null)
        {
            context.Happiness.Appear();
            context.Happiness.ChangeHappiness(amount);
            Managers.Sound.Play("happiness_gauge", Sound.Effect);
        }

        context.NextID = row.nextID;
        yield break;
    }
}