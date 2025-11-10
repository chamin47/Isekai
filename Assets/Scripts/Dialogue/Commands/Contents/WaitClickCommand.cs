using System.Collections;
using UnityEngine;

public class WaitClickCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        string eventParam = row.eventParam?.Trim().ToLower();
        int selection = -1;

        if (eventParam == "click_flower")
        {
            var ui = Managers.UI.ShowPopupUI<UI_Click_Flower>();
            yield return ui.ClickFlower(i => selection = i);
        }
        else if (eventParam == "click_lady")
        {
            var ladiesController = context.ActorDirector.GetComponent<LadiesController>();
            yield return ladiesController.ClickLady(i => selection = i);
        }

        yield return null;

        var clickTarget = context.ClickTargetTable?.Get(row.eventParam?.Trim());
        if (clickTarget == null)
        {
            Debug.LogError($"Click Target not found: {row.eventParam}");
            context.NextID = row.nextID;
            yield break;
        }

        Debug.Log($"WaitClick selection: {selection}");
        context.NextID = clickTarget.options[selection].nextID;
    }
}