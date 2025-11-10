using System.Collections;

public class WaitForInputCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        string userText = "";
        yield return context.InputPrompt?.Prompt(row.script, s => userText = s);

        var nextID = context.BranchTable?.Resolve(row.eventParam?.Trim(), "Default");
        
        context.TextPresenter.ClearAllStacked();

        context.NextID = nextID;
    }
}