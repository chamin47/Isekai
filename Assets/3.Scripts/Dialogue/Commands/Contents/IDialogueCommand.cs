using System.Collections;

public interface IDialogueCommand
{
    IEnumerator Execute(DialogueContext context, DialogueData row);
}