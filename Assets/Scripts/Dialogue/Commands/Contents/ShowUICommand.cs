using System.Collections;
using UnityEngine;

public class ShowUICommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        var (name, duration) = ParamParser.UI(row.eventParam?.Trim());

        if (name == "portrait")
        {
            var ui = Managers.UI.ShowPopupUI<UI_Portrait>();
            yield return ui.Disapear(duration);
        }

        context.NextID = row.nextID;
    }
}