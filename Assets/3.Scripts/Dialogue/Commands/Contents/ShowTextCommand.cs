using System.Collections;

public class ShowTextCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData data)
    {
        if (!string.IsNullOrEmpty(data.script))
            yield return context.TextPresenter.ShowText(data.speaker, data.script, data.animName);

        context.NextID = data.nextID;
    }
}