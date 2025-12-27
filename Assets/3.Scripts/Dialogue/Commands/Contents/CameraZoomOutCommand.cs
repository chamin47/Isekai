using System.Collections;
using UnityEngine;

public class CameraZoomOutCommand : IDialogueCommand
{
    public IEnumerator Execute(DialogueContext context, DialogueData data)
    {
        var (scale, dur, anchor) = ParamParser.Zoom3(data.eventParam, 1f, 0.5f, null);
        
        if (context.CameraService != null)
        {
            bool hasScript = !string.IsNullOrWhiteSpace(data.script) && data.script != "null";
            
            if (hasScript)
            {
                context.Fire(context.CameraService as MonoBehaviour, 
                    context.CameraService.ZoomOutTo(scale, dur, anchor));
            }
            else
            {
                yield return context.CameraService.ZoomOutTo(scale, dur, anchor);
            }
        }

        if (!string.IsNullOrWhiteSpace(data.animName) && context.ActorDirector != null)
        {
            context.Fire(context.ActorDirector as MonoBehaviour,
                context.ActorDirector.PlayAnim(data.speaker, data.animName));
        }

        if (!string.IsNullOrWhiteSpace(data.script) && data.script != "null")
            yield return context.TextPresenter.ShowText(data.speaker, data.script, data.animName);

        context.NextID = data.nextID;
    }
}