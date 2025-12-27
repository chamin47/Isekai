using System.Collections;
using UnityEngine;

public class CameraZoomInCommand : IDialogueCommand
{
    private readonly bool _useStackedText;

    public CameraZoomInCommand(bool stackedText = false)
    {
        _useStackedText = stackedText;
    }

    public IEnumerator Execute(DialogueContext context, DialogueData data)
    {
        var (scale, dur, anchor) = ParamParser.Zoom3(data.eventParam, 1f, 0.5f, null);
        
        if (context.CameraService != null)
            context.Fire(context.CameraService as MonoBehaviour, 
                context.CameraService.ZoomTo(scale, dur, anchor));

        if (!string.IsNullOrWhiteSpace(data.animName) && context.ActorDirector != null)
            context.Fire(context.ActorDirector as MonoBehaviour,
                context.ActorDirector.PlayAnim(data.speaker, data.animName));

        if (!string.IsNullOrWhiteSpace(data.script))
        {
            if (_useStackedText)
                yield return (context.TextPresenter as DialogueTextPresenter)
                    ?.ShowTextStacked(data.speaker, data.script, data.animName);
            else
                yield return context.TextPresenter.ShowText(data.speaker, data.script, data.animName);
        }

        context.NextID = data.nextID;
    }
}