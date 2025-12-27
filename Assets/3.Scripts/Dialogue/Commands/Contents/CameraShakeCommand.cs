using System.Collections;
using UnityEngine;

public class CameraShakeCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData row)
    {
        var (mag, dur) = ParamParser.Floats2(row.eventParam?.Trim(), 0.2f, 0.4f);
        
        if (context.CameraService != null)
        {
            context.Fire(context.CameraService as MonoBehaviour, 
                context.CameraService.Shake(mag, dur));
        }

        if (!string.IsNullOrWhiteSpace(row.script))
            yield return context.TextPresenter?.ShowText(row.speaker, row.script, row.animName);

        context.NextID = row.nextID;
    }
}