using System.Collections;
using UnityEngine;

public class PlayAnimCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        Debug.Log("PlayAnim");
        
        float? duration = ParamParser.NullableFloat(row.eventParam);
        IEnumerator animCoroutine = null;
        
        if (context.ActorDirector != null)
            animCoroutine = context.ActorDirector.PlayAnim(row.speaker, row.animName, duration);

        bool noScript = string.IsNullOrWhiteSpace(row.script) || row.script == "null";
        if (noScript && animCoroutine != null)
        {
            yield return animCoroutine;
        }

        context.NextID = row.nextID;
    }
}