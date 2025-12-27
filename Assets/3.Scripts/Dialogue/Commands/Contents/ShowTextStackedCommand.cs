using System.Collections;

public class ShowTextStackedCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        if (!string.IsNullOrEmpty(row.script))
        {
            yield return context.TextPresenter.ShowTextStacked(row.speaker, row.script, row.animName);
        }

        context.NextID = row.nextID;
    }
}