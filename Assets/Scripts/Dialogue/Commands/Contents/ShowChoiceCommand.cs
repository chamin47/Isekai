using System.Collections;
using UnityEngine;

public class ShowChoiceCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        var choice = context.ChoiceTable?.Get(row.eventParam?.Trim());
        if (choice == null)
        {
            Debug.LogError($"Choice not found: {row.eventParam}");
            context.NextID = row.nextID;
            yield break;
        }

        var player = context.Player;
        bool isLeft = player.transform.position.x < context.ActorDirector.transform.position.x;
        
        Vector3 offset = isLeft ? new Vector3(-1.9f, 1.0f, 0) : new Vector3(1.9f, 1.0f, 0);
        context.ChoiceUI.transform.position = player.transform.position + offset;

        int sel = -1;
        yield return context.ChoiceUI?.ShowChoices(choice, i => sel = i);
        
        if (0 <= sel && sel < choice.options.Count)
            context.NextID = choice.options[sel].nextID;
        else
            context.NextID = row.nextID;

        (context.TextPresenter as DialogueTextPresenter)?.ClearAllStacked();
    }
}